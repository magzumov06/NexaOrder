using Domain.CallBack;
using Infrastructure.Buttons;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class MainCallbackHandndler(ITelegramBotClient bot)
{
    public async Task Handle(long chatId,int messageId, string data)
    {
        switch (data)
        {
            case BotCallbacks.Products:
                await bot.EditMessageText(chatId,
                    messageId,
                    "🛍 Products",
                    replyMarkup: MainButtons.GetProductsKeyboard());
                break;
        }
    }
}