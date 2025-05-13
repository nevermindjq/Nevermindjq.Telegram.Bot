using Nevermindjq.Models.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;

public interface IBroadcastOptions : IEntity<string> {
	public long UserId { get; set; }

	public bool ToUsers { get; set; }
	public bool ToAdmins { get; set; }
}