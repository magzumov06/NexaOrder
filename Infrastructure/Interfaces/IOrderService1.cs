using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Interfaces;

public interface IOrderService1
{
    Task Create(int userId);
    Task<List<Order>> GetUserOrders(int userId);
    Task<List<Order>> GetAll();
    Task UpdateStatus(int id, OrderStatus status);
    Task Delete(int id);
    Task<(int users, int orders)> Stats();
}