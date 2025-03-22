using Microsoft.EntityFrameworkCore;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered {
	public abstract class MessageCommand(DbContext? context = null) : Command(context) {
		public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.Message is { });

		protected override long GetUserId(Update update) => update.Message.From.Id;
	}
}