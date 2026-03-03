using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService(DataContext db) : IOrderService
{
    public async Task Add(Order order)
    {
        await db.Orders.AddAsync(order);
        await db.SaveChangesAsync();
    }

    public async Task<List<Order>> GetUserOrders(int userId)
        => await db.Orders
            .Where(x => x.UserId == userId)
            .ToListAsync();

    public async Task<List<Order>> GetAll()
        => await db.Orders
            .Include(x => x.User)
            .ToListAsync();

    public async Task<Order?> GetById(int id)
        => await db.Orders.FindAsync(id);

    public async Task Delete(Order order)
    {
        db.Orders.Remove(order);
        await db.SaveChangesAsync();
    }

    public async Task<int> Count()
        => await db.Orders.CountAsync();
}