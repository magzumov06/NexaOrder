using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<User?> GetByTelegramId(long telegramId);
    Task Add(User user);
    Task<int> Count();
}