using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Example.Commands;

[Path("/start")]
public class StartCommand : MessageCommand {
	public override Task ExecuteAsync(Update update) {
		return Bot.SendMessage(update.Message.From.Id, $"Hello {update.Message.From.Username}! You send: {update.Message.Text}");
	}
}