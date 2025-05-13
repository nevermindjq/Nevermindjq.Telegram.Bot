using Microsoft.AspNetCore.Authorization;

using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Base;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Configurations;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.UI;

[Path("/admin"), Path("admin"), Authorize("Admin")]
public class Admin<TUser, TRole>(AdminConfiguration configuration) : InformationCommand, IAuthenticated<TUser, TRole>
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole> {
	public TUser User { get; set; }

	public override string BuildText(Update update) {
		ParseMode = configuration.ParseMode;

		return configuration.Text;
	}

	public override InlineKeyboardMarkup BuildMarkup(Update update) {
		var markup = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>> {
			new() {
				InlineKeyboardButton.WithCallbackData("\ud83d\udee1\ufe0f Роли", "admin:roles"),
				InlineKeyboardButton.WithCallbackData("\u2709\ufe0f Рассылка", "broadcast"),
			},
			new() {
				InlineKeyboardButton.WithCallbackData("\u2699\ufe0f Настройки", "admin:settings")
			}
		});

		foreach (var row in configuration.Markup) {
			markup.AddNewRow(row.ToArray());
		}

		return markup;
	}
}