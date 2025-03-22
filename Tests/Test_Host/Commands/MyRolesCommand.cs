using Bot.Data;

using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Test_Host.Commands {
	public class MyRolesCommand(ITelegramBotClient bot, UsersDbContext context) : MessageCommand("/roles", context) {
		public override Task ExecuteAsync(Update update) => bot.SendMessage(User.Id, string.Join('\n', User.Roles.Select(role => role.Name)));
	}
}