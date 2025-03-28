using Serilog;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public abstract class Command: ICommand {

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