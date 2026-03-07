using System.Net.Http.Json;
using Domain.DTO.Order;
using Domain.Entities;
using Infrastructure.Interfaces;
using Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services;

public class TelegramService(
    ITelegramBotClient bot,
    IOrderService orderService,
    HttpClient httpClient)
    : ITelegramService
{
    private static readonly Dictionary<long, string> UserState = new();

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
            UserState[chatId] = "main";

            await bot.SendMessage(
                chatId,
                "👋 Welcome to NexaOrderBot",
                replyMarkup: GetMainKeyboard()
            );
            return;
        }

        if (!UserState.TryGetValue(chatId, out var state))
            return;

        // CREATE ORDER
        if (state == "create_order")
        {
            var parts = text.Split(" ");

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int productId) ||
                !int.TryParse(parts[1], out int quantity))
            {
                await bot.SendMessage(chatId, "❌ Format: ProductId Quantity\nExample: 1 2");
                return;
            }

            var dto = new CreateOrderDto
            {
                ProductId = productId,
                Quantity = quantity,
                Address = "Telegram Order",
                PaymentMethod = PaymentMethod.Cash
            };

            var result = await orderService.CreateOrderAsync(dto);

            UserState[chatId] = "main";

            await bot.SendMessage(chatId, result, replyMarkup: GetMainKeyboard());
        }

        // CREATE PRODUCT
        else if (state == "create_product")
        {
            var parts = text.Split(" ");

            if (parts.Length != 3)
            {
                await bot.SendMessage(chatId, "❌ Format:\nName Price Quantity\nExample:\nApple 10 5");
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

            UserState[chatId] = "main";

            await bot.SendMessage(chatId, "✅ Product Created");
        }

        // DELETE PRODUCT
        else if (state == "waiting_delete_product")
        {
            if (!int.TryParse(text, out var productId))
            {
                await bot.SendMessage(chatId, "❌ Invalid Product ID");
                return;
            }

            await httpClient.DeleteAsync(
                $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{productId}");

            UserState[chatId] = "main";

            await bot.SendMessage(chatId, "🗑 Product Deleted");
        }

        // GET PRODUCT
        else if (state == "waiting_get_product")
        {
            if (!int.TryParse(text, out var productId))
            {
                await bot.SendMessage(chatId, "❌ Invalid ID");
                return;
            }

            var response = await httpClient.GetAsync(
                $"https://kenny-sunnier-russel.ngrok-free.dev/api/products/{productId}");

            var json = await response.Content.ReadAsStringAsync();

            await bot.SendMessage(chatId, $"📦 Product:\n{json}");

            UserState[chatId] = "main";
        }

        // GET ORDER
        else if (state == "waiting_get_order")
        {
            if (!int.TryParse(text, out var orderId))
            {
                await bot.SendMessage(chatId, "❌ Invalid Order ID");
                return;
            }

            var response = await httpClient.GetAsync(
                $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{orderId}");

            var json = await response.Content.ReadAsStringAsync();

            await bot.SendMessage(chatId, $"📦 Order:\n{json}");

            UserState[chatId] = "main";
        }

        // DELETE ORDER
        else if (state == "waiting_delete_order")
        {
            if (!int.TryParse(text, out var orderId))
            {
                await bot.SendMessage(chatId, "❌ Invalid Order ID");
                return;
            }

            await httpClient.DeleteAsync(
                $"https://kenny-sunnier-russel.ngrok-free.dev/api/orders/{orderId}");

            UserState[chatId] = "main";

            await bot.SendMessage(chatId, "🗑 Order Deleted");
        }
    }

    private async Task HandleCallbackAsync(CallbackQuery callback)
    {
        if (callback.Message == null)
            return;

        var chatId = callback.Message.Chat.Id;
        var data = callback.Data;

        switch (data)
        {
            case "create_order":

                UserState[chatId] = "create_order";

                await bot.SendMessage(chatId,
                    "📦 Send ProductId and Quantity\nFormat:\n1 2");
                break;

            case "products":

                var products = await GetProductsFromApi();

                var text = new StringBuilder("🛍 Products:\n\n");

                foreach (var p in products)
                {
                    text.AppendLine($"Id: {p.Id}");
                    text.AppendLine($"Name: {p.Name}");
                    text.AppendLine($"Price: {p.Price}\n");
                }

                await bot.SendMessage(chatId, text.ToString());
                break;

            case "admin_panel":

                await bot.SendMessage(chatId,
                    "👑 Admin Panel",
                    replyMarkup: GetAdminKeyboard());
                break;

            case "admin_products":

                await bot.SendMessage(chatId,
                    "📦 Product Management",
                    replyMarkup: GetProductAdminKeyboard());
                break;

            case "admin_orders":

                await bot.SendMessage(chatId,
                    "📑 Order Management",
                    replyMarkup: GetOrderAdminKeyboard());
                break;

            case "create_product":

                UserState[chatId] = "create_product";

                await bot.SendMessage(chatId,
                    "➕ Send Product:\nName Price Quantity\nExample:\nApple 10 5");
                break;

            case "delete_product":

                UserState[chatId] = "waiting_delete_product";

                await bot.SendMessage(chatId, "🗑 Send Product ID:");
                break;

            case "get_product":

                UserState[chatId] = "waiting_get_product";

                await bot.SendMessage(chatId, "🔎 Send Product ID:");
                break;

            case "get_order":

                UserState[chatId] = "waiting_get_order";

                await bot.SendMessage(chatId, "🔎 Send Order ID:");
                break;

            case "delete_order":

                UserState[chatId] = "waiting_delete_order";

                await bot.SendMessage(chatId, "🗑 Send Order ID:");
                break;

            case "go_back":

                UserState[chatId] = "main";

                await bot.SendMessage(chatId,
                    "🔙 Main Menu",
                    replyMarkup: GetMainKeyboard());
                break;
        }

        await bot.AnswerCallbackQuery(callback.Id);
    }

    private async Task<List<Product>> GetProductsFromApi()
    {
        var response = await httpClient.GetAsync(
            "https://kenny-sunnier-russel.ngrok-free.dev/api/products");

        if (!response.IsSuccessStatusCode)
            return new List<Product>();

        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<Product>>(json)
               ?? new List<Product>();
    }

    private InlineKeyboardMarkup GetMainKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🛒 Create Order", "create_order")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🛍 Products", "products")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("👑 Admin Panel", "admin_panel")
            }
        });
    }

    private InlineKeyboardMarkup GetAdminKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📦 Products", "admin_products")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📑 Orders", "admin_orders")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Back", "go_back")
            }
        });
    }

    private InlineKeyboardMarkup GetProductAdminKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("➕ Create", "create_product")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔎 Get By Id", "get_product")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🗑 Delete", "delete_product")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Back", "admin_panel")
            }
        });
    }

    private InlineKeyboardMarkup GetOrderAdminKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔎 Get Order By Id", "get_order")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🗑 Delete Order", "delete_order")
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Back", "admin_panel")
            }
        });
    }
}