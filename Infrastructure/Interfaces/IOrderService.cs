using Domain.DTO.Order;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task<string> CreateOrderAsync(long telegramId,CreateOrderDto order);
    Task<string> UpdateOrderAsync(UpdateOrderDto order);
    Task<bool> DeleteOrderAsync(int id);
    Task<GetOrderDto?> GetOrderAsync(int id);
    Task<List<GetOrderDto>> GetOrderAsync();
}