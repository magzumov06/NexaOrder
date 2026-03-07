using Domain.DTO.User;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class UserService(DataContext context) : IUserService
{
    public async Task<string> CreateUserAsync(CreateUserDto createUserDto)
    {
        try
        {
            var newUser = new User()
            {
                Username = createUserDto.Username,
                Address = createUserDto.Address,
                Age = createUserDto.Age,
                CreatedAt = DateTime.UtcNow,
            };
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();
            return "User created";
        }
        catch (Exception e)
        {
            throw new Exception("Error creating user" + e.Message);
        }
    }

    public async Task<string> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<GetUserDto> GetUserAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<GetUserDto>> GetUsersAsync()
    {
        throw new NotImplementedException();
    }
}