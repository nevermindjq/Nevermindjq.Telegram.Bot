using Nevermindjq.Telegram.Bot.Database.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Entities.Identity;

public class User : IEntity<long> {
	public List<Role> Roles { get; set; } = [];

	// IEntity
	public long Id { get; set; }
}