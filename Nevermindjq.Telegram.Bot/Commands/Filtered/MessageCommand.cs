using Microsoft.EntityFrameworkCore;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered {
	public abstract class MessageCommand(string command_name, DbContext? context = null) : Command(context) {
		public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.Message is { } message && message.Text == command_name);

		protected override long GetUserId(Update update) => update.Message.From.Id;
	}
}