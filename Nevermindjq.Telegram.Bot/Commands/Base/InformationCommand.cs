using Nevermindjq.Telegram.Bot.Commands.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands.Base;

public abstract class InformationCommand : Command, IInformationCommand {
	public ParseMode ParseMode { get; set; } = ParseMode.Html;

	public virtual Task ProcessAsync(Update update) => Task.CompletedTask;

	public abstract string BuildText(Update update);

	public abstract InlineKeyboardMarkup BuildMarkup(Update update);

	public override async Task ExecuteAsync(Update update) {
		await ProcessAsync(update);

		switch (update) {
			case { Message: not null }:
				await Bot.SendMessage(update.Message.From.Id, BuildText(update), ParseMode, replyMarkup: BuildMarkup(update));
				break;
			case { CallbackQuery: not null }:
				await Bot.EditMessageText(
					update.CallbackQuery.From.Id,
					update.CallbackQuery.Message.MessageId,
					BuildText(update), ParseMode,
					replyMarkup: BuildMarkup(update)
				);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(update));
		}
	}

	public override Task<bool> CanExecuteAsync(Update update) =>
		Task.FromResult(update is { Message: not null } or { CallbackQuery: not null});
}

public abstract class InformationCommandAsync : InformationCommand, IInformationCommandAsync {
	public override string BuildText(Update update) => BuildTextAsync(update).Result;
	public abstract Task<string> BuildTextAsync(Update update);

	public override InlineKeyboardMarkup BuildMarkup(Update update) => BuildMarkupAsync(update).Result;
	public abstract Task<InlineKeyboardMarkup> BuildMarkupAsync(Update update);

	public override async Task ExecuteAsync(Update update) {
		await ProcessAsync(update);

		switch (update) {
			case { Message: not null }:
				await Bot.SendMessage(update.Message.From.Id, await BuildTextAsync(update), ParseMode, replyMarkup: await BuildMarkupAsync(update));
			break;
			case { CallbackQuery: not null }:
				await Bot.EditMessageText(
					update.CallbackQuery.From.Id,
					update.CallbackQuery.Message.MessageId,
					await BuildTextAsync(update), ParseMode,
					replyMarkup: await BuildMarkupAsync(update)
				);
			break;
			default:
				throw new ArgumentOutOfRangeException(nameof(update));
		}
	}
}