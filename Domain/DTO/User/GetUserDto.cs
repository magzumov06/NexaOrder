namespace Domain.DTO.User;

public class GetUserDto
{
    public required string Username { get; set; }
    public required string Address {get; set;}
    public int Age { get; set; }
}