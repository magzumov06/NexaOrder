using Domain.CallBack;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Buttons;

public static class AdminButtons
{
    public static InlineKeyboardMarkup GetOrderAdminKeyboard()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("🔎 Get Order By Id", BotCallbacks.GetOrder)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🗑 Delete Order", BotCallbacks.DeleteOrder)
            ],
            [
                InlineKeyboardButton.WithCallbackData("GetOrders", BotCallbacks.GetOrders)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.AdminPanel)
            ]
        ]);
    }
    
    public static InlineKeyboardMarkup GetProductAdminKeyboard()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("➕ Create", BotCallbacks.CreateProduct)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔎 Get By Id", BotCallbacks.GetProduct)
            ],
            [
                InlineKeyboardButton.WithCallbackData("GetProducts", BotCallbacks.GetProducts)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🗑 Delete", BotCallbacks.DeleteProduct)
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.AdminPanel)
            ]
        ]);
    }
    
    public static InlineKeyboardMarkup GetAdminKeyboard()
    {
        return new InlineKeyboardMarkup([
            [
                InlineKeyboardButton.WithCallbackData("📦 Products", BotCallbacks.AdminProducts)
            ],
            [
                InlineKeyboardButton.WithCallbackData("📑 Orders",BotCallbacks.AdminOrders )
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.Back)
            ]
        ]);
    }
    
}