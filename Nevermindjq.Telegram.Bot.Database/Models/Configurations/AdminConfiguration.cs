using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Configurations.Abstractions;
using Nevermindjq.Telegram.Bot.Database.UI;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.Models.Configurations;

public sealed class AdminConfiguration : ICommandConfiguration {
	// Public
	public bool Enabled { get; set; }

	public string Text { get; set; } = "\ud83d\udee0\ufe0f *Панель администратора*";
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;
	public IEnumerable<IEnumerable<InlineKeyboardButton>>? Markup { get; set; }

	// Internal
	internal Type? AdminType { get; set; }

	//
	public void SetAdminType<TUser, TRole>()
		where TUser : IUser<TUser, TRole>
		where TRole : IRole<TUser, TRole> {
		AdminType = typeof(Admin<TUser, TRole>);
	}
}