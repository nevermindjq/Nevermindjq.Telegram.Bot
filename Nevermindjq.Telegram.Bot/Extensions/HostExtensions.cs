using Microsoft.Extensions.DependencyInjection;
using Nevermindjq.Models.Services.States.Abstractions;
using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.Services.Hosted;
using Nevermindjq.Telegram.Bot.States;
using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Extensions {
	public static class HostExtensions {
		public static void AddTelegramBot(this IServiceCollection services, string token, FileOptions options) {
			services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));

			services.AddSingleton<BotStateRepository>(_ => new BotStateRepository(options));
			services.AddSingleton<IState<BotState>, BotStateRepository>(services => services.GetRequiredService<BotStateRepository>());
			services.AddHostedService<BotStateRepository>(services => services.GetRequiredService<BotStateRepository>());

			services.AddHostedService<Listener>();
		}
	}
}