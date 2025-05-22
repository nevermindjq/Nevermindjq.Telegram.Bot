using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands;

[Path("msg:delete")]
public class DeleteMessage : Callback {
	public static InlineKeyboardButton Button { get; set; } = InlineKeyboardButton.WithCallbackData("\u274c Удалить", "msg:delete");

	public override Task ExecuteAsync(Update update) {
		Mediator.GetNext(update.CallbackQuery.From.Id);

		return Bot.DeleteMessage(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId);
	}
}