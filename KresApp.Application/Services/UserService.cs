using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;
using KresApp.Domain.Enums;

namespace KresApp.Application.Services;

public class UserService
{
    private readonly IUserRepository _repo;
    private readonly IClassRepository _classRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ILdapService _ldap;

    public UserService(IUserRepository repo, IClassRepository classRepo, IPasswordHasher hasher, ILdapService ldap)
    {
        _repo = repo;
        _classRepo = classRepo;
        _hasher = hasher;
        _ldap = ldap;
    }

    public async Task<IEnumerable<UserListDto>> GetAll(UserRole? role = null)
    {
        var users = role.HasValue 
            ? await _repo.GetByRoleAsync(role.Value)
            : await _repo.GetAllAsync();

        var classes = await _classRepo.GetAllAsync();

        return users.Select(u => {
            var userClass = classes.FirstOrDefault(c => c.TeacherId == u.Id);
            return new UserListDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role,
                ClassId = userClass?.Id,
                ClassName = userClass?.Name
            };
        });
    }

    public async Task Create(CreateUserDto dto)
    {
        var existing = await _repo.GetByEmail(dto.Email);
        if (existing != null) throw new Exception("Bu e-posta adresi zaten kullanımda.");

        // LDAP/AD Kontrolü
        var ldapUser = await _ldap.GetUserInfoAsync(dto.Email);
        
        string hash;
        string name = dto.Name;
        string? phone = dto.Phone;

        if (ldapUser != null)
        {
            // LDAP'ta bulundu, şifre yönetimini LDAP'a bırakıyoruz
            hash = "LDAP_USER";
            name = string.IsNullOrWhiteSpace(ldapUser.Name) ? dto.Name : ldapUser.Name;
            phone = string.IsNullOrWhiteSpace(ldapUser.Phone) ? dto.Phone : ldapUser.Phone;
        }
        else
        {
            // Kurum domainleri için LDAP zorunluluğu (Opsiyonel: Eğer LDAP'ta yoksa ekletme)
            bool isCorporateDomain = dto.Email.Contains("@aile.gov.tr") || 
                                     dto.Email.Contains("@aile.bulutu") || 
                                     dto.Email.Contains(".local");
            
            if (isCorporateDomain)
            {
                throw new Exception("Bu e-posta adresi kurum rehberinde (AD/LDAP) bulunamadı. Lütfen bilgileri kontrol edin.");
            }

            hash = _hasher.Hash(dto.Password);
        }

        var user = new User(dto.Email, hash, dto.Role, name, phone);
        await _repo.AddAsync(user);
    }

    public async Task Update(Guid id, UpdateUserDto dto)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) throw new Exception("Kullanıcı bulunamadı.");

        var existingEmail = await _repo.GetByEmail(dto.Email);
        if (existingEmail != null && existingEmail.Id != id)
            throw new Exception("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");

        user.UpdateProfile(dto.Name, dto.Phone);
        user.UpdateEmail(dto.Email);
        user.UpdateRole(dto.Role);
        
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.ChangePassword(_hasher.Hash(dto.Password));
        }

        await _repo.UpdateAsync(user);
    }

    public async Task Delete(Guid id)
    {
        await _repo.DeleteAsync(id);
    }
}
