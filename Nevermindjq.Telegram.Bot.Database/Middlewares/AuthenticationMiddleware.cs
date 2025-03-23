using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Middlewares;

public class AuthenticationMiddleware(DbContext context) : ICommandMiddleware<IAuthenticatedUser> {
	public async Task<bool> HandleAsync(Update update, IAuthenticatedUser command) {
		var user = await context.Set<User>().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == command.GetUserId(update));

		if (user is null) {
			return false;
		}

		command.User = user;

		return true;

	}
}