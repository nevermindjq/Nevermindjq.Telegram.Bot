using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Models.Abstractions;

public interface IAuthenticated<TUser, TRole> : ICommand
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public TUser User { get; set; }

	public long GetUserId(Update update) {
		switch (update) {
			case { Message: not null }:
				return update.Message.From.Id;
			case { CallbackQuery: not null }:
				return update.CallbackQuery.From.Id;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
}