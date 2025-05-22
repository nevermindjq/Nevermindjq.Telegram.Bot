using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Newtonsoft.Json;

using Serilog;

using Telegram.Bot;

namespace Nevermindjq.Telegram.Bot.Services;

public class CommandsResolver(
	IUpdateMediator<long> mediator,
	IUpdateResolver resolver,
	ITelegramBotClient bot
	) : ICommandsResolver {
	public async Task<ICommand?> ByMediatorAsync(IServiceProvider services, Update update) {
		var user_id = resolver.UserId(update);

		//
		if (mediator.GetNext(user_id) is not { } next || update is not { Message: not null }) {
			return null;
		}

		//
		ICommand? command = null;

		if (next.command_key is not null) {
			var found = services.GetKeyedServices<ICommand>($"{nameof(Update)} {next.command_key}");

			if (next.command_type == typeof(ICommand) && found.Count() == 1) {
				command = found.FirstOrDefault();
			}
			else {
				command = found.FirstOrDefault(x => x.GetType() == next.command_type);
			}
		}
		else {
			command = (ICommand?)services.GetService(next.command_type);
		}

		//
		if (command is not null) {
			await bot.DeleteMessages(user_id, new int[next.delete_count].Select((_, i) => resolver.MessageId(update) - i));
		}

		//
		return command;
	}

	public Task<ICommand?> ByTriggerAsync(IServiceProvider services, Update update) {
		if (resolver.Payload<string>(update) is not { } trigger) {
			Log.Warning("Trigger not found for update\n{0}", JsonConvert.SerializeObject(update));

			return Task.FromResult<ICommand?>(null);
		}

		if (services.GetKeyedService<ICommand>($"{nameof(Update)} {trigger}") is { } command) {
			return Task.FromResult<ICommand?>(command);
		}

		Log.Warning("Command with key: '{0} {1}' is not found.", nameof(Update), trigger);

		return Task.FromResult<ICommand?>(null);
	}
}

