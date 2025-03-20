using Nevermindjq.Telegram.Bot.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Entities.Identity {
	public class User : IEntity<long> {
		// IEntity
		public long Id { get; set; }

		public List<Role> Roles { get; set; } = [];
	}
}