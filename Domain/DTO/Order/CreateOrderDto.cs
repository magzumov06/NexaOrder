using Domain.Enums;

namespace Domain.DTO.Order;

public class CreateOrderDto
{
    public int ProductId { get; set; }
    public required string Address { get; set; }
    public PaymentMethod PaymentMethod { get; set; } =  PaymentMethod.Cash;
    public int Quantity { get; set; } = 1;
}