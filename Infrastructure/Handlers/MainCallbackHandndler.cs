using Domain.CallBack;
using Infrastructure.Buttons;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class MainCallbackHandndler(ITelegramBotClient bot)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.GetProduct:
                await bot.SendMessage(chatId,
                    "Products",
                    replyMarkup: MainButtons.GetProductsKeyboard());
                break;
        }
    }
}