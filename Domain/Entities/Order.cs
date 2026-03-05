using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    [Required]
    public required string Address { get; set; }
    public PaymentMethod PaymentMethod { get; set; } =  PaymentMethod.Cash;
    public OrderStatus Status { get; set; } = OrderStatus.Processing;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } 
    public User? User { get; set; }
    public Product? Product { get; set; } 
}