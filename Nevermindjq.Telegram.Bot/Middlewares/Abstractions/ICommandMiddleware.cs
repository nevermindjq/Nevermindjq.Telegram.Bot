using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

namespace Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

public interface ICommandMiddleware<TModel> where TModel : class {
	public Task<IMiddlewareResponse> HandleAsync(Update update, TModel command);
}