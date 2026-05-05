using KresApp.Application.DTOs;
using KresApp.Application.Interfaces;
using KresApp.Domain.Entities;

namespace KresApp.Application.Services;

public class MealMenuService
{
    private readonly IMealMenuRepository _repo;
    public MealMenuService(IMealMenuRepository repo) => _repo = repo;

    public async Task<List<MealMenuDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(x => new MealMenuDto
        {
            Id = x.Id, Day = x.Day, Breakfast = x.Breakfast, Lunch = x.Lunch, Snack = x.Snack
        }).ToList();
    }

    public async Task CreateAsync(CreateMealMenuDto dto)
    {
        await _repo.AddAsync(new MealMenu(dto.Day, dto.Breakfast, dto.Lunch, dto.Snack));
    }

    public async Task UpdateAsync(Guid id, CreateMealMenuDto dto)
    {
        var menu = await _repo.GetByIdAsync(id);
        if (menu == null) throw new Exception("Menu not found");
        menu.Update(dto.Day, dto.Breakfast, dto.Lunch, dto.Snack);
        await _repo.UpdateAsync(menu);
    }

    public async Task DeleteAsync(Guid id)
    {
        var menu = await _repo.GetByIdAsync(id);
        if (menu != null) await _repo.DeleteAsync(menu);
    }
}
