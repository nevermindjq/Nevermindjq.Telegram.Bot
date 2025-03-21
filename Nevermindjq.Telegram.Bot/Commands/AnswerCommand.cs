using Microsoft.AspNetCore.Components;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands {
	public sealed class AnswerCommand : Command {
		// Services
		[Inject] public required ITelegramBotClient Bot { get; set; }

		// Properties
		public required Func<Update, string> Text { get; set; }
		public ReplyMarkup? Markup { get; set; }
		public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;

		public override Task ExecuteAsync(Update update) {
			return Bot.SendMessage(GetUserId(update), Text(update), ParseMode, replyMarkup: Markup);
		}

		protected override long GetUserId(Update update) => update.Message.Chat.Id;
	}
}