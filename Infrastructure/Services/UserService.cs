using Domain.DTO.User;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(DataContext context) : IUserService
{
    public async Task<string> CreateUserAsync(CreateUserDto createUserDto)
    {
        var phone = createUserDto.Phone.Replace(" ", "");

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Phone == phone);

        if (existingUser != null)
            return "User already exists";

        var newUser = new User()
        {
            Username = createUserDto.Username,
            Address = createUserDto.Address,
            Age = createUserDto.Age,
            Phone = phone,
            TelegramId = createUserDto.TelegramId,
            Role = phone.Contains("208020660")
                ? UserRole.Admin
                : UserRole.User,
            IsBlocked = false,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();

        Console.WriteLine($"TelegramId: {createUserDto.TelegramId}");
        return "User created";
    }

    public async Task<string> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x=>x.Id == updateUserDto.Id);
            if (user == null)
            {
                return "User not found";
            }
            user.Username = updateUserDto.Username;
            user.Address = updateUserDto.Address;
            user.Age = updateUserDto.Age;
            user.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return "User updated";
        }
        catch (Exception e)
        {
           throw new Exception("Error updating user" + e.Message);
        }
    }

    public async Task<string> DeleteUserAsync(int id)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return "User not found";
            }
            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return "User deleted";
        }
        catch (Exception e)
        {
            throw new Exception("Error deleting user" + e.Message);
        }
    }

    public async Task<GetUserDto?> GetUserAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return null;

        return new GetUserDto
        {
            Id = user.Id,
            TelegramId = user.TelegramId,
            Username = user.Username,
            Address = user.Address,
            Age = user.Age,
            Phone = user.Phone,
            Role = user.Role
        };
    }
    public async Task<List<GetUserDto>> GetUsersAsync()
    {
        try
        {
            var users = await context.Users
                .Select(u=>new GetUserDto()
                {
                    Id = u.Id,
                    TelegramId = u.TelegramId,
                    Username = u.Username,
                    Address = u.Address,
                    Age = u.Age,
                    Phone = u.Phone,
                }).ToListAsync();
            return users;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting users" + e.Message);
        }
    }

    public async Task<User?> GetUserByPhoneAsync(string phoneNumber)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Phone == phoneNumber);
    }

    public async Task<User?> GetRole(long telegramId)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        return user;
    }
}