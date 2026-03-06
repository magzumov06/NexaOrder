using Domain.DTO.Order;
using Infrastructure.Interfaces;
using Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    IOrderService orderService) : ITelegramService
{
    private static readonly Dictionary<long, string> UserState = new();

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message != null)
            await HandleMessageAsync(update.Message);

        if (update.CallbackQuery != null)
            await HandleCallbackAsync(update.CallbackQuery);
    }

    private async Task HandleMessageAsync(Message message)
    {
        var chatId = message.Chat.Id;
        var text = message.Text ?? "";
        
        if (text == "/start")
        {
            UserState[chatId] = "main";

            await bot.SendMessage(
                chatId,
                "👋 Welcome to NexaOrderBot",
                replyMarkup: GetMainKeyboard()
            );

            return;
        }

        if (UserState.ContainsKey(chatId) &&
            UserState[chatId] == "create_order")
        {
            var parts = text.Split(" ");

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int productId) ||
                !int.TryParse(parts[1], out int quantity))
            {
                await bot.SendMessage(
                    chatId,
                    "❌ Format: ProductId Quantity\nExample: 1 2"
                );
                return;
            }

            var dto = new CreateOrderDto
            {
                ProductId = productId,
                Quantity = quantity,
                Address = "Telegram Order",
                PaymentMethod = PaymentMethod.Cash
            };

            var result = await orderService.CreateOrderAsync(dto);

            UserState[chatId] = "main";

            await bot.SendMessage(
                chatId,
                result,
                replyMarkup: GetMainKeyboard()
            );

            return;
        }
    }
    

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message == null)
            return;

        var chatId = callback.Message.Chat.Id;
        var data = callback.Data;

        if (data == "create_order")
        {
            UserState[chatId] = "create_order";

            await bot.SendMessage(
                chatId,
                "📦 Send ProductId and Quantity\nFormat:\n1 2",
                replyMarkup: GetBackKeyboard()
            );
        }

        if (data == "my_orders")
        {
            UserState[chatId] = "orders";

            var orders = await orderService.GetOrderAsync();

            if (!orders.Any())
            {
                await bot.SendMessage(
                    chatId,
                    "❌ No orders",
                    replyMarkup: GetBackKeyboard()
                );

                await bot.AnswerCallbackQuery(callback.Id);
                return;
            }

            var text = "📦 Orders:\n\n";

            foreach (var o in orders)
            {
                text += $"Order #{o.Id}\n";
                text += $"Product: {o.ProductId}\n";
            }

            await bot.SendMessage(
                chatId,
                text,
                replyMarkup: GetBackKeyboard()
            );
        }

        if (data == "go_back")
        {
            UserState[chatId] = "main";

            await bot.SendMessage(
                chatId,
                "🔙 Main Menu",
                replyMarkup: GetMainKeyboard()
            );
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

    private InlineKeyboardMarkup GetBackKeyboard()
    {
        return new InlineKeyboardMarkup(
            InlineKeyboardButton.WithCallbackData("🔙 Go Back", "go_back")
        );
    }
}