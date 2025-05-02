using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Middlewares.Models.Abstractions;

namespace Nevermindjq.Telegram.Bot.Middlewares.Models;

public class MiddlewareResponse : IMiddlewareResponse {
	public bool IsSuccess { get; init; } = true;
	public Exception? Exception { get; init; } = null;
	public Type? Redirect { get; protected set; } = null;

	public void SetRedirect<TCommand>() where TCommand : ICommand {
		Redirect = typeof(TCommand);
	}

	public static MiddlewareResponse CreateException<TRedirect>(string message) where TRedirect : ICommand {
		var response = new MiddlewareResponse {
			IsSuccess = false,
			Exception = new Exception(message),
		};

		response.SetRedirect<TRedirect>();

		return response;
	}
}