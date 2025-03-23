using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUpdateMediator<TKey> where TKey : notnull {
	public bool AddNext<TCommand>(TKey key) where TCommand : ICommand;
	public bool AddNext<TCommand>(Update update) where TCommand : ICommand;

	public Type? GetNext(TKey key);
}