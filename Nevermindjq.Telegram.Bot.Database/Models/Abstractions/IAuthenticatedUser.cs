using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Models.Abstractions;

public interface IAuthenticatedUser {
	public User User { get; set; }

	public long GetUserId(Update update);
}