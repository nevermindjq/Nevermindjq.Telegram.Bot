using System.Reflection;
using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Extensions;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using FileOptions=Nevermindjq.Models.Services.States.Options.FileOptions;

// Build
var builder = Host.CreateApplicationBuilder(args);

#region Configuration

builder.Environment.EnvironmentName = Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Production";
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, false);
builder.Configuration.AddUserSecrets(Assembly.GetAssembly(typeof(Program))!, true, false);
builder.Configuration.AddCommandLine(args);

#endregion

builder.Services.AddSlimMessageBus(config => {
	config.WithProviderMemory()
		  .AutoDeclareCommands();
});

builder.Services.AddKeyedSingleton<IConsumer<Update>, AnswerCommand>("/start", (services, _) => new AnswerCommand {
	Text = "Some Text",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddKeyedSingleton<IConsumer<Update>, AnswerCommand>("/info", (services, _) => new AnswerCommand {
	Text = "Some Info",
	Bot = services.GetRequiredService<ITelegramBotClient>()
});

builder.Services.AddTelegramBot(builder.Configuration["Bot:Token"]!, new FileOptions("bin/net8.0/Debug/Bot", "state"));

// Run
builder.Build().Run();