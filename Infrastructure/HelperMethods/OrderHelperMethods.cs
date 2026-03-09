using System.Net.Http.Json;
using Domain.DTO.Order;
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
                "Format: ProductId Quantity\nExample: 1 2");
            return;
        }

        if (!int.TryParse(parts[0], out var productId) ||
            !int.TryParse(parts[1], out var quantity))
        {
            await bot.SendMessage(chatId,
                "❌ Invalid numbers");
            return;
        }

        var dto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = quantity,
            Address = "Telegram Order",
            PaymentMethod = PaymentMethod.Cash
        };

        var response = await httpClient.PostAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/orders",
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

        var json = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId, json);

        TelegramService.UserState[chatId] = "main";
    }
    
}