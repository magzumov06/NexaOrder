using Domain.DTO.User;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<string> CreateUserAsync(CreateUserDto createUserDto);
    Task<string> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<string> DeleteUserAsync(int id);
    Task<GetUserDto?> GetUserAsync(int id);
    Task<List<GetUserDto>> GetUsersAsync();
    Task<User?> GetUserByPhoneAsync(string phoneNumber);
}