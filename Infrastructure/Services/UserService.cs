using Domain.DTO.User;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class UserService(DataContext context) : IUserService
{
    public async Task<string> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            // var newUser = new User()
            // {
            //     Username = createUserDto.Username,
            //     Address = createUserDto.Address,
            //     Age = createUserDto.Age, 
            //     Phone = createUserDto.Phone,
            //     IsBlocked = false,
            //     IsAdmin = false,
            //     CreatedAt = DateTime.UtcNow
            //     
            // };
            // await context.Users.AddAsync(newUser);
            // await context.SaveChangesAsync();
            // if (newUser.Id == 1)
            // {
            //     newUser.IsAdmin = true;
            //     await context.SaveChangesAsync();
            // }
            return "User created";
        }
        catch (Exception e)
        {
            throw new Exception("Error creating user" + e.Message);
        }
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
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return null;
            }

            var dto = new GetUserDto()
            {
                Username = user.Username,
                Address = user.Address,
                Age = user.Age,
                Phone = user.Phone,
            };
            return dto;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting user" + e.Message);
        }
    }

    public async Task<List<GetUserDto>> GetUsersAsync()
    {
        try
        {
            var users = await context.Users
                .Select(u=>new GetUserDto()
                {
                    Username = u.Username,
                    Address = u.Address,
                    Age = u.Age,
                    Phone = u.Phone
                }).ToListAsync();
            return users;
        }
        catch (Exception e)
        {
            throw new Exception("Error getting users" + e.Message);
        }
    }
}