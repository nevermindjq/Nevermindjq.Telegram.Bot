using SlimMessageBus;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public interface ICommand : IConsumer<Update> {
		public Task ExecuteAsync(Update update);
		public Task<bool> CanExecuteAsync(Update update);
	}
}