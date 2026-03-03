using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(builder.Configuration
        .GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUserService1, UserService1>();
builder.Services.AddScoped<IOrderService1, OrderService1>();

builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var token = builder.Configuration.GetSection("BotConfiguration")
        .GetValue<string>("Token");

    if (string.IsNullOrEmpty(token))
        throw new Exception("Bot Token is missing!");

    return new TelegramBotClient(token);
});


builder.Services.AddControllers();
builder.Logging.AddConsole();

var app = builder.Build();

app.UseExceptionHandler("/error");
app.UseDeveloperExceptionPage();

app.MapControllers();

app.Run();
