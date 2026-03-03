using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramController(ITelegramBotClient bot) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] Update update)
    {
        if (update?.Message?.Text == null)
            return Ok();

        var chatId = update.Message.Chat.Id;
        var text = update.Message.Text;

        try
        {
            await bot.SendMessage(chatId, "You said: " + text);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return Ok();
    }
}