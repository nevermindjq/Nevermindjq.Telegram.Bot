using Nevermindjq.Telegram.Bot.Commands.Abstractions;

using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Commands.Abstractions;

public interface IRegisterCommand : ICommand {
	public Task<bool> RegisterAsync(Update update);
}