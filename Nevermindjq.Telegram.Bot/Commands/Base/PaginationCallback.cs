using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Nevermindjq.Telegram.Bot.Extensions;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Base;

public abstract class PaginationCallback(string callback) : Callback, IPaginationCallback {
	public abstract InlineKeyboardButton BackButton { get; set; }

	public abstract Task<string> Text(Update update, Range range);

	public abstract Task<List<List<InlineKeyboardButton>>> Markup(Update update, Range range);

	public async Task ExecuteAsync(Update update, int per_page, int max) {
		var page = int.TryParse(update.CallbackQuery.Data.After(), out var result) ? result : 0;

		// Build markup
		var rest = max % per_page;
		var max_int = (max - rest) / per_page;
		var range = new Range(page * per_page, page * per_page + per_page);
		var markup = new InlineKeyboardMarkup(await Markup(update, range));

		if (page > 0 && page + 1 < max_int / per_page + 1) {
			markup.AddNewRow(
				InlineKeyboardButton.WithCallbackData("\u2b05\ufe0f", $"{callback} {page-1}"),
				InlineKeyboardButton.WithCallbackData("\u27a1\ufe0f", $"{callback} {page+1}")
			);
		}
		else {
			if (page + 1 < max_int / per_page + 1) {
				markup.AddNewRow(InlineKeyboardButton.WithCallbackData("\u27a1\ufe0f", $"{callback} {page+1}"));
			}

			if (page > 0) {
				markup.AddNewRow(InlineKeyboardButton.WithCallbackData("\u2b05\ufe0f", $"{callback} {page-1}"));
			}
		}

		markup.AddNewRow(BackButton);

		// Send message
		await Bot.EditMessageText(
			update.CallbackQuery.From.Id,
			update.CallbackQuery.Message.MessageId,
			await Text(update, range),
			replyMarkup: markup
		);
	}
}