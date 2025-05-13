using MemoryPack;

using Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.Models.Context;

[MemoryPackable]
public partial class BroadcastRequest<TOptions> where TOptions : IBroadcastOptions {
	public string Text { get; set; }
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;
	public byte[]? ImageStream { get; set; }

	[MemoryPackAllowSerialize]
	public List<List<InlineKeyboardButton>>? Markup { get; set; }

	[MemoryPackAllowSerialize]
	public TOptions Options { get; set; }
}