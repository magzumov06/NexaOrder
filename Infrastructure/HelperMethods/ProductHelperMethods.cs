using System.Text.Json;
using Domain.Entities;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.HelperMethods;

public class ProductHelperMethods(ITelegramBotClient bot,
    HttpClient httpClient)
{
    
    public async Task GetProduct(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{text}");

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "❌ Product not found");
            TelegramService.UserState[chatId] = "main";
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        var product = JsonSerializer.Deserialize<Product>(json);

        if (product == null)
        {
            await bot.SendMessage(chatId, "❌ Error reading product");
            TelegramService.UserState[chatId] = "main";
            return;
        }

        var message =
            $"📦 Product Info:\n\n" +
            $"🆔 Id: {product.Id}\n" +
            $"📛 Name: {product.Name}\n" +
            $"💰 Price: {product.Price}\n" +
            $"📦 Quantity: {product.Quantity}";

        await bot.SendMessage(chatId, message);

        TelegramService.UserState[chatId] = "main";
    }
}