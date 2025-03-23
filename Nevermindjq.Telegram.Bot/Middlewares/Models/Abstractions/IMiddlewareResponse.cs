using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

public interface IMiddlewareResponse {
	public bool IsSuccess { get; init; }
	public Exception? Exception { get; init; }
	public Type? Redirect { get; }
}