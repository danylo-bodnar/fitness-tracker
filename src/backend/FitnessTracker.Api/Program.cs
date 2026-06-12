using FitnessTracker.Api.Parsers;
using FitnessTracker.Api.Telegram;
using FitnessTracker.Application.Common.Interfaces;
using FitnessTracker.Application.Workouts.Handlers;
using FitnessTracker.Domain.Interfaces;
using FitnessTracker.Infrastructure.Persistence;
using FitnessTracker.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.Load();

var token = Environment.GetEnvironmentVariable("BOT_TOKEN")
    ?? throw new InvalidOperationException("BOT_TOKEN not set");

var connection = Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
    ?? throw new InvalidOperationException("DATABASE_CONNECTION not set");

builder.Services.AddDbContext<WriteDbContext>(o => o.UseNpgsql(connection));
builder.Services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<LogWorkoutHandler>());

builder.Services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));

builder.Services.AddSingleton<IWorkoutParser, WorkoutTextParser>();
builder.Services.AddSingleton<IUpdateHandler, WorkoutUpdateHandler>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();

app.MapGet("/health", () => "ok");

if (!app.Environment.IsDevelopment())
{
    app.MapPost("/bot", async (
        Update update,
        ITelegramBotClient bot,
        IUpdateHandler handler,
        CancellationToken ct) =>
    {
        await handler.HandleUpdateAsync(bot, update, ct);
    });
}

app.Run();
