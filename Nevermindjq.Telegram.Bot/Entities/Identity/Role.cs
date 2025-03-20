using Nevermindjq.Telegram.Bot.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Entities.Identity {
	public class Role : IEntity<string> {
		// IEntity
		public string Id { get; set; } = Guid.NewGuid().ToString();

		public string Name { get; set; } = null!;
		public List<User> Users { get; set; } = [];
	}
}