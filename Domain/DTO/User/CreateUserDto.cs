namespace Domain.DTO.User;

public class CreateUserDto
{
    public string Username { get; set; }
    public string Address {get; set;}
    public int Age { get; set; }
    public required string Phone { get; set; }
    public long TelegramId { get; set; }
}