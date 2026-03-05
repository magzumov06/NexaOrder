using Domain.Enums;

namespace Domain.DTO.Order;

public class GetOrderDto
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public PaymentMethod PaymentMethod { get; set; } =  PaymentMethod.Cash;
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } 
}