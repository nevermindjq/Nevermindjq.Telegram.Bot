using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Models.Services.States.Abstractions;
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.Services.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Hosted;
using Nevermindjq.Telegram.Bot.States;

using Telegram.Bot;

#nullable disable

namespace Nevermindjq.Telegram.Bot.Extensions {
	public static class HostExtensions {
		public static void AddTelegramBot(this IServiceCollection services, Assembly assembly, string token, FileOptions? options = null) {
			// Set Defaults
			options ??= new FileOptions(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bot"), "options");

			// Register Services
			services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));

			services.AddSingleton(_ => new BotStateRepository((FileOptions)options));
			services.AddSingleton<IState<BotState>, BotStateRepository>(services => services.GetRequiredService<BotStateRepository>());
			services.AddHostedService(services => services.GetRequiredService<BotStateRepository>());

			services.AddSingleton<UpdateDispatcher>();
			services.AddSingleton<IUpdateDispatcher>(x => x.GetRequiredService<UpdateDispatcher>());
			services.AddSingleton<IUpdateMediator<long>>(x => x.GetRequiredService<UpdateDispatcher>());

			services.AddCommands(assembly);

			services.AddHostedService<Listener>();

		}

		public static void AddCommand<TCommand>(this IServiceCollection services, string key, Func<IServiceProvider, object?, TCommand> factory) where TCommand : class, ICommand {
			services.AddKeyedTransient<ICommand, TCommand>($"{nameof(Update)} {key}",  factory);
		}

		private static void AddCommands(this IServiceCollection services, Assembly assembly) {
			var types = assembly.GetTypes()
								.Select(x => (type: x, attr: x.GetCustomAttribute<PathAttribute>()))
								.Where(x => x.attr is not null);

			foreach (var (type, attr) in types) {
				switch (type.GetCustomAttribute<LifetimeAttribute>()?.Lifetime ?? ServiceLifetime.Transient) {
					case ServiceLifetime.Singleton:
						services.AddSingleton(type);
						services.AddKeyedSingleton(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type));
						break;
					case ServiceLifetime.Scoped:
						services.AddScoped(type);
						services.AddKeyedScoped(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type));
						break;
					case ServiceLifetime.Transient:
						services.AddTransient(type);
						services.AddKeyedTransient(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type));
						break;
				}
			}
		}
	}
}