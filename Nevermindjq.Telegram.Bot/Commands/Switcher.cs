using System.Linq.Expressions;

using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands;

public sealed class Switcher<T>(IRepository<T> repository) : Callback
	where T : class {
	public required string Text { get; init; }

	public required Func<IRepository<T>, Update, Task<T>> GetModel { get; init; }

	public required Func<IRepository<T>, T, Task> Toggle { get; init; }

	public override async Task ExecuteAsync(Update update) {
		var current = update.CallbackQuery.Data.Split(':').Last() == "on";

		var markup = update.CallbackQuery
						   .Message.ReplyMarkup.InlineKeyboard
						   .Select(x => x.ToList())
						   .ToList();

		foreach (var row in markup) {
			foreach (var btn in row) {
				if (btn.CallbackData != update.CallbackQuery.Data) {
					continue;
				}

				btn.Text = $"{Text} [{(current ? "\u2705" : "\u274c")}]";
				var data_sub = btn.CallbackData.Substring(0, btn.CallbackData.LastIndexOf(':'));
				btn.CallbackData = $"{data_sub}:{(current ? "off" : "on")}";
			}
		}

		// Update value
		await Toggle.Invoke(repository, await GetModel(repository, update));

		// Update layout
		await Bot.EditMessageReplyMarkup(
			update.CallbackQuery.From.Id,
			update.CallbackQuery.Message.MessageId,
			new InlineKeyboardMarkup(markup)
		);
	}

	//
	internal string Key { get; set; }

	public InlineKeyboardButton Build<TObject>(TObject options, Expression<Func<TObject, bool>> current) {
		var state = current.Compile().Invoke(options);

		return InlineKeyboardButton.WithCallbackData($"{Text} [{(state ? "\u2705" : "\u274c")}]", $"{Key}:{(state ? "off" : "on")}");
	}
}