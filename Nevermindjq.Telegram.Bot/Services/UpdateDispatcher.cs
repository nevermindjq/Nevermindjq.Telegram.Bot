using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Newtonsoft.Json;

using Serilog;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Services;

internal class UpdateDispatcher(IServiceScopeFactory factory, ICommandsResolver commands) : IUpdateDispatcher {
	public async Task Dispatch(Update update) {
		await using var scope = factory.CreateAsyncScope();
		var services = scope.ServiceProvider;

		// Get command
		if ((await commands.ByMediatorAsync(services, update) ?? await commands.ByTriggerAsync(services, update)) is not { } command) {
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
}