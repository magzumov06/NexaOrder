using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.User;
using Domain.CallBack;
using Domain.DTO.Role;
using Domain.Enums;
using Infrastructure.Buttons;
using Infrastructure.Handlers;
using Infrastructure.HelperMethods;
using Infrastructure.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    HttpClient httpClient,
    OrderHelperMethods order,
    ProductHelperMethods product,
    UserHelperMethods user) : ITelegramService
{
    private readonly AdminCallbackHandler _adminCallback = new(bot);
    private readonly MainCallbackHandndler _mainCallback = new(bot);
    private readonly OrderCallbackHandler _orderCallback = new(bot, order);
    private readonly ProductCallbackHandler _productCallback = new(bot, product);
    private readonly UserCallbackHandler _userCallback = new(bot, user);

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

        switch (text)
        {
            case "/start":
                await StartRegistration(chatId);
                return;
            case "/admin":
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
            
            case "waiting_age":
                await HandleAge(chatId, text);
                break;

            case "waiting_address":
                await HandleAddress(chatId, text);
                break;

            case "create_order":
                await order.CreateOrder(chatId, text);
                break;
            
            case "update_product":
                await product.UpdateProduct(chatId, text);
                break;

            case "waiting_delete_order":
                await order.DeleteOrder(chatId, text);
                break;
            case "waiting_delete_user":
                await user.DeleteUser(chatId, text);
                break;

            case "waiting_get_order":
                await order.GetOrder(chatId, text);
                break;

            case "waiting_get_product":
                await product.GetProduct(chatId, text);
                break;

            case "waiting_get_user":
                await user.GetUser(chatId, text);
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

    private async Task HandleAge(long chatId, string age)
    {
        if (!int.TryParse(age, out var parsedAge))
        {
            await bot.SendMessage(chatId, "❌ Лутфан рақам ворид кунед.");
            return;
        }

        TempUsers[chatId].Age = parsedAge;
        UserState[chatId] = "waiting_address";

        await bot.SendMessage(chatId, "Шумо аз кадом шаҳр ҳастед?");
    }

    private async Task HandleUsername(long chatId, string username)
    {
        TempUsers[chatId].Username = username;

        UserState[chatId] = "waiting_age";

        await bot.SendMessage(chatId, "Шумо чандсола ҳастед?");
    }

    private async Task HandleAddress(long chatId, string text)
    {
        var dto = TempUsers[chatId];

        dto.Address = text;
        dto.TelegramId = chatId;  


        var response = await httpClient.PostAsJsonAsync<CreateUserDto>(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/user",
            dto);
        
        TempUsers.Remove(chatId);
        UserState[chatId] = "main";

        await bot.SendMessage(chatId,
            "✅ Registration completed",
            replyMarkup: MainButtons.GetMainKeyboard());
    }
    


   

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message == null)
            return;

        var chatId = callback.Message.Chat.Id;
        var messageId = callback.Message.MessageId;
        var data = callback.Data;

        switch (data)
        {
            case BotCallbacks.Back:
                UserState[chatId] = "main";

                await bot.EditMessageText(
                    chatId,
                    messageId,
                    "🏠 Main Menu",
                    replyMarkup: MainButtons.GetMainKeyboard());

                break;
        }

        await _adminCallback.Handle(chatId, messageId, data);
        await _orderCallback.Handle(chatId, data);
        await _productCallback.Handle(chatId, data);
        await _userCallback.Handle(chatId, data);
        await _mainCallback.Handle(chatId,messageId, data);

        await bot.AnswerCallbackQuery(callback.Id);
    }
    
    private async Task ShowAdminPanel(long chatId)
    {
        try
        {
            var response = await httpClient.GetAsync(
                $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/role/{chatId}");

            if (!response.IsSuccessStatusCode)
            {
                await bot.SendMessage(chatId, "❌ Access denied");
                return;
            }

            var role = await response.Content.ReadFromJsonAsync<RoleDto>();

            if (role == null || role.Role != UserRole.Admin)
            {
                await bot.SendMessage(chatId, "❌ Шумо Admin нестед.");
                return;
            }

            await bot.SendMessage(chatId,
                "🛠 Admin Panel",
                replyMarkup: AdminButtons.GetAdminKeyboard());
        }
        catch (Exception)
        {
            await bot.SendMessage(chatId, "❌ Server error");
        }
    }
}