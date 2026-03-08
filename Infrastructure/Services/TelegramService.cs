using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Domain.DTO.Order;
using Domain.DTO.User;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Buttons;
using Infrastructure.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    IOrderService orderService,
    HttpClient httpClient)
    : ITelegramService
{
    private static readonly Dictionary<long, string> UserState = new();
    private static readonly Dictionary<long, CreateUserDto> TempUsers = new();

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message != null)
            await HandleMessageAsync(update.Message);

        if (update.CallbackQuery != null)
            await HandleCallbackAsync(update.CallbackQuery);
    }

    private async Task HandleMessageAsync(Message message)
    {
        var chatId = message.Chat.Id;
        var text = message.Text ?? "";

        if (text == "/start")
        {
            await StartRegistration(chatId);
            return;
        }

        if (message.Contact != null)
        {
            await HandlePhone(chatId, message.Contact.PhoneNumber);
            return;
        }

        if (!UserState.TryGetValue(chatId, out var state))
            return;

        switch (state)
        {
            case "waiting_username":
                await HandleUsername(chatId, text);
                break;

            case "waiting_address":
                await HandleAddress(chatId, text);
                break;

            case "create_order":
                await HandleCreateOrder(chatId, text);
                break;

            case "create_product":
                await HandleCreateProduct(chatId, text);
                break;

            case "waiting_delete_product":
                await DeleteProduct(chatId, text);
                break;

            case "waiting_get_product":
                await GetProduct(chatId, text);
                break;

            case "waiting_delete_order":
                await DeleteOrder(chatId, text);
                break;

            case "waiting_get_order":
                await GetOrder(chatId, text);
                break;
        }
    }

    private async Task StartRegistration(long chatId)
    {
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(
            new[]
            {
                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                {
                    new("📱 Send Phone")
                    {
                        RequestContact = true
                    }
                }
            })
        {
            ResizeKeyboard = true
        };

        UserState[chatId] = "waiting_phone";

        await bot.SendMessage(chatId,
            "Барои сабти ном рақами телефони худро фиристед",
            replyMarkup: keyboard);
    }

    private async Task HandlePhone(long chatId, string phone)
    {
        TempUsers[chatId] = new CreateUserDto
        {
            Phone = phone,
            Username = null,
            Address = null
        };

        UserState[chatId] = "waiting_username";

        await bot.SendMessage(chatId,
            "Ташаккур! Акнун номатонро ворид кунед.");
    }

    private async Task HandleUsername(long chatId, string username)
    {
        TempUsers[chatId].Username = username;

        UserState[chatId] = "waiting_address";

        await bot.SendMessage(chatId,
            "Лутфан адрес / шаҳрро ворид кунед.");
    }

    private async Task HandleAddress(long chatId, string address)
    {
        TempUsers[chatId].Address = address;
        TempUsers[chatId].Age = 18;

        var dto = TempUsers[chatId];

        await httpClient.PostAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/users",
            dto);

        TempUsers.Remove(chatId);

        UserState[chatId] = "main";

        await bot.SendMessage(chatId,
            "✅ Сабти ном анҷом ёфт!",
            replyMarkup: MainButtons.GetMainKeyboard());
    }

    private async Task HandleCreateOrder(long chatId, string text)
    {
        var parts = text.Split(" ");

        if (parts.Length != 2)
        {
            await bot.SendMessage(chatId,
                "❌ Format: ProductId Quantity\nExample: 1 2");
            return;
        }

        var dto = new CreateOrderDto
        {
            ProductId = int.Parse(parts[0]),
            Quantity = int.Parse(parts[1]),
            Address = "Telegram Order",
            PaymentMethod = PaymentMethod.Cash
        };

        var response = await httpClient.PostAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/orders",
            dto);

        var result = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId,
            result,
            replyMarkup: MainButtons.GetMainKeyboard());

        UserState[chatId] = "main";
    }

    private async Task HandleCreateProduct(long chatId, string text)
    {
        var parts = text.Split(" ");

        if (parts.Length != 3)
        {
            await bot.SendMessage(chatId,
                "❌ Format: Name Price Quantity\nExample: Apple 10 5");
            return;
        }

        var product = new
        {
            Name = parts[0],
            Price = decimal.Parse(parts[1]),
            Quantity = int.Parse(parts[2])
        };

        await httpClient.PostAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/products",
            product);

        await bot.SendMessage(chatId, "✅ Product Created");

        UserState[chatId] = "main";
    }

    private async Task DeleteProduct(long chatId, string text)
    {
        if (!int.TryParse(text, out var id))
        {
            await bot.SendMessage(chatId, "❌ Invalid Product ID");
            return;
        }

        await httpClient.DeleteAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{id}");

        await bot.SendMessage(chatId, "🗑 Product Deleted");

        UserState[chatId] = "main";
    }

    private async Task GetProduct(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{text}");

        var json = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId, json);

        UserState[chatId] = "main";
    }

    private async Task DeleteOrder(long chatId, string text)
    {
        await httpClient.DeleteAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{text}");

        await bot.SendMessage(chatId, "🗑 Order Deleted");

        UserState[chatId] = "main";
    }

    private async Task GetOrder(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{text}");

        var json = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId, json);

        UserState[chatId] = "main";
    }

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message == null)
            return;

        var chatId = callback.Message.Chat.Id;

        switch (callback.Data)
        {
            case "create_order":
                UserState[chatId] = "create_order";

                await bot.SendMessage(chatId,
                    "📦 Send ProductId and Quantity\nExample:\n1 2");
                break;

            case "admin_panel":
                await bot.SendMessage(chatId,
                    "👑 Admin Panel",
                    replyMarkup: AdminButtons.GetAdminKeyboard());
                break;

            case "back":
                UserState[chatId] = "main";

                await bot.SendMessage(chatId,
                    "Main Menu",
                    replyMarkup: MainButtons.GetMainKeyboard());
                break;
        }

        await bot.AnswerCallbackQuery(callback.Id);
    }
}