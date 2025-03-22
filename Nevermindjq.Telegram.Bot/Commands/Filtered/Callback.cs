using Microsoft.EntityFrameworkCore;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered {
	public abstract class Callback(DbContext? context = null) : Command(context) {
		public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.CallbackQuery is { });

		protected override long GetUserId(Update update) => update.CallbackQuery.From.Id;
	}
}