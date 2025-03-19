using Microsoft.Extensions.Hosting;
using Serilog;
using SlimMessageBus;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nevermindjq.Telegram.Bot.Services.Hosted {
	public class Listener(ITelegramBotClient bot, IMessageBus bus, BotStateRepository state) : IHostedService, IUpdateHandler {
		public async Task StartAsync(CancellationToken cancellationToken) {
			if (!await bot.TestApi(cancellationToken)){
				throw new Exception("Test API failed");
			}

			bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, new ReceiverOptions {
				AllowedUpdates = [
					UpdateType.Message,
					UpdateType.CallbackQuery
				],
				DropPendingUpdates = state.Model.HandlePendingUpdates,
				Limit = state.Model.Limit,
				Offset = state.Model.Offset,
			}, cancellationToken: cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
			return bus.Publish(update, cancellationToken: cancellationToken);
		}

		public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken) {
			Log.Error(exception, "Error was occured while handling update");

			return Task.CompletedTask;
		}
	}
}