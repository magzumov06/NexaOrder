using System.Text;
using System.Text.Json;
using Domain.CallBack;
using Domain.Entities;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.Handlers;

public class OrderCallbackHandler(
    ITelegramBotClient bot,
    HttpClient httpClient)
{
    public async Task Handle(long chatId, string data)
    {
        switch (data)
        {
            case BotCallbacks.CreateOrder:
                TelegramService.UserState[chatId] = "create_order";

                await bot.SendMessage(chatId,
                    "Send ProductId Quantity\nExample: 1 2");
                break;

            case BotCallbacks.DeleteOrder:
                TelegramService.UserState[chatId] = "waiting_delete_order";

                await bot.SendMessage(chatId,
                    "Send Order ID");
                break;

            case BotCallbacks.GetOrder:
                TelegramService.UserState[chatId] = "waiting_get_order";

                await bot.SendMessage(chatId,
                    "Send Order ID");
                break;

            case BotCallbacks.GetOrders:

                var orders = await GetOrdersFromApi();

                var text = new StringBuilder("Orders:\n\n");

                foreach (var o in orders)
                {
                    text.AppendLine($"Id: {o.Id}");
                    text.AppendLine($"ProductId: {o.ProductId}");
                    text.AppendLine($"Quantity: {o.Quantity}");
                    text.AppendLine($"Address: {o.Address}");
                    text.AppendLine($"Payment: {o.PaymentMethod}");
                    text.AppendLine($"Status: {o.Status}");
                    text.AppendLine();
                }

                await bot.SendMessage(chatId, text.ToString());
                break;
        }
    }

    private async Task<List<Order>> GetOrdersFromApi()
    {
        var response = await httpClient.GetAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/orders");

        if (!response.IsSuccessStatusCode)
            return new List<Order>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Order>>(json)
               ?? new List<Order>();
    }
}