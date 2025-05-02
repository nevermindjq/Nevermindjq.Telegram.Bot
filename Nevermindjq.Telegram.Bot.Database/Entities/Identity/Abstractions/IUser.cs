using Nevermindjq.Models.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;

public interface IUser<TUser, TRole> : IEntity<long>
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public List<TRole> Roles { get; set; }
}