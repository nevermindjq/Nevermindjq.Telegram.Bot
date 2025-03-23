using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types;

using User = Nevermindjq.Telegram.Bot.Database.Entities.Identity.User;

namespace Test_Host.Commands {
	[Path("/roles"), Lifetime(ServiceLifetime.Scoped)]
	public class MyRolesCommand(ITelegramBotClient bot) : MessageCommand, IAuthenticatedUser {
		public User User { get; set; }

		public override Task ExecuteAsync(Update update) => bot.SendMessage(User.Id, string.Join('\n', User.Roles.Select(role => role.Name)));

		// IAuthenticatedUser
		public long GetUserId(Update update) => update.Message.From.Id;
	}
}