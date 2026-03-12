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

    public static InlineKeyboardMarkup GetProductsKeyboard()
    {
        return new InlineKeyboardMarkup([

            [
                InlineKeyboardButton.WithCallbackData("🔎 Get By Id", BotCallbacks.GetProduct)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔎 GetProducts", BotCallbacks.GetProducts)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.Back)
            ]
        ]);
    }
}