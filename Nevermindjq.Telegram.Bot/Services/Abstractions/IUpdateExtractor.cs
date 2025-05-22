namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUpdateResolver {
	public T? Payload<T>(Update update) where T : class;
	public int MessageId(Update update);
	public long UserId(Update update);
}