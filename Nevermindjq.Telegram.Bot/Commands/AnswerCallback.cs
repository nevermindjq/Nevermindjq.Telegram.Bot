using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands;

public sealed class AnswerCallback(ITelegramBotClient bot) : Callback {
	public required Func<Update, string> Text { get; set; }
	public InlineKeyboardMarkup? Markup { get; set; }
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;

	public override Task ExecuteAsync(Update update) {
		return bot.EditMessageText(
			update.CallbackQuery.From.Id,
			update.CallbackQuery.Message.MessageId,
			Text(update),
			parseMode: ParseMode,
			replyMarkup: Markup
		);
	}
}

public sealed class SAnswerCallback(Func<Update, Task> action) : Callback {
	public override Task ExecuteAsync(Update update) => action.Invoke(update);
}