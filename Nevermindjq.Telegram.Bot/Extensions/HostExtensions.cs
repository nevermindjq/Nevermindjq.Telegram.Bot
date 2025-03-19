using Microsoft.Extensions.DependencyInjection;
using Nevermindjq.Models.Services.States.Abstractions;
using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.Services.Hosted;
using Nevermindjq.Telegram.Bot.States;

namespace Nevermindjq.Telegram.Bot.Extensions {
	public static class HostExtensions {
		public static void AddTelegramBot(this IServiceCollection services, Action<FileOptions> configure) {
			services.AddSingleton<BotStateRepository>(_ => {
				var options = new FileOptions();

				configure(options);

				return new BotStateRepository(options);
			});

			services.AddSingleton<IState<BotState>, BotStateRepository>(services => services.GetRequiredService<BotStateRepository>());
			services.AddHostedService<BotStateRepository>(services => services.GetRequiredService<BotStateRepository>());
			services.AddHostedService<Listener>();
		}
	}
}