using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface IUpdateMediator<TKey> where TKey : notnull {
	public void AddNext<TCommand>(TKey key, string? command_key = null, uint? count_delete = null) where TCommand : ICommand;

	public (Type command_type, string? command_key, uint delete_count)? GetNext(TKey key);
}