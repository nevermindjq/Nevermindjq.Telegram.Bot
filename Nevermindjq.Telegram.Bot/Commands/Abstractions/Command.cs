using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public abstract class Command(DbContext? context = null) : ICommand, IAuthenticator {
		public User? User { get; protected set; }

		public async Task OnHandle(Update update, CancellationToken cancellationToken) {
			if (!await CanExecuteAsync(update)){
				return;
			}

			var type = this.GetType();
			var user_id = GetUserId(update);

			Log.Verbose("Starting. Command {0}. User: {1}", type.Name, user_id);
			Log.Verbose("Try to authenticate user. Command: {0}. User: {1}", type.Name, user_id);
			User = await AuthenticateAsync(update);
			Log.Verbose(User == null ? "User unauthenticated. Command: {0}. User: {1}" : "User authenticated. Command: {0}. User: {1}", type.Name, user_id);

			try {
				await ExecuteAsync(update);
			}
			catch (Exception e) {
				Log.Error(e, "Error while executing. Command: {0}. User: {1}", type.Name, user_id);
				throw;
			}

			Log.Verbose("Command successfully executed. Command: {0}. User: {1}", type.Name, user_id);
		}

		// ICommand
		public abstract Task ExecuteAsync(Update update);

		public virtual Task<bool> CanExecuteAsync(Update update) => Task.FromResult(true);

		// IAuthenticator
		public Task<User?> AuthenticateAsync(Update update) {
			if (!IsValidDatabase(context)) {
				return Task.FromResult<User?>(null);
			}

			return context!.Set<User>()
			               .Include(x => x.Roles)
			               .FirstOrDefaultAsync(x => x.Id == GetUserId(update));
		}

		// Protected
		protected abstract long GetUserId(Update update);

		protected virtual bool IsValidDatabase(DbContext? context) {
			return context is not null && context.Model.FindEntityType(typeof(User)) != null;
		}
	}
}