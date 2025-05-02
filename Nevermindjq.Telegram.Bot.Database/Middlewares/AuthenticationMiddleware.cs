using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Middlewares;

public class AuthenticationMiddleware<TUser, TRole, TRedirect>(IRepository<TUser> users) : ICommandMiddleware<IAuthenticated<TUser, TRole>>
	where TRedirect : ICommand
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public async Task<IMiddlewareResponse> HandleAsync(Update update, IAuthenticated<TUser, TRole> command) {
		var user = await users.GetAsync(command.GetUserId(update));

		if (user is null) {
			return MiddlewareResponse.CreateException<TRedirect>("User not found");
		}

		command.User = user;

		return new MiddlewareResponse();

	}
}