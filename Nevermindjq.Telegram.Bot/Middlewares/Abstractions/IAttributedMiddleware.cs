using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

namespace Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

public interface IAttributedMiddleware<in TAttribute, in TCommand>
	where TAttribute : Attribute
	where TCommand : ICommand {
	public Task<IMiddlewareResponse?> HandleAsync(Update update, TAttribute attribute, TCommand command);
}