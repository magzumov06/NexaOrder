using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.Order;
using Domain.DTO.User;
using Domain.Entities;
using Domain.Enums;
using Domain.CallBack;
using Infrastructure.Buttons;
using Infrastructure.Handlers;
using Infrastructure.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

using User = Domain.Entities.User;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    HttpClient httpClient) : ITelegramService
{
    private readonly AdminCallbackHandler _adminCallback = new(bot);
    private readonly OrderCallbackHandler _orderCallback = new(bot, httpClient);
    private readonly ProductCallbackHandler _productCallback = new(bot, httpClient);

    public static readonly Dictionary<long, string> UserState = new();
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

            case "waiting_delete_order":
                await DeleteOrder(chatId, text);
                break;

            case "waiting_get_order":
                await GetOrder(chatId, text);
                break;

            case "waiting_get_product":
                await GetProduct(chatId, text);
                break;

            case "waiting_get_user":
                await GetUser(chatId, text);
                break;
        }
    }

    // ================= REGISTRATION =================

    private async Task StartRegistration(long chatId)
    {
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(
            new[]
            {
                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton[]
                {
                    new("📱 Send Phone") { RequestContact = true }
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
            Address = null,
            Username = null
        };

        UserState[chatId] = "waiting_username";

        await bot.SendMessage(chatId, "Номатонро ворид кунед:");
    }

    private async Task HandleUsername(long chatId, string username)
    {
        TempUsers[chatId].Username = username;

        UserState[chatId] = "waiting_address";

        await bot.SendMessage(chatId, "Адрес / шаҳрро ворид кунед:");
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
            "✅ Registration completed",
            replyMarkup: MainButtons.GetMainKeyboard());
    }

  

    private async Task HandleCreateOrder(long chatId, string text)
    {
        var parts = text.Split(" ");

        if (parts.Length != 2)
        {
            await bot.SendMessage(chatId,
                "Format: ProductId Quantity\nExample: 1 2");
            return;
        }

        if (!int.TryParse(parts[0], out var productId) ||
            !int.TryParse(parts[1], out var quantity))
        {
            await bot.SendMessage(chatId,
                "❌ Invalid numbers");
            return;
        }

        var dto = new CreateOrderDto
        {
            ProductId = productId,
            Quantity = quantity,
            Address = "Telegram Order",
            PaymentMethod = PaymentMethod.Cash
        };

        var response = await httpClient.PostAsJsonAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/orders",
            dto);

        var result = await response.Content.ReadAsStringAsync();

        await bot.SendMessage(chatId, result);

        UserState[chatId] = "main";
    }

    private async Task DeleteOrder(long chatId, string text)
    {
        await httpClient.DeleteAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{text}");

        await bot.SendMessage(chatId, "Order Deleted");

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

    private async Task GetProduct(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{text}");

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "❌ Product not found");
            UserState[chatId] = "main";
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        var product = JsonSerializer.Deserialize<Product>(json);

        if (product == null)
        {
            await bot.SendMessage(chatId, "❌ Error reading product");
            UserState[chatId] = "main";
            return;
        }

        var message =
            $"📦 Product Info:\n\n" +
            $"🆔 Id: {product.Id}\n" +
            $"📛 Name: {product.Name}\n" +
            $"💰 Price: {product.Price}\n" +
            $"📦 Quantity: {product.Quantity}";

        await bot.SendMessage(chatId, message);

        UserState[chatId] = "main";
    }

    private async Task GetUser(long chatId, string text)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/{text}");

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "❌ User not found");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        var user = JsonSerializer.Deserialize<User>(json);

        if (user == null)
            return;

        var message =
            $"User Info:\n" +
            $"Id: {user.Id}\n" +
            $"Username: {user.Username}\n" +
            $"Phone: {user.Phone}\n" +
            $"Address: {user.Address}\n" +
            $"Age: {user.Age}";

        await bot.SendMessage(chatId, message);

        UserState[chatId] = "main";
    }

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message == null)
            return;

        var chatId = callback.Message.Chat.Id;
        var data = callback.Data;

        switch (data)
        {
            case BotCallbacks.Back:
                UserState[chatId] = "main";

                await bot.SendMessage(chatId,
                    "Main Menu",
                    replyMarkup: MainButtons.GetMainKeyboard());
                break;
        }

        await _adminCallback.Handle(chatId, data);
        await _orderCallback.Handle(chatId, data);
        await _productCallback.Handle(chatId, data);

        await bot.AnswerCallbackQuery(callback.Id);
    }
}