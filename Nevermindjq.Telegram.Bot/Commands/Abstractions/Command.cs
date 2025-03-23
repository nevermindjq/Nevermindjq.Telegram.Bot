using Serilog;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public abstract class Command: ICommand {

		public virtual async Task OnHandle(Update update, CancellationToken cancellationToken) {
			if (!await CanExecuteAsync(update)){
				return;
			}

			var type = this.GetType();

			Log.Verbose("Starting. Command {0}.", type.Name);

			try {
				await ExecuteAsync(update);
			}
			catch (Exception e) {
				Log.Error(e, "Error while executing. Command: {0}.", type.Name);
				throw;
			}

			Log.Verbose("Command successfully executed. Command: {0}.", type.Name);
		}

		// ICommand
		public abstract Task ExecuteAsync(Update update);

		public virtual Task<bool> CanExecuteAsync(Update update) => Task.FromResult(true);
	}
}