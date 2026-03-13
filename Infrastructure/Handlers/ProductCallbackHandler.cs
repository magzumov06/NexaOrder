using System.Text;
using System.Text.Json;
using Domain.CallBack;
using Domain.Entities;
using Infrastructure.HelperMethods;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class ProductCallbackHandler(ITelegramBotClient bot,
    ProductHelperMethods product)
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
            
            case BotCallbacks.UpdateProduct:
                TelegramService.UserState[chatId] = "update_product";
                await bot.SendMessage(chatId,
                    "Send Product Id");
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
                var products = await product.GetProductsFromApi();

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
}