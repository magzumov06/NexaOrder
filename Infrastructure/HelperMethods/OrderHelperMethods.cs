using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.Order;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.HelperMethods;

public class OrderHelperMethods(ITelegramBotClient bot,
    HttpClient httpClient)
{
    public async Task CreateOrder(long chatId, string text)
    {
        var parts = text.Split(" ");

        if (parts.Length != 2)
        {
            await bot.SendMessage(chatId,
                "Format: ProductId Quantity YourAddress\nExample: 1 2 address");
            return;
        }

        if (!int.TryParse(parts[0], out var productId) ||
            !int.TryParse(parts[1], out var quantity))
        {
            await bot.SendMessage(chatId,
                "❌ Invalid numbers");
            return;
        }
        
        var address = parts[2];
        
        var dto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = quantity,
            Address = address,
            PaymentMethod = PaymentMethod.Cash
        };

        var response = await httpClient.PostAsJsonAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders?telegramId={chatId}",
            dto);

        var result = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId, result);

        TelegramService.UserState[chatId] = "main";
    }
    
    public async Task DeleteOrder(long chatId, string text)
    {
        await httpClient.DeleteAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{text}");

        await bot.SendMessage(chatId, "Order Deleted");

        TelegramService.UserState[chatId] = "main";
    }

    public async Task GetOrder(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{text}");

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "Order Not Found");
            TelegramService.UserState[chatId] = "main";
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        var order = JsonSerializer.Deserialize<Order>(json);

        if (order == null)
        {
            await bot.SendMessage(chatId, "Order Not Found");
            TelegramService.UserState[chatId] = "main";
            return;
        }

        var message =
            $"Order information:\n\n" +
            $"Id: {order.Id}\n" +
            $"Address: {order.Address}\n" +
            $"Payment Method: {order.PaymentMethod}\n" +
            $"Status: {order.Status}\n";



    await bot.SendMessage(chatId, message);

        TelegramService.UserState[chatId] = "main";
    }
    
    public async Task<List<Order>> GetOrdersFromApi()
    {
        var response = await httpClient.GetAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/orders");

        if (!response.IsSuccessStatusCode)
            return new List<Order>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Order>>(json)
               ?? new List<Order>();
    }
    
}