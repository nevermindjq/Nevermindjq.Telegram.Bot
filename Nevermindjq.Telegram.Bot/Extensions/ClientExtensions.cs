using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Extensions;

public static class ClientExtensions {
	internal static readonly List<PathAttribute> i_CommandAttributes = new();

	public static Task RegisterCommandsAsync(this ITelegramBotClient client, Assembly assembly) {
		i_CommandAttributes.AddRange(
			assembly.Commands()
					.Select(x => x.attr)
		);

		return client.SetMyCommands(
			i_CommandAttributes
				.OrderBy(x => x.InformationOrder)
				.Where(x => x.Path[0] == '/' && x.Description is not null)
				.Select(x => new BotCommand(x.Path, x.Description!))
		);
	}
}