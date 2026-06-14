using FitnessTracker.Api.Parsers;
using FitnessTracker.Api.Telegram;
using FitnessTracker.Application;
using FitnessTracker.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["BOT_TOKEN"]!));

builder.Services.AddSingleton<IWorkoutParser, WorkoutTextParser>();
builder.Services.AddSingleton<IUpdateHandler, WorkoutUpdateHandler>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/health", () => "ok");

app.MapPost("/bot", async (
    Update update,
    ITelegramBotClient bot,
    IUpdateHandler handler,
    CancellationToken ct) =>
{
    await handler.HandleUpdateAsync(bot, update, ct);
});

app.Run();
