using Nevermindjq.Telegram.Bot.Database.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Entities.Identity;

public class Role : IEntity<string> {
	public string Name { get; set; } = null!;

	public List<User> Users { get; set; } = [];

	// IEntity
	public string Id { get; set; } = Guid.NewGuid().ToString();
}