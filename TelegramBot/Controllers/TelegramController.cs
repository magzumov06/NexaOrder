using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramController(ITelegramService telegramService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Update([FromBody] Update update)
    {
        await telegramService.HandleUpdateAsync(update);
        return Ok();
    }
}