using System.Text;
using System.Text.Json;
using Domain.CallBack;
using Domain.Entities;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class ProductCallbackHandler(ITelegramBotClient bot,
    HttpClient httpClient)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.CreateProduct:
                TelegramService.UserState[chatId] = "create_product";
                await bot.SendMessage(chatId,
                    "Send Product: Name Price Quantity");
                break;
            
            case BotCallbacks.DeleteProduct:
                TelegramService.UserState[chatId] = "waiting_delete_product";
                await bot.SendMessage(chatId, "Send Product ID");
                break;
            
            case BotCallbacks.GetProduct:
                TelegramService.UserState[chatId] = "waiting_get_product";
                await bot.SendMessage(chatId, "Send Product ID");
                break;

            case BotCallbacks.GetProducts:
                var products = await GetProductsFromApi();

                var text = new StringBuilder("📦 Product Info:\n\n");

                foreach (var p in products)
                {
                    text.AppendLine($"🆔 Id: {p.Id}");
                    text.AppendLine($"📛 Name: {p.Name}");
                    text.AppendLine($"💰 Price: {p.Price}");
                    text.AppendLine($"📦 Quantity: {p.Quantity}");             
                    text.AppendLine($"📝 Description: {p.Description ?? "–"}"); 
                    text.AppendLine($"🖼 ImageUrl: {p.ImageUrl ?? "–"}\n");
                }

                await bot.SendMessage(chatId, text.ToString());
                break;
        }
    }
    
    private async Task<List<Product>> GetProductsFromApi()
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