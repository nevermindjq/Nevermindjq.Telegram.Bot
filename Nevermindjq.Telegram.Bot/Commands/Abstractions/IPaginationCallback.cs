using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public interface IPaginationCallback {
	public InlineKeyboardButton BackButton { get; }

	public Task<string> Text(Update update, Range range);
	public Task<List<List<InlineKeyboardButton>>> Markup(Update update, Range range);

	public Task ExecuteAsync(Update update, int per_page, int max);
}