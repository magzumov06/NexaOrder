using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IOrderService
{
    Task Add(Order order);
    Task<List<Order>> GetUserOrders(int userId);
    Task<List<Order>> GetAll();
    Task<Order?> GetById(int id);
    Task Delete(Order order);
    Task<int> Count();
}