namespace Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

public interface ICommandMiddleware<TModel> where TModel : class {
	public Task HandleAsync(Update update, TModel command);
}