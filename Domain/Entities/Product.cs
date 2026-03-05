using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Product
{
    public int  Id { get; set; }
    [Required]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    
    public List<Order>? Orders { get; set; }
}