namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUserContextAsync {
	public Task<bool> SetAsync<T>(long key, T value, uint timeout = 1) where T : class?;
	public Task<T?> GetAsync<T>(long key, bool remove = true) where T : class?;
}