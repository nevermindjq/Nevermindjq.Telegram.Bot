namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUpdateDispatcher {
	public Task Dispatch(Update update);
}