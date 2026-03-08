namespace Domain.DTO.User;

public class CreateUserDto
{
    public required string Username { get; set; }
    public required string Address {get; set;}
    public int Age { get; set; }
    public string Phone { get; set; }
}