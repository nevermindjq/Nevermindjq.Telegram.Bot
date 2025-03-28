using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered;

public abstract class Callback : Command {
	public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.CallbackQuery is { });
}