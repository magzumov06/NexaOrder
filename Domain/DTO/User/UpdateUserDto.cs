namespace Domain.DTO.User;

public class UpdateUserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Address {get; set;}
    public int Age { get; set; }
}