using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using KresApp.Application.Interfaces;

namespace KresApp.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration configuration)
    {
        var jwtKey = configuration["Jwt:Key"];
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        if (string.IsNullOrWhiteSpace(jwtKey))
        {
            throw new InvalidOperationException("Configuration value 'Jwt:Key' is not set or is empty. Please set Jwt:Key in configuration.");
        }
        if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
        {
            throw new InvalidOperationException("Configuration value 'Jwt:Key' must be at least 32 bytes for HmacSha256.");
        }
        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException("Configuration value 'Jwt:Issuer' is not set or is empty.");
        }
        if (string.IsNullOrWhiteSpace(audience))
        {
            throw new InvalidOperationException("Configuration value 'Jwt:Audience' is not set or is empty.");
        }

        _jwtKey = jwtKey;
        _issuer = issuer;
        _audience = audience;
    }

    public string Generate(Guid userId, string role, string email)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Email, email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtKey)
        );

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: null,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}