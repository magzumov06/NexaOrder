using Domain.DTO.Order;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class OrderService(DataContext context) : IOrderService
{
    public async Task<string> CreateOrderAsync(long telegramId ,CreateOrderDto orderDto)
    {

        var user = await context.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);

        if (user == null)
            return "User not found";
       
        var product = await context.Products
            .FirstOrDefaultAsync(x => x.Id == orderDto.ProductId);

        if (product == null)
            return "❌ Product not found";

      
        if (product.Quantity <= 0)
            return "❌ Product is out of stock";

        if (product.Quantity < orderDto.Quantity)
            return "❌ Not enough product quantity available";

        
        product.Quantity -= orderDto.Quantity;

        var order = new Order
        {
            UserId = user.Id,
            Address = orderDto.Address,
            PaymentMethod = orderDto.PaymentMethod,
            Status = OrderStatus.Processing,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ProductId = product.Id,
            Quantity = orderDto.Quantity
        };

        await context.Orders.AddAsync(order);

        await context.SaveChangesAsync();

        
        if (orderDto.PaymentMethod == PaymentMethod.Online)
        {
            return
                "💳 Online Payment\n" +
                "🏦 Bank: Душанбе Сити\n" +
                "💳 Card: 1234567890\n\n" +
                "📌 Please transfer the amount.";
        }

        return "✅ Order created successfully (Cash payment)";
    }

    public async Task<string> UpdateOrderAsync(UpdateOrderDto orderDto)
    {
        try
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(x => x.Id == orderDto.Id);

            if (order == null)
                return "❌ Order not found";

            order.Address = orderDto.Address ?? order.Address;
            order.PaymentMethod = orderDto.PaymentMethod;
            order.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return "✅ Order updated successfully";
        }
        catch (Exception e)
        {
            throw new Exception("Error in UpdateOrder");
        }
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        try
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return false;
            
            var product = await context.Products
                .FirstOrDefaultAsync(x => x.Id == order.ProductId);
            
            if (product != null)
            {
                product.Quantity += order.Quantity;
            }
            
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            throw new Exception("Error in DeleteOrder");
        }
    }

    public async Task<GetOrderDto?> GetOrderAsync(int id)
    {
        try
        {
            var order = await context.Orders
                .FirstOrDefaultAsync(x => x.Id == id);
            if (order == null)
                return null;
            var dto = new GetOrderDto()
            {
                Id = order.Id,
                Address = order.Address,
                PaymentMethod = order.PaymentMethod,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
            };
            return dto;
        }
        catch (Exception e)
        {
           throw new Exception("Error in GetOrder");
        }
    }

    public async Task<List<GetOrderDto>> GetOrderAsync()
    {
        try
        {
            var order = await context.Orders
                .Select(o=> new GetOrderDto()
                {
                    Id = o.Id,
                    Address = o.Address,
                    PaymentMethod = o.PaymentMethod,
                    ProductId = o.ProductId,
                    Quantity = o.Quantity,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                }).ToListAsync();
            return order;
        }
        catch (Exception e)
        {
            throw new Exception("Error in GetOrder");
        }
    }
}