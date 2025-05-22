using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Serilog;

namespace Nevermindjq.Telegram.Bot.Services;

public class UpdateMediator : IUpdateMediator<long> {
	private readonly Dictionary<long, (Type command_type, string? command_key, uint delete_count)?> commands = new();

	public void AddNext<TCommand>(long key, string? command_key = null, uint? delete_count = null) where TCommand : ICommand {
#if DEBUG
		Log.Debug("Added new command: {0}. Key: {1}", typeof(TCommand).Name, key);
#endif

		if (!commands.TryAdd(key, (typeof(TCommand), command_key, delete_count ?? 2))) {
			commands[key] = (typeof(TCommand), command_key, delete_count ?? 2);
		}
	}

	public (Type command_type, string? command_key, uint delete_count)? GetNext(long key) {
		if (commands.GetValueOrDefault(key) is not { } tuple) {
			return null;
		}

		commands.Remove(key);

#if DEBUG
		Log.Debug("Got & Removed command: {0}. Key: {1}", tuple.command_type.Name, key);
#endif

		return tuple;
	}
}