using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.Models.Configurations.Abstractions;

public interface ICommandConfiguration {
	public string Text { get; set; }
	public ParseMode ParseMode { get; set; }
	public IEnumerable<IEnumerable<InlineKeyboardButton>>? Markup { get; set; }
}