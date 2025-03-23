using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Database.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Middlewares;

public class AuthenticationMiddleware(DbContext context) : ICommandMiddleware<IAuthenticatedUser> {
	public async Task<IMiddlewareResponse> HandleAsync(Update update, IAuthenticatedUser command) {
		var user = await context.Set<User>().Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == command.GetUserId(update));

		if (user is null) {
			var response = new MiddlewareResponse {
				IsSuccess = false,
				Exception = new Exception("User not found"),
			};

			response.SetRedirect<IRegisterCommand>();

			return response;
		}

		command.User = user;

		return new MiddlewareResponse();

	}
}