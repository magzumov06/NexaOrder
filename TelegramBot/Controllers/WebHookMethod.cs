using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Controllers;

[ApiController]
[Route("api/telegram")]
public class TelegramController(
    ITelegramBotClient bot,
    IOrderService1 orderService) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] Update update)
    {
        if (update == null)
            return Ok();

      

        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text;

            if (text.StartsWith("/order"))
            {
                await orderService.Create((int)chatId);

                await bot.SendMessage(
                    chatId: chatId,
                    text: "✅ Order created successfully!"
                );
            }

            if (text == "/myorders")
            {
                var orders = await orderService.GetUserOrders((int)chatId);

                var message = "📦 Your Orders:\n\n";

                foreach (var o in orders)
                {
                    message += $"ID: {o.Id} | Status: {o.Status}\n";
                }

                if (string.IsNullOrEmpty(message))
                    message = "No orders yet.";

                await bot.SendMessage(
                    chatId: chatId,
                    text: message
                );
            }
        }


        if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            var callback = update.CallbackQuery;
            var data = callback.Data;

            if (!string.IsNullOrEmpty(data))
            {
                if (data.StartsWith("Cancelled"))
                {
                    var id = int.Parse(data.Split("_")[1]);

                    await orderService.UpdateStatus(id, OrderStatus.Cancelled);
                }

                if (data.StartsWith("Processing"))
                {
                    var id = int.Parse(data.Split("_")[1]);

                    await orderService.UpdateStatus(id, OrderStatus.Processing);
                }

                await bot.AnswerCallbackQuery(callback.Id);
            }
        }

        return Ok();
    }
}