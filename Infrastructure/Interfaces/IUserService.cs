using Domain.DTO.User;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<string> CreateUserAsync(CreateUserDto createUserDto);
    Task<string> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
    Task<GetUserDto> GetUserAsync(int id);
    Task<List<GetUserDto>> GetUsersAsync();
}