using System.Reflection;

using Bot.Data;

using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Extensions;

using Telegram.Bot;

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

builder.Services.AddTelegramBot(Assembly.GetExecutingAssembly(), builder.Configuration["Bot:Token"]!);

// Run
builder.Build().Run();