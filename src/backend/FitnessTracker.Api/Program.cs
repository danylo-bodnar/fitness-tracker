using System.Text;
using System.Text.Json;
using FitnessTracker.Api.Middleware;
using FitnessTracker.Api.Parsers;
using FitnessTracker.Api.Telegram;
using FitnessTracker.Application;
using FitnessTracker.Application.Common.Options;
using FitnessTracker.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

builder.AddServiceDefaults();

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Jwt configuration is missing");

        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddAuthorization();

var allowedOrigins = new[]
{
    "http://localhost:5173",
    "https://fitness-tracker-pink-nu.vercel.app",
};

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<TelegramOptions>(
    builder.Configuration.GetSection(TelegramOptions.SectionName));
builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection(AppOptions.SectionName));

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["BOT_TOKEN"]!));

builder.Services.AddSingleton<IWorkoutParser, WorkoutTextParser>();
builder.Services.AddSingleton<WorkoutUpdateHandler>();
builder.Services.AddSingleton<TelegramLoginCallbackHandler>();
builder.Services.AddSingleton<IUpdateHandler, CompositeUpdateHandler>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

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
