using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Newtonsoft.Json;

using Serilog;

namespace Nevermindjq.Telegram.Bot.Services;

public class UpdateDispatcher(IServiceScopeFactory factory) : IUpdateDispatcher, IUpdateMediator<long> {
	private readonly Dictionary<long, Type> m_commands = new();

	public async Task Dispatch(Update update) {
		if (GetTrigger(update) is not { } trigger) {
			Log.Warning("Trigger not found for update\n{0}", JsonConvert.SerializeObject(update));

			return;
		}

		using var scope = factory.CreateScope();

		// Get command
		ICommand? command = null;

		if (update is { Message.From.Id: var id } && GetNext(id) is { } next) {
			command = (ICommand?)scope.ServiceProvider.GetService(next);
		}

		command ??= scope.ServiceProvider.GetKeyedService<ICommand>($"{nameof(Update)} {trigger}");

		if (command is null) {
			Log.Warning("Command with key: '{0} {1}' is not found.", nameof(Update), trigger);

			return;
		}

		// Execute middlewares
		var interfaces = command.GetType().GetInterfaces();

		foreach (var interface_type in interfaces) {
			var middleware_type = typeof(ICommandMiddleware<>)
				.MakeGenericType(interface_type);

			if (scope.ServiceProvider.GetService(middleware_type) is { } middleware) {
#if DEBUG
				Log.Debug("Start executing {0} for command {1}.", middleware_type.Name, command.GetType().Name);
#endif

				var response = await (Task<IMiddlewareResponse>)middleware_type
																.GetMethod(nameof(ICommandMiddleware<ICommand>.HandleAsync))!
																.Invoke(middleware, new object[] { update, command })!;

				if (!response.IsSuccess) {
					Log.Error(response.Exception, "Exception while command execution.");

					if (response.Redirect is not null) {
#if DEBUG
						Log.Debug("Redirecting to {0}.", response.Redirect.Name);
#endif

						await ((ICommand)scope.ServiceProvider.GetRequiredService(response.Redirect)).OnHandleAsync(update);
					}

					return;
				}

#if DEBUG
				else {
					Log.Debug("{0} for command {1} was successfully executed.", middleware_type.Name, command.GetType().Name);
				}
#endif
			}
		}

		// Execute command
#if DEBUG
		Log.Debug("Start executing command {0}.", command.GetType().Name);
#endif

		await command.OnHandleAsync(update);

#if DEBUG
		Log.Debug("Command {0} was successfully executed.", command.GetType().Name);
#endif
	}

	protected string? GetTrigger(Update update) {
		return update switch {
			{ Message: not null }       => update.Message.Text!.UpTo(0, " "),
			{ CallbackQuery: not null } => update.CallbackQuery.Data!.UpTo(0, " "),
			_                           => null
		};
	}

	// IUpdateMediator
	public bool AddNext<TCommand>(long key) where TCommand : ICommand {
#if DEBUG
		Log.Debug("Added new command: {0}. Key: {1}", typeof(TCommand).Name, key);
#endif
		return m_commands.TryAdd(key, typeof(TCommand));
	}

	public bool AddNext<TCommand>(Update update) where TCommand : ICommand => AddNext<TCommand>(update.Message.From.Id);

	public Type? GetNext(long key) {
		if (m_commands.GetValueOrDefault(key) is not { } type) {
			return null;
		}

#if DEBUG
		Log.Debug("Got & Removed command: {0}. Key: {1}", type.Name, key);
#endif

		return type;
	}
}