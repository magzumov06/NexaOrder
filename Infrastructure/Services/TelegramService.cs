using Domain.DTO.Order;
using Infrastructure.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    IOrderService orderService) : ITelegramService
{
    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message != null)
        {
            await HandleMessageAsync(update.Message);
        }

        if (update.CallbackQuery != null)
        {
            await HandleCallbackAsync(update.CallbackQuery);
        }
    }
    

    private async Task HandleMessageAsync(Message message)
    {
        var chatId = message.Chat.Id;
        var text = message.Text;

        if (text == "/start")
        {
            var keyboard = GetMainKeyboard();

            await bot.SendMessage(
                chatId,
                "👋 Welcome to NexaOrderBot",
                replyMarkup: keyboard
            );
            return;
        }

        // create order format: productId quantity
        var parts = text.Split(" ");

        if (parts.Length == 2)
        {
            int productId = int.Parse(parts[0]);
            int quantity = int.Parse(parts[1]);

            var dto = new CreateOrderDto
            {
                ProductId = productId,
                Quantity = quantity,
                Address = "Telegram Order",
                PaymentMethod = PaymentMethod.Cash
            };

            var result = await orderService.CreateOrderAsync(dto);

            await bot.SendMessage(chatId, result);
        }
    }


   

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message != null)
        {
            var chatId = callback.Message.Chat.Id;

            if (callback.Data == "create_order")
            {
                await bot.SendMessage(
                    chatId,
                    "Send ProductId and Quantity"
                );
            }

            if (callback.Data == "my_orders")
            {
                var orders = await orderService.GetOrderAsync();

                if (!orders.Any())
                {
                    await bot.SendMessage(chatId, "❌ No orders");
                    return;
                }

                var text = "📦 Orders:\n\n";

                foreach (var o in orders)
                {
                    text += $"Order #{o.Id}\n";
                    text += $"Product: {o.ProductId}\n\n";
                }

                await bot.SendMessage(chatId, text);
            }
        }

        await bot.AnswerCallbackQuery(callback.Id);
    }

   

    private InlineKeyboardMarkup GetMainKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🛒 Create Order", "create_order")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📦 My Orders", "my_orders")
            }
        });
    }

   
}
