using Microsoft.AspNetCore.Components;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands {
	public sealed class AnswerCommand : Command {
		// Services
		[Inject] public required ITelegramBotClient Bot { get; set; }

		// Properties
		public required string Text { get; set; }
		public ReplyMarkup? Markup { get; set; }
		public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;

		public override Task ExecuteAsync(Update update) {
			return Bot.SendMessage(GetUserId(update), Text, ParseMode, replyMarkup: Markup);
		}

		protected override long GetUserId(Update update) => update.Message.Chat.Id;
	}
}