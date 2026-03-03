using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(DataContext db) : IUserService
{
    public async Task<User?> GetByTelegramId(long telegramId)
        => await db.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);

    async Task IUserService.Add(User user)
    {
        await Add(user);
    }

    public async Task Add(User user)
    {
        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
    }

    async Task<User?> IUserService.GetByTelegramId(long telegramId)
    {
        return await GetByTelegramId(telegramId);
    }

    public async Task<int> Count()
        => await db.Users.CountAsync();
}