using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Extensions;

using SlimMessageBus;

namespace Nevermindjq.Telegram.Bot.Services {
	public class UpdateDispatcher(IMessageBus bus, IServiceScopeFactory factory) {
		public Task Dispatch(Update update) {
			if (GetTrigger(update) is { } trigger) {
				using (var scope = factory.CreateScope()) {
					if (scope.ServiceProvider.GetKeyedService<IConsumer<Update>>(trigger) is { } consumer) {
						return consumer.OnHandle(update, default);
					}
				}
			}

			return bus.Publish(update, nameof(Update));
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