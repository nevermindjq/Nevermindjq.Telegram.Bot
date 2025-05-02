using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Commands.Abstractions;

public interface ICommand {
	public Task OnHandleAsync(Update update);
	public Task ExecuteAsync(Update update);
	public Task<bool> CanExecuteAsync(Update update);
}

public interface IInjectedCommand : ICommand {
	public IUserContextAsync? ContextAsync { get; set; }
	public IUpdateMediator<long> Mediator { get; set; }
	public ITelegramBotClient Bot { get; set; }
}