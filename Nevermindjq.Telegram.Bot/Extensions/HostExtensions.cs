using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
	/// <summary>
	/// Registers and configures the core Telegram Bot services and dependencies.
	/// </summary>
	/// <param name="services">The service collection to add the bot services to.</param>
	/// <param name="token">The Telegram Bot API token.</param>
	/// <param name="options">
	/// Optional file options for bot state storage. If not provided, defaults to a directory named "Bot" in the application's base directory.
	/// </param>
	/// <param name="use_caching_user_context">
	/// If true, uses a caching implementation for user context; otherwise, uses the default implementation.
	/// </param>
	public static void AddTelegramBot(this IServiceCollection services, string token, FileOptions? options = null, bool use_caching_user_context = true) {
		// Set Defaults
		options ??= new FileOptions(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Bot"), "options");

		services.AddSingleton<ITelegramBotClient>(_ => new TelegramBotClient(token));

		// Register State Repository
		if (options is not null) {
			services.TryAddSingleton(_ => new BotStateRepository((FileOptions)options));
			services.TryAddSingleton<IState<BotState>>(services => services.GetRequiredService<BotStateRepository>());
			services.AddHostedService(services => services.GetRequiredService<BotStateRepository>());
		}

		// Register Update Dispatcher
		services.TryAddSingleton<IUpdateDispatcher, UpdateDispatcher>();
		services.TryAddSingleton<IUpdateMediator<long>, UpdateMediator>();
		services.TryAddTransient<IUpdateResolver, UpdateResolver>();
		services.TryAddTransient<ICommandsResolver, CommandsResolver>();

		//
		if (use_caching_user_context) {
			services.TryAddTransient<IUserContextAsync, UserContextInCache>();
		}

		// Register Commands
		services.AddCommands(Assembly.GetEntryAssembly()!);

		services.AddCommand("msg:delete", (_, _) => new DeleteMessage());

		//
		services.AddHostedService<Listener>();
	}

	#region Commands

	/// <summary>
	/// Registers a command with a specified key and factory method, with optional description and order.
	/// </summary>
	/// <typeparam name="TCommand">The command type implementing ICommand.</typeparam>
	/// <param name="services">The service collection.</param>
	/// <param name="key">The command key.</param>
	/// <param name="factory">The factory function to create the command instance.</param>
	/// <param name="description">Optional command description.</param>
	/// <param name="order">Optional order for information display.</param>
	/// <returns>True if the command was registered successfully; otherwise, false.</returns>
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

	/// <summary>
	/// Registers a command type and its associated path attributes in the service collection.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="info">A tuple containing the command type and its path attributes.</param>
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

	/// <summary>
	/// Registers a command of type <typeparamref name="TCommand"/> in the service collection.
	/// </summary>
	/// <typeparam name="TCommand">The command type implementing ICommand.</typeparam>
	/// <param name="services">The service collection.</param>
	public static void AddCommand<TCommand>(this IServiceCollection services) where TCommand : ICommand {
		services.AddCommand(AssemblyExtensions.CommandInfo<TCommand>());
	}

	/// <summary>
	/// Registers all commands found in the specified assembly in the service collection.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="assembly">The assembly to scan for commands.</param>
	public static void AddCommands(this IServiceCollection services, Assembly assembly) {
		var commands = assembly.Commands();

		foreach (var cmd in commands) {
			services.AddCommand(cmd);
		}
	}

	#endregion

	#region Switcher

	/// <summary>
	/// Registers a switcher command with a specified key and factory method in the service collection.
	/// </summary>
	/// <param name="services">The service collection.</param>
	/// <param name="key">The switcher key used to identify the command.</param>
	/// <param name="factory">The factory function to create the switcher command instance.</param>
	public static void AddSwitcher<T>(this IServiceCollection services, string key, Func<IServiceProvider, object?, Switcher<T>> factory) where T : class {
		services.AddKeyedTransient<ICommand>($"{nameof(Update)} {key}:btn", factory);
		services.AddKeyedTransient<ICommand>($"{nameof(Update)} {key}:off", (x, _) => x.GetRequiredKeyedService<ICommand>($"{nameof(Update)} {key}:btn"));
		services.AddKeyedTransient<ICommand>($"{nameof(Update)} {key}:on", (x, _) => x.GetRequiredKeyedService<ICommand>($"{nameof(Update)} {key}:btn"));
	}

	/// <summary>
	/// Retrieves a switcher of type <typeparamref name="T"/> by key from the service provider.
	/// </summary>
	/// <typeparam name="T">The switcher type.</typeparam>
	/// <param name="services">The service provider.</param>
	/// <param name="key">The switcher key used to identify the command.</param>
	/// <returns>The switcher instance if found; otherwise, null.</returns>
	public static Switcher<T>? GetSwitcher<T>(this IServiceProvider services, string key) where T : class {
		var switcher = (Switcher<T>?)services.GetKeyedService<ICommand>($"{nameof(Update)} {key}:btn");

		if (switcher is not null) {
			switcher.Key = key;
		}

		return switcher;
	}

	#endregion
}
