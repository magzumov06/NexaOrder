using Domain.Entities;
using Domain.Enums;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class OrderService1 : IOrderService1
{
    private readonly IOrderService _orders;
    private readonly IUserService _users;

    public OrderService1(IOrderService orders, IUserService users)
    {
        _orders = orders;
        _users = users;
    }

    public async Task Create(int userId)
    {
        var order = new Order { UserId = userId };
        await _orders.Add(order);
    }

    public async Task<List<Order>> GetUserOrders(int userId)
        => await _orders.GetUserOrders(userId);

    public async Task<List<Order>> GetAll()
        => await _orders.GetAll();

    public async Task UpdateStatus(int id, OrderStatus status)
    {
        var order = await _orders.GetById(id)
                    ?? throw new Exception("Order not found");

        order.Status = status;
        await _orders.Add(order); // SaveChanges reuse
    }

    public async Task Delete(int id)
    {
        var order = await _orders.GetById(id)
                    ?? throw new Exception("Order not found");

        await _orders.Delete(order);
    }

    public async Task<(int users, int orders)> Stats()
    {
        return (await _users.Count(), await _orders.Count());
    }
}