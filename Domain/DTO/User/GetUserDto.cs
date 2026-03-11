using Domain.Enums;

namespace Domain.DTO.User;

public class GetUserDto
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public required string Username { get; set; }
    public required string Address {get; set;}
    public int Age { get; set; }
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
}