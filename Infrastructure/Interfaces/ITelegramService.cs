
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace Infrastructure.Interfaces;

public interface ITelegramService
{
    Task HandleUpdateAsync(Update update);
}