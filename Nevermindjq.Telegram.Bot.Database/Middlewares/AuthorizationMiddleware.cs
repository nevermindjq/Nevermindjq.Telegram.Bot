using Microsoft.AspNetCore.Authorization;

using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

using Serilog;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Middlewares;

public class AuthorizationMiddleware<TUser, TRole, TRedirect> : IAttributedMiddleware<AuthorizeAttribute, IAuthenticated<TUser, TRole>>
	where TRedirect : ICommand
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public Task<IMiddlewareResponse?> HandleAsync(Update update, AuthorizeAttribute attribute, IAuthenticated<TUser, TRole> command) {
		if (attribute.Policy?.Split(',') is not { } attr_roles) {
			Log.Warning("Roles not set for command {CommandName}", command.GetType().Name);

			return Task.FromResult<IMiddlewareResponse?>(MiddlewareResponse.CreateException<TRedirect>($"Roles not set for command {command.GetType().Name}"));
		}

 		if (command.User is not { Roles.Count: >= 1 }) {
			Log.Warning("User {UserId} is not authorized", command.User.Id);

			return Task.FromResult<IMiddlewareResponse?>(MiddlewareResponse.CreateException<TRedirect>("User not found or doesn't have roles"));
		}

		if (command.User.Roles.All(x => !attr_roles.Contains(x.Name))) {
			Log.Warning("User {UserId} is not authorized", command.User.Id);

			return Task.FromResult<IMiddlewareResponse?>(MiddlewareResponse.CreateException<TRedirect>("User is not authorized"));
		}

		return Task.FromResult<IMiddlewareResponse?>(new MiddlewareResponse());
	}
}