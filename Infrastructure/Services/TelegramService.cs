using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.Order;
using Domain.DTO.User;
using Domain.Entities;
using Domain.Enums;
using Domain.CallBack;
using Infrastructure.Buttons;
using Infrastructure.Handlers;
using Infrastructure.HelperMethods;
using Infrastructure.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

using User = Domain.Entities.User;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    HttpClient httpClient,
    OrderHelperMethods methods,
    ProductHelperMethods product) : ITelegramService
{
    private readonly AdminCallbackHandler _adminCallback = new(bot);
    private readonly OrderCallbackHandler _orderCallback = new(bot, httpClient);
    private readonly ProductCallbackHandler _productCallback = new(bot, httpClient);
    private readonly UserCallbackHandler _userCallback = new(bot, httpClient);

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
        
        if (text == "/admin")
        {
            await ShowAdminPanel(chatId);
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
                await methods.CreateOrder(chatId, text);
                break;

            case "waiting_delete_order":
                await methods.DeleteOrder(chatId, text);
                break;
            case "waiting_delete_user":
                await methods.DeleteUser(chatId, text);
                break;

            case "waiting_get_order":
                await methods.GetOrder(chatId, text);
                break;

            case "waiting_get_product":
                await product.GetProduct(chatId, text);
                break;

            case "waiting_get_user":
                await GetUser(chatId, text);
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
        phone = phone.Replace(" ", "");

        if (!phone.StartsWith("+"))
            phone = "+" + phone;

        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/phone/{phone}");

        if (response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId,
                "✅ Шумо аллакай регистрация кардаед",
                replyMarkup: MainButtons.GetMainKeyboard());

            UserState[chatId] = "main";
            return;
        }

        TempUsers[chatId] = new CreateUserDto
        {
            Phone = phone,
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

    private async Task HandleAddress(long chatId, string text)
    {
        var dto = TempUsers[chatId];

        dto.Address = text;
        dto.TelegramId = chatId;   // <- Ин ҷо муҳим аст

        Console.WriteLine($"DTO перед отправкой: {JsonSerializer.Serialize(dto)}"); // debug

        var response = await httpClient.PostAsJsonAsync<CreateUserDto>(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/user",
            dto);

        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response API: {result}"); // debug

        TempUsers.Remove(chatId);
        UserState[chatId] = "main";

        await bot.SendMessage(chatId,
            "✅ Registration completed",
            replyMarkup: MainButtons.GetMainKeyboard());
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
            $"TelegramId: {user.TelegramId}\n" +
            $"Username: {user.Username}\n" +
            $"Phone: {user.Phone}\n" +
            $"Address: {user.Address}\n" +
            $"Age: {user.Age}\n"+
            $"Role: {user.Role}";

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
        await _userCallback.Handle(chatId, data);

        await bot.AnswerCallbackQuery(callback.Id);
    }
    
    
    private async Task ShowAdminPanel(long chatId)
    {
        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/role/{chatId}");

        var json = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        if (obj == null || !obj.ContainsKey("Role") || obj["Role"] != "Admin")
        {
            await bot.SendMessage(chatId, "❌ Шумо иҷозат надоред.");
            return;
        }

        await bot.SendMessage(chatId, "Admin Panel",
            replyMarkup: AdminButtons.GetAdminKeyboard());
    }
}