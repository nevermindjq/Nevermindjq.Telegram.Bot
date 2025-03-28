using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered;

public abstract class MessageCommand : Command {
	public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.Message is { });
}