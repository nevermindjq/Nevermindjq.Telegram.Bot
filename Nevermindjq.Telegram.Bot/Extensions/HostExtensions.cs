using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Models.States.Abstractions;
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.Services.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Hosted;
using Nevermindjq.Telegram.Bot.Services.UserContexts;
using Nevermindjq.Telegram.Bot.States;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Extensions;

public static class HostExtensions {
	public static void AddTelegramBot(this IServiceCollection services, string token, FileOptions? options = null, bool use_caching_user_context = true) {
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

		if (use_caching_user_context) {
			services.AddTransient<IUserContextAsync, UserContextInCache>();
		}

		#region Register commands

		// By reflection
		services.AddCommands(Assembly.GetEntryAssembly()!);

		// Custom
		services.AddCommand("msg:delete", (_, _) => new DeleteMessage());

		#endregion

		services.AddHostedService<Listener>();
	}

	#region Commands

	public static bool AddCommand<TCommand>(this IServiceCollection services, string key, Func<IServiceProvider, object?, TCommand> factory, string? description = null, int order = 0) where TCommand : class, ICommand {
		try {
			services.AddKeyedTransient<ICommand, TCommand>($"{nameof(Update)} {key}", factory);
		}
		catch {
			return false;
		}

		if (description is not null) {
			ClientExtensions.i_CommandAttributes.Add(new(key) {
				Description = description,
				InformationOrder = order
			});
		}

		return true;
	}

	public static void AddCommand(this IServiceCollection services, (Type type, IEnumerable<PathAttribute> attrs) info) {
		var (type, attrs) = info;

		switch (type.GetCustomAttribute<LifetimeAttribute>(true)?.Lifetime ?? ServiceLifetime.Transient) {
			case ServiceLifetime.Singleton:
				services.AddSingleton(type);

				foreach (var attr in attrs) {
					services.AddKeyedSingleton(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type)!);
				}
			break;
			case ServiceLifetime.Scoped:
				services.AddScoped(type);

				foreach (var attr in attrs) {
					services.AddKeyedScoped(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type)!);
				}
			break;
			case ServiceLifetime.Transient:
				services.AddTransient(type);

				foreach (var attr in attrs) {
					services.AddKeyedTransient(typeof(ICommand), $"{nameof(Update)} {attr.Path}", (x, _) => x.GetService(type)!);
				}
			break;
		}
	}

	public static void AddCommand<TCommand>(this IServiceCollection services) where TCommand : ICommand {
		services.AddCommand(AssemblyExtensions.CommandInfo<TCommand>());
	}

	public static void AddCommands(this IServiceCollection services, Assembly assembly) {
		var commands = assembly.Commands();

		foreach (var cmd in commands) {
			services.AddCommand(cmd);
		}
	}

	#endregion

	#region Switcher

	public static void AddSwitcher(this IServiceCollection services, string key, Func<IServiceProvider, object?, ICommand> factory) {
		services.AddKeyedTransient($"{nameof(Update)} {key}:btn", factory);
		services.AddKeyedTransient($"{nameof(Update)} {key}:off", (x, _) => x.GetRequiredKeyedService<ICommand>($"{nameof(Update)} {key}:btn"));
		services.AddKeyedTransient($"{nameof(Update)} {key}:on", (x, _) => x.GetRequiredKeyedService<ICommand>($"{nameof(Update)} {key}:btn"));
	}

	public static Switcher<T>? GetSwitcher<T>(this IServiceProvider services, string key) where T : class {
		var switcher = (Switcher<T>?)services.GetKeyedService<ICommand>($"{nameof(Update)} {key}:btn");

		if (switcher is not null) {
			switcher.Key = key;
		}

		return switcher;
	}

	#endregion
}