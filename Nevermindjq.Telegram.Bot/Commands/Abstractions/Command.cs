using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Serilog;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public abstract class Command : IInjectedCommand {
	public IUserContextAsync ContextAsync { get; set; }
	public IUpdateMediator<long> Mediator { get; set; }
	public ITelegramBotClient Bot { get; set; }

	public async Task OnHandleAsync(Update update) {
		var type = this.GetType();

		try {
			await ExecuteAsync(update);
		}
		catch (Exception e) {
			Log.Error(e, "Error while executing. Command: {0}.", type.Name);
			throw;
		}
	}

	// ICommand
	public abstract Task ExecuteAsync(Update update);

	public virtual Task<bool> CanExecuteAsync(Update update) => Task.FromResult(true);
}