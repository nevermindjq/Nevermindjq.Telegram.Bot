using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public interface IInformationCommandBase : IInjectedCommand {
	public ParseMode ParseMode { get; set; }

	public long GetUserId(Update update);
}

public interface IInformationCommand : IInjectedCommand {
	public Task ProcessAsync(Update update);
	public string BuildText(Update update);
	public InlineKeyboardMarkup BuildMarkup(Update update);
}

public interface IInformationCommandAsync : IInformationCommand {
	public Task<string> BuildTextAsync(Update update);
	public Task<InlineKeyboardMarkup> BuildMarkupAsync(Update update);
}