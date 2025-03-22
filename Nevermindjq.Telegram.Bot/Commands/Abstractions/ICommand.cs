namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public interface ICommand {
		public Task OnHandle(Update update, CancellationToken cancellationToken);
		public Task ExecuteAsync(Update update);
		public Task<bool> CanExecuteAsync(Update update);
	}
}