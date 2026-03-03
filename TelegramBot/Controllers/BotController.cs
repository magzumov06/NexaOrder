using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Controllers;

[ApiController]
[Route("bot")]
public class BotController(
    ITelegramBotClient bot,
    IUserService1 users,
    IOrderService1 orders)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Update(
        [FromBody] Update update)
    {
        if (!Request.Headers.TryGetValue(
                "8619247540:AAHf11S",
                out var token) || token != "8619247540:AAHf11SnSOjGZKMCoptYhZEDgVaXolGnbIE")
            return Unauthorized();

        if (update.Message?.Text == "/start")
        {
            var user = await users.GetOrCreate(
                update.Message.Chat.Id,
                update.Message.Chat.Username ?? "unknown");

            await bot.SendMessage(
                user.TelegramId,
                "Menu:\n1️⃣ Order\n2️⃣ My Orders");
        }

        if (update.Message?.Text == "Order")
        {
            var user = await users.GetOrCreate(
                update.Message.Chat.Id,
                update.Message.Chat.Username ?? "unknown");

            await orders.Create(user.Id);

            await bot.SendMessage(
                user.TelegramId,
                "✅ Order Created");
        }

        if (update.Message?.Text == "My Orders")
        {
            var user = await users.GetOrCreate(
                update.Message.Chat.Id,
                update.Message.Chat.Username ?? "unknown");

            var list = await orders.GetUserOrders(user.Id);

            var text = string.Join("\n",
                list.Select(x => $"#{x.Id} - {x.Status}"));

            await bot.SendMessage(
                user.TelegramId,
                text == "" ? "No orders" : text);
        }

        return Ok();
    }
}