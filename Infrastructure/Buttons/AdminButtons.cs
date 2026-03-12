using Domain.CallBack;
using Telegram.Bot.Types.ReplyMarkups;

namespace Infrastructure.Buttons;

public static class AdminButtons
{
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
                InlineKeyboardButton.WithCallbackData("👤 Users", BotCallbacks.AdminUser)
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
                InlineKeyboardButton.WithCallbackData("🗑 Delete", BotCallbacks.DeleteProduct)
            ],
            [
                InlineKeyboardButton.WithCallbackData("✏️ Update", BotCallbacks.UpdateProduct)
            ],
            
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.AdminPanel)
            ]
        ]);
    }
    
    
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
                InlineKeyboardButton.WithCallbackData("✏️ Update Product", BotCallbacks.UpdateProduct)    
            ],
            [
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.AdminPanel)
            ]
        ]);
    }
    
    

    public static InlineKeyboardMarkup GetUserKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("❌ Delete User", BotCallbacks.DeleteUser)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔎 Get User By Id", BotCallbacks.GetUser)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("👥 Get Users", BotCallbacks.GetUsers)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Back", BotCallbacks.AdminPanel)
            }
        });
    }
    
}