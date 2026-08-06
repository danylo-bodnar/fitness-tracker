using System.Text;
using System.Text.Json;
using Npgsql;
using FitnessTracker.Api.Middleware;
using FitnessTracker.Api.Telegram;
using FitnessTracker.Application;
using FitnessTracker.Application.Common.Options;
using FitnessTracker.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;
using Telegram.Bot.Polling;
using FitnessTracker.Api.Exceptions;
using Serilog;
using FitnessTracker.Api.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithMachineName()
        .WriteTo.Console();
});

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

var connectionString = builder.Configuration.GetConnectionString("fitness-tracker")
    ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
    ?? throw new InvalidOperationException("No database connection string found");

builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<TelegramOptions>(
    builder.Configuration.GetSection(TelegramOptions.SectionName));
builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection(AppOptions.SectionName));

builder.Services.AddSingleton<ITelegramBotClient>(_ =>
    new TelegramBotClient(builder.Configuration["BOT_TOKEN"]!));

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<WorkoutStateService>();
builder.Services.AddSingleton<WorkoutConversationHandler>();
builder.Services.AddSingleton<WorkoutUpdateHandler>();
builder.Services.AddSingleton<TelegramLoginCallbackHandler>();
builder.Services.AddSingleton<IUpdateHandler, CompositeUpdateHandler>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCorrelationId();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseRateLimiting();

app.UseAuthorization();

app.MapControllers();

app.MapDefaultEndpoints();

app.MapGet("/health", () => "ok");

app.Run();

public partial class Program { }
