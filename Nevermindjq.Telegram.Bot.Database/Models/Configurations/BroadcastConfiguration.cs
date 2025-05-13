using System.Linq.Expressions;

using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Configurations.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;
using Nevermindjq.Telegram.Bot.Database.UI;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.Models.Configurations;

public sealed class BroadcastSwitcherConfiguration<T> {
	public required string Role { get; set; }

	public required string Text { get; init; }

	public required Func<IRepository<T>, Update, Task<T>> GetModel { get; init; }

	public required Func<IRepository<T>, T, Task> Toggle { get; init; }

	public required Expression<Func<T, bool>> State { get; set; }
}

public sealed class BroadcastConfiguration<TOptions> : ICommandConfiguration where TOptions : class, IBroadcastOptions {

	// Public
	public bool Enabled { get; set; }
	public bool UseTest { get; set; }

	public string Text { get; set; } = "\ud83d\udce2 *Рассылка*";
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;
	public IEnumerable<IEnumerable<InlineKeyboardButton>>? Markup { get; set; }

	public List<BroadcastSwitcherConfiguration<TOptions>> Switchers { get; } = new() {
		new() {
			Role = "Users",
			Text = "\ud83d\udc65 Пользователям",
			GetModel = (repository, update) => {
				return repository
					   .FindAsync(x => x.UserId == update.CallbackQuery.From.Id)
					   .FirstAsync()
					   .AsTask();
			},
			Toggle = async (repository, options) => {
				options.ToUsers = !options.ToUsers;

				await repository.UpdateAsync(options);
			},
			State = x => x.ToUsers
		},
		new() {
			Role = "Admins",
			Text = "\ud83d\udc51 Администраторам",
			GetModel = (repository, update) => {
				return repository
					   .FindAsync(x => x.UserId == update.CallbackQuery.From.Id)
					   .FirstAsync()
					   .AsTask();
			},
			Toggle = async (repository, options) => {
				options.ToAdmins = !options.ToAdmins;

				await repository.UpdateAsync(options);
			},
			State = x => x.ToAdmins
		}
	};

	// Internal
	internal Type? BroadcastType { get; set; }
	internal Type? BroadcastTestType { get; set; }
	internal Type? BroadcastSendConfirmType { get; set; }

	//
	public void SetBroadcastType<TUser, TRole, TOptions>()
		where TUser : IUser<TUser, TRole>
		where TRole : IRole<TUser, TRole>
		where TOptions : class, IBroadcastOptions {
		BroadcastType = typeof(BroadcastPanel<TUser, TRole, TOptions>);
		BroadcastTestType = typeof(BroadcastTest<TUser, TRole, TOptions>);
		BroadcastSendConfirmType = typeof(BroadcastSendConfirm<TUser, TRole, TOptions>);
	}
}