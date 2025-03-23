using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services {
	public class UpdateDispatcher(IServiceScopeFactory factory) {
		public async Task Dispatch(Update update) {
			if (GetTrigger(update) is not { } trigger) {
				return;
			}

			using var scope = factory.CreateScope();

			// Get command
			if (scope.ServiceProvider.GetKeyedService<ICommand>($"{nameof(Update)} {trigger}") is not { } command) {
				return;
			}

			// Execute middlewares
			var interfaces = command.GetType().GetInterfaces();

			foreach (var interface_type in interfaces) {
				var middleware_type = typeof(ICommandMiddleware<>)
					.MakeGenericType(interface_type);

				if (scope.ServiceProvider.GetService(middleware_type) is { } middleware&&
					!await (Task<bool>)middleware_type.GetMethod(nameof(ICommandMiddleware<ICommand>.HandleAsync))!
													  .Invoke(middleware, new object[] { update, command })!) {
					return;
				}
			}

			// Execute command
			await command.OnHandleAsync(update);
		}

		protected string? GetTrigger(Update update) {
			return update switch {
				{ Message: not null } => update.Message.Text!.UpTo(0, " "),
				{ CallbackQuery: not null } => update.CallbackQuery.Data!.UpTo(0, " "),
				_ => null
			};
		}
	}
}