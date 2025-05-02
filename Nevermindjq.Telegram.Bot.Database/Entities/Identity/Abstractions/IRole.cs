using Nevermindjq.Models.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;

public interface IRole<TUser, TRole> : IEntity<string>
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public string Name { get; set; }
	public List<TUser> Users { get; set; }
}