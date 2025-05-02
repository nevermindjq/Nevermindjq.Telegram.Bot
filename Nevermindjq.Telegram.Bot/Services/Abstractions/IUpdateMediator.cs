using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUpdateMediator<TKey> where TKey : notnull {
	public void AddNext<TCommand>(TKey key) where TCommand : ICommand;

	public Type? GetNext(TKey key);
}