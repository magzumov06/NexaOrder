using Domain.CallBack;
using Infrastructure.Buttons;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class AdminCallbackHandler(ITelegramBotClient bot)
{
    public async Task Handle(long chatId, int messageId, string data)
    {
        switch (data)
        {
            case BotCallbacks.AdminPanel:

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🛠 Admin Panel",
                    replyMarkup: AdminButtons.GetAdminKeyboard());

                break;

            case BotCallbacks.AdminProducts:

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "📦 Product Management",
                    replyMarkup: AdminButtons.GetProductAdminKeyboard());

                break;

            case BotCallbacks.AdminOrders:

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🧾 Order Management",
                    replyMarkup: AdminButtons.GetOrderAdminKeyboard());

                break;

            case BotCallbacks.AdminUser:

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "👤 User Management",
                    replyMarkup: AdminButtons.GetUserKeyboard());

                break;
        }
    }
}