using Domain.CallBack;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Buttons;

public static class MainButtons
{
    public static InlineKeyboardMarkup GetMainKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🛒 Create Order", BotCallbacks.CreateOrder)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🛍 Products", BotCallbacks.Products)
            }
        });
    }
}