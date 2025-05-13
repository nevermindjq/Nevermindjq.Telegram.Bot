using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Newtonsoft.Json;

using Serilog;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Services;

internal class UpdateDispatcher(IServiceScopeFactory factory, ITelegramBotClient bot) : IUpdateDispatcher, IUpdateMediator<long> {
	private readonly Dictionary<long, (Type command_type, string? command_key, uint delete_count)?> m_commands = new();

	public async Task Dispatch(Update update) {
		await using var scope = factory.CreateAsyncScope();
		var services = scope.ServiceProvider;

		// Get id

		long id = 0;

		switch (update) {
			case { Message: not null }:
				id = update.Message.From.Id;
				break;
			case { CallbackQuery: not null }:
				id = update.CallbackQuery.From.Id;
				break;
		}

		// Get by mediator
		ICommand? command = null;

		if (GetNext(id) is { } next && update is { Message: not null }) {
			if (next.command_key is not null) {
				var found = services.GetKeyedServices<ICommand>($"{nameof(Update)} {next.command_key}");

				if (next.command_type == typeof(ICommand) && found.Count() == 1) {
					command = found.FirstOrDefault();
				}
				else {
					command = found.FirstOrDefault(x => x.GetType() == next.command_type);
				}
			}
			else {
				command = (ICommand?)services.GetService(next.command_type);
			}

			await bot.DeleteMessages(id, new long[next.delete_count].Select((_, i) => {
				switch (update) {
					case { Message: not null }:
						return update.Message.MessageId - i;
					case { CallbackQuery: not null }:
						return update.CallbackQuery.Message.MessageId - i;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}));
		}

		// Get by trigger
		if (GetTrigger(update) is not { } trigger) {
			Log.Warning("Trigger not found for update\n{0}", JsonConvert.SerializeObject(update));

			return;
		}

		command ??= services.GetKeyedService<ICommand>($"{nameof(Update)} {trigger}");

		if (command is null) {
			Log.Warning("Command with key: '{0} {1}' is not found.", nameof(Update), trigger);

			return;
		}

		// Execute typed middlewares
		if (!await ExecuteMiddlewares(
			services, update, command,
			@interface => {
				return typeof(ICommandMiddleware<>).MakeGenericType(@interface);
			}, update, command)) {
			return;
		}

		// Execute attributed middlewares
		foreach (var attr in command.GetType().GetCustomAttributes(true)) {
			if (!await ExecuteMiddlewares(
				services, update, command,
				@interface => {
					return typeof(IAttributedMiddleware<,>)
						.MakeGenericType(attr.GetType(), @interface);
				}, update, attr, command)) {
				return;
			}
		}

		await ExecuteCommand(services, update, command);
	}

	#region Commands

	protected async Task ExecuteCommand(IServiceProvider services, Update update, ICommand command) {
		if (command is IInjectedCommand injected) {
			injected.Bot = services.GetRequiredService<ITelegramBotClient>();
			injected.Mediator = services.GetRequiredService<IUpdateMediator<long>>();
			injected.ContextAsync = services.GetService<IUserContextAsync>();
		}

#if DEBUG
		Log.Debug("Start executing command {0}.", command.GetType().Name);
#endif

		await command.OnHandleAsync(update);

#if DEBUG
		Log.Debug("Command {0} was successfully executed.", command.GetType().Name);
#endif
	}

	#endregion

	#region Middlewares

	protected Task<IMiddlewareResponse> InvokeMiddleware(object? middleware, params object?[]? args) {
		return (Task<IMiddlewareResponse>)middleware!.GetType().GetMethod("HandleAsync")!.Invoke(middleware, args)!;
	}

	protected async Task<bool> ExecuteMiddleware(IServiceProvider services, Update update, IMiddlewareResponse? response, Type middleware_type, Type command_type) {
		if (response is null) {
			Log.Warning("Null response of executing middleware {MiddlewareName}", middleware_type.Name);

			return false;
		}

		if (!response.IsSuccess) {
			Log.Error(response.Exception, "Exception while command execution.");

			if (response.Redirect is not null) {
#if DEBUG
				Log.Debug("Redirecting to {0}.", response.Redirect.Name);
#endif

				await ExecuteCommand(services, update, (ICommand)services.GetRequiredService(response.Redirect));
			}

			return false;
		}

#if DEBUG
		Log.Debug("{0} for command {1} was successfully executed.", middleware_type.Name, command_type.Name);
#endif

		return true;
	}

	protected async Task<bool> ExecuteMiddlewares(IServiceProvider services, Update update, object command, Func<Type, Type> create_middleware_type, params object?[]? args) {
		var command_type = command.GetType();

		foreach (var @interface in command.GetType().GetInterfaces()) {
			var middleware_type = create_middleware_type(@interface);

			foreach (var middleware in services.GetServices(middleware_type)) {
				if (middleware is null) {
					continue;
				}

#if DEBUG
				Log.Debug("Start executing {0} for command {1}.", middleware_type.Name, command_type.Name);
#endif

				if (!await ExecuteMiddleware(services, update, await InvokeMiddleware(middleware, args), middleware_type, command.GetType())) {
					return false;
				}
			}
		}

		return true;
	}

	#endregion

	protected string? GetTrigger(Update update) {
		return update switch {
			{ Message.Text: not null }       => update.Message.Text.UpTo(),
			{ CallbackQuery.Data: not null } => update.CallbackQuery.Data.UpTo(),
			_                                => null
		};
	}

	#region IUpdateMediator

	public void AddNext<TCommand>(long key, string? command_key = null, uint? delete_count = null) where TCommand : ICommand {
#if DEBUG
		Log.Debug("Added new command: {0}. Key: {1}", typeof(TCommand).Name, key);
#endif
		if (!m_commands.TryAdd(key, (typeof(TCommand), command_key, delete_count ?? 2))) {
			m_commands[key] = (typeof(TCommand), command_key, delete_count ?? 2);
		}
	}

	public (Type command_type, string? command_key, uint delete_count)? GetNext(long key) {
		if (m_commands.GetValueOrDefault(key) is not { } tuple) {
			return null;
		}

		m_commands.Remove(key);

#if DEBUG
		Log.Debug("Got & Removed command: {0}. Key: {1}", tuple.command_type.Name, key);
#endif

		return tuple;
	}

	#endregion
}