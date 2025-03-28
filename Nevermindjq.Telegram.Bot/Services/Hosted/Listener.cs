using System.Reflection;

using Microsoft.Extensions.Hosting;
using Nevermindjq.Models.Services.States.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;
using Nevermindjq.Telegram.Bot.States;
using Serilog;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace Nevermindjq.Telegram.Bot.Services.Hosted;

public class Listener(ITelegramBotClient bot, IState<BotState> state, IUpdateDispatcher dispatcher) : IHostedService, IUpdateHandler {
	public async Task StartAsync(CancellationToken cancellationToken) {
		Log.Information("Listener starting");

		if (!await bot.TestApi(cancellationToken)){
			throw new Exception("Test API failed");
		}

		await bot.RegisterCommandsAsync(Assembly.GetEntryAssembly()!);

		bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new ReceiverOptions {
			AllowedUpdates = [
				UpdateType.Message,
				UpdateType.CallbackQuery
			],
			DropPendingUpdates = state.Model.HandlePendingUpdates,
			Limit = state.Model.Limit,
			Offset = state.Model.Offset,
		}, cancellationToken: cancellationToken);

		Log.Information("Listener started");
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		return Task.CompletedTask;
	}

	public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
		return dispatcher.Dispatch(update);
	}

	public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken) {
		return Task.Run(() => Log.Error(exception, "Error was occured while handling update"), cancellationToken);
	}
}