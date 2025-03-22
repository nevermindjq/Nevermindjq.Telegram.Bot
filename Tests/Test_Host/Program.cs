using System.Reflection;

using Bot.Data;

using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Extensions;

using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;
using Telegram.Bot;

using FileOptions = Nevermindjq.Models.Services.States.Options.FileOptions;

// Build
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<UsersDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Users")), ServiceLifetime.Singleton);

builder.Services.AddSlimMessageBus(config => {
	config.WithProviderMemory().AutoRegisterCommands(Assembly.GetExecutingAssembly());
});

builder.Services.AddCommand("/start", (services, _) => new AnswerCommand {
	Text = _ => "Some Text",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddCommand("/info", (services, _) => new AnswerCommand {
	Text = _ => "Some Info",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddTelegramBot(builder.Configuration["Bot:Token"]!, new FileOptions("bin/net8.0/Debug/Bot", "state"));

// Run
builder.Build().Run();