using System.Text;
using Domain.CallBack;
using Infrastructure.HelperMethods;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class UserCallbackHandler(ITelegramBotClient bot,
    UserHelperMethods user)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.GetUser:
                TelegramService.UserState[chatId] = "waiting_get_user";
                await bot.SendMessage(chatId, "Send User ID");
                break;
            
            case BotCallbacks.DeleteUser:
                TelegramService.UserState[chatId] = "waiting_delete_user";
                await bot.SendMessage(chatId, "Send User ID");
                break;
            
            case BotCallbacks.GetUsers:
                var users = await user.GetUsers();

                var text = new StringBuilder("Users: \n\n");

                foreach (var u in users)
                {
                    text.AppendLine($"ID: {u.Id}");
                    text.AppendLine($"Username: {u.Username}");
                    text.AppendLine($"Age: {u.Age}");
                    text.AppendLine($"Phone: {u.Phone}");
                    text.AppendLine($"Address: {u.Address}");
                    text.AppendLine($"Role: {u.Role}");
                }
                
                await bot.SendMessage(chatId, text.ToString());
                break;
        }
    } 
}