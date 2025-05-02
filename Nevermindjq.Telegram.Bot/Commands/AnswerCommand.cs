using Nevermindjq.Telegram.Bot.Commands.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands;

public sealed class AnswerCommand(ITelegramBotClient bot) : Command {
	public required Func<Update, string> Text { get; set; }
	public ReplyMarkup? Markup { get; set; }
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;

	public override Task ExecuteAsync(Update update) {
		return bot.SendMessage(update.Message.Chat.Id, Text(update), ParseMode, replyMarkup: Markup);
	}
}

public sealed class SAnswerCommand(Func<Update, Task> action) : Command {
	public override Task ExecuteAsync(Update update) => action.Invoke(update);
}