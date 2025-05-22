using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Models.Abstractions;

public interface IAuthenticated<TUser, TRole> : ICommand
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public TUser User { get; set; }
}