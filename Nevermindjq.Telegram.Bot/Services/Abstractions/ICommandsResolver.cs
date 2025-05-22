using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services.Abstractions;

public interface ICommandsResolver {
	public Task<ICommand?> ByMediatorAsync(IServiceProvider services, Update update);
	public Task<ICommand?> ByTriggerAsync(IServiceProvider services, Update update);
}