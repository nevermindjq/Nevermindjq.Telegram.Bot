using Nevermindjq.Telegram.Bot.Commands.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Base;

public abstract class InformationCommand : Command, IInformationCommand {
	public ParseMode ParseMode { get; set; } = ParseMode.Html;

	public abstract Task ProcessAsync(Update update);

	public abstract string BuildText();

	public abstract InlineKeyboardMarkup BuildMarkup();

	public override async Task ExecuteAsync(Update update) {
		await ProcessAsync(update);

		switch (update) {
			case { Message: not null }:
				await Bot.SendMessage(update.Message.From.Id, BuildText(), ParseMode, replyMarkup: BuildMarkup());
				break;
			case { CallbackQuery: not null }:
				await Bot.EditMessageText(
					update.CallbackQuery.From.Id,
					update.CallbackQuery.Message.MessageId,
					BuildText(), ParseMode,
					replyMarkup: BuildMarkup()
				);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(update));
		}
	}

	public override Task<bool> CanExecuteAsync(Update update) =>
		Task.FromResult(update is { Message: not null } or { CallbackQuery: not null});

	public long GetUserId(Update update) {
		switch (update) {
			case { Message: not null }:
				return update.Message.From.Id;
			case { CallbackQuery: not null }:
				return update.CallbackQuery.From.Id;
			default:
				throw new ArgumentOutOfRangeException(nameof(update));
		}
	}
}