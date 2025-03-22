using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;

namespace Nevermindjq.Telegram.Bot.Services {
	public class UpdateDispatcher(IServiceScopeFactory factory) {
		public Task Dispatch(Update update) {
			if (GetTrigger(update) is { } trigger) {
				using (var scope = factory.CreateScope()) {
					if (scope.ServiceProvider.GetKeyedService<ICommand>($"{nameof(Update)} {trigger}") is { } consumer) {
						return consumer.OnHandle(update, default);
					}
				}
			}

			return Task.CompletedTask;
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