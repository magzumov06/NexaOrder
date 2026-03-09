using Domain.CallBack;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class UserCallbackHandler(ITelegramBotClient bot,
    HttpClient httpClient)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.GetUser:
                TelegramService.UserState[chatId] = "waiting_get_user";
                await bot.SendMessage(chatId, "Send User ID");
                break;
        }
    } 
}