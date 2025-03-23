using Bot.Data;

using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Test_Host.Commands {
	[Path("/roles"), Lifetime(ServiceLifetime.Scoped)]
	public class MyRolesCommand(ITelegramBotClient bot, UsersDbContext context) : MessageCommand(context) {
		public override Task ExecuteAsync(Update update) => bot.SendMessage(User.Id, string.Join('\n', User.Roles.Select(role => role.Name)));
	}
}