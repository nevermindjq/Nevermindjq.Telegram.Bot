using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public interface IInformationCommand : IInjectedCommand {
	public ParseMode ParseMode { get; set; }

	public Task ProcessAsync(Update update);
	public string BuildText();
	public InlineKeyboardMarkup BuildMarkup();

	public long GetUserId(Update update);
}