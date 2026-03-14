using System.Net.Http.Json;
using System.Text.Json;
using Domain.DTO.User;
using Domain.Entities;
using Infrastructure.Services;
using Telegram.Bot;

namespace Infrastructure.HelperMethods;

public class UserHelperMethods(ITelegramBotClient bot,
    HttpClient httpClient)
{
    
    
    
    public async Task DeleteUser(long chatId, string text)
    {
        await httpClient.DeleteAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/{text}"
        );
        await bot.SendMessage(chatId, "User Deleted");
        TelegramService.UserState[chatId] = "main";
    }
    
    public async Task GetUser(long chatId, string text)
    {
        if (!int.TryParse(text, out var id))
        {
            await bot.SendMessage(chatId, "❌ Invalid Id");
            return;
        }

        var response = await httpClient.GetAsync(
            $"https://kenny-sunnier-russel.ngrok-free.dev/api/user/{id}");

        if (!response.IsSuccessStatusCode)
        {
            await bot.SendMessage(chatId, "❌ User not found");
            return;
        }

        var user = await response.Content.ReadFromJsonAsync<GetUserDto>();

        if (user == null)
            return;
        
        var message =
            $"👤 User Info:\n" +
            $"🆔 Id: {user.Id}\n" +
            $"💬 TelegramId: {user.TelegramId}\n" +
            $"📛 Username: {user.Username}\n" +
            $"📱 Phone: {user.Phone}\n" +
            $"🏠 Address: {user.Address}\n" +
            $"🎂 Age: {user.Age}\n" +
            $"🔑 Role: {user.Role}";

        await bot.SendMessage(chatId, message);
    }
    
    
    public async Task<List<User>> GetUsers()
    {
        var response = await httpClient.GetAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/user");
        
        if (!response.IsSuccessStatusCode)
            return new List<User>();
        
        var json = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<List<User>>(json)
               ?? new List<User>();
    }
}