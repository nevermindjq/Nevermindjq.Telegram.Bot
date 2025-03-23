using System.Reflection;

using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Database.Middlewares;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

using Telegram.Bot;

using Test_Host.Data;

// Build
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<UsersDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Users")), ServiceLifetime.Singleton);

builder.Services.AddCommand("/start", (services, _) => new AnswerCommand {
	Text = _ => "Some Text",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddCommand("/info", (services, _) => new AnswerCommand {
	Text = _ => "Some Info",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddScoped<ICommandMiddleware<IAuthenticatedUser>, AuthenticationMiddleware>(x => new AuthenticationMiddleware(x.GetRequiredService<UsersDbContext>()));

builder.Services.AddTelegramBot(Assembly.GetExecutingAssembly(), builder.Configuration["Bot:Token"]!);

// Run
builder.Build().Run();