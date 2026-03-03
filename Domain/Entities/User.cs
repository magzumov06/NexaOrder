using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    [Required]
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public required string Username { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Order> Orders { get; set; } = new();
}