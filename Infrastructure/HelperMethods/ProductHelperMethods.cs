using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.Product;
using Domain.Entities;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.HelperMethods;

public class ProductHelperMethods(ITelegramBotClient bot,
    HttpClient httpClient)
{

    public async Task UpdateProduct(long chatId, string text)
    {
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4)
        {
            await bot.SendMessage(chatId,
                "❌ Format:\nId Name Price Quantity");
            return;
        }

        if (!int.TryParse(parts[0], out var id) ||
            !decimal.TryParse(parts[2], out var price) ||
            !int.TryParse(parts[3], out var quantity))
        {
            await bot.SendMessage(chatId, "❌ Invalid data");
            return;
        }

        var dto = new UpdateProductDto
        {
            Id = id,
            Name = parts[1],
            Price = price,
            Quantity = quantity
        };

        var response = await httpClient.PutAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/products",
            dto);

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "❌ Product not found");
            TelegramService.UserState[chatId] = "main";
            return;
        }

        await bot.SendMessage(chatId, "✅ Product updated");
        TelegramService.UserState[chatId] = "main";
    }
    
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
    
    public async Task<List<Product>> GetProductsFromApi()
    {
        var response = await httpClient.GetAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/products");

        if (!response.IsSuccessStatusCode)
            return new List<Product>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Product>>(json)
               ?? new List<Product>();
    }
}