using Domain.CallBack;
using Infrastructure.Buttons;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class AdminCallbackHandler(ITelegramBotClient bot)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.AdminPanel:
                await bot.SendMessage(chatId,
                    "Admin Panel",
                    replyMarkup: AdminButtons.GetAdminKeyboard());
                break;

            case BotCallbacks.AdminProducts:
                await bot.SendMessage(chatId,
                    "Product Management",
                    replyMarkup: AdminButtons.GetProductAdminKeyboard());
                break;

            case BotCallbacks.AdminOrders:
                await bot.SendMessage(chatId,
                    "Order Management",
                    replyMarkup: AdminButtons.GetOrderAdminKeyboard());
                break;
            
            case BotCallbacks.AdminUser:
                await bot.SendMessage(chatId,
                    "User Management",
                    replyMarkup: AdminButtons.GetUserKeyboard());
                break;
        }
    }
}