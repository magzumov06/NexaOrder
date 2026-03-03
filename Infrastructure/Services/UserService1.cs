using Domain.Entities;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class UserService1(IUserService repo) : IUserService1
{
    public async Task<User> GetOrCreate(long telegramId, string username)
    {
        var user = await repo.GetByTelegramId(telegramId);

        if (user != null)
            return user;

        user = new User
        {
            TelegramId = telegramId,
            Username = username
        };

        await repo.Add(user);
        return user;
    }
}