using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserService1
{
    Task<User> GetOrCreate(long telegramId, string username);
}