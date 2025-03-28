using MemoryPack;

namespace Nevermindjq.Telegram.Bot.States;

[MemoryPackable]
public partial class BotState {
	public bool HandlePendingUpdates { get; init; } = false;
	public int Limit { get; init; } = 10;
	public int Offset { get; init; } = 0;
}