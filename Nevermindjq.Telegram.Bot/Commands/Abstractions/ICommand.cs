namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public interface ICommand {
		public Task OnHandleAsync(Update update);
		public Task ExecuteAsync(Update update);
		public Task<bool> CanExecuteAsync(Update update);
	}
}