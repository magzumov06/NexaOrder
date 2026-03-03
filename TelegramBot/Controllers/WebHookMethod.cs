using Domain.Enums;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
        
        if (update.Type == UpdateType.Message && update.Message!.Text != null)
        {
            var chatId = (int)update.Message.Chat.Id;
            var text = update.Message.Text;

        
            if (text.StartsWith("/order"))
            {
                await orderService.Create(chatId);

                await bot.SendMessage(
                    chatId,
                    "✅ Order created successfully!"
                );
            }

           
            if (text == "/myorders")
            {
                var orders = await orderService.GetUserOrders(chatId);

                var message = "📦 Your Orders:\n\n";

                foreach (var o in orders)
                {
                    message += $"ID: {o.Id} | Status: {o.Status}\n";
                }

                await bot.SendMessage(chatId, message);
            }
        }

       
        if (update.Type == UpdateType.CallbackQuery)
        {
            var callback = update.CallbackQuery!;
            var data = callback.Data;

            if (data!.StartsWith("Cancelled"))
            {
                var id = int.Parse(data.Split("_")[1]);

                await orderService.UpdateStatus(id, OrderStatus.Cancelled);

                await bot.AnswerCallbackQuery(callback.Id);
            }

            if (data.StartsWith("Processing"))
            {
                var id = int.Parse(data.Split("_")[1]);

                await orderService.UpdateStatus(id, OrderStatus.Processing);

                await bot.AnswerCallbackQuery(callback.Id);
            }
        }

        return Ok();
    }
}