using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    [Required]
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public required string Username { get; set; }
    public required string Address {get; set;}
    public int Age { get; set; }
    public string Phone { get; set; }
    public bool IsBlocked { get; set; } =  false;
    public bool IsAdmin { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Order> Orders { get; set; } = new();
}