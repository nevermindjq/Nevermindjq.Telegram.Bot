using Microsoft.AspNetCore.Authorization;

using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Commands.Base;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Extensions;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Configurations;
using Nevermindjq.Telegram.Bot.Database.Models.Context;
using Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.UI;

[Path("broadcast"), Authorize("Admin")]
public class BroadcastPanel<TUser, TRole, TOptions>(
	IServiceProvider services,
	IRepository<TOptions> r_options,
	BroadcastConfiguration<TOptions> configuration
	) : InformationCommandAsync, IAuthenticated<TUser, TRole>
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole>
	where TOptions : class, IBroadcastOptions {

	// Public
	public TUser User { get; set; }

	// Private
	private TOptions? Options { get; set; }

	//
	public override async Task ProcessAsync(Update update) {
		Options ??= await r_options.FindAsync(x => x.UserId == update.CallbackQuery.From.Id).FirstAsync();
	}

	public override Task<string> BuildTextAsync(Update update) {
		ParseMode = configuration.ParseMode;

		return Task.FromResult(configuration.Text);
	}

	public override Task<InlineKeyboardMarkup> BuildMarkupAsync(Update update) {
		#region Get switchers buttons

		var switchers = Options!.Roles()
						.Where(x => {
							return configuration.Switchers.Any(cfg => cfg.Role == x);
						})
						.Select(x => (x, services.GetSwitcher<TOptions>($"brd:{x.ToLower()}")))
						.Cast<(string role, Switcher<TOptions>? switcher)>()
						.Where(x => x.switcher is not null)
						.Select(x => {
							return x.switcher!.Build(Options, configuration.Switchers.First(cfg => cfg.Role == x.role).State);
						})
						.Chunk(2)
						.Select(x => x.ToList())
						.ToList();

		#endregion

		var markup = new InlineKeyboardMarkup(switchers);

		if (configuration.Markup is not null) {
			foreach (var row in configuration.Markup) {
				markup.AddNewRow(row.ToArray());
			}
		}

		// Main
		markup.AddNewRow(new List<InlineKeyboardButton?> {
			configuration.UseTest ? InlineKeyboardButton.WithCallbackData("\ud83e\uddea Тест", "brd:send:test") : null,
			InlineKeyboardButton.WithCallbackData("\u270d\ufe0f Отправить", "brd:send:set:text"),
		}.Where(x => x is not null).ToArray()!);

		// Footer
		markup.AddNewRow(InlineKeyboardButton.WithCallbackData("\ud83d\udd19 На главную", "start"));

		//
		return Task.FromResult(markup);
	}
}

[Path("brd:send:test"), Authorize("Admin")]
public class BroadcastTest<TUser, TRole, TOptions>(
	IRepository<TUser> users,
	IRepository<TOptions> options,
	IUserContextAsync context
	) : Callback
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole>
	where TOptions : IBroadcastOptions {
	public override async Task ExecuteAsync(Update update) {
		await new BroadcastSendConfirm<TUser, TRole, TOptions>(users) {
			Bot = Bot,
			ContextAsync = context,
			Request = new BroadcastRequest<TOptions> {
				Text = "\ud83e\uddea Test",
				Options = await options.FindAsync(x => x.UserId == update.CallbackQuery.From.Id).FirstAsync(),
				Markup = [[DeleteMessage.Button]]
			},
		}.ExecuteAsync(update);
	}
}

[Path("brd:send:confirm"), Authorize("Admin")]
public class BroadcastSendConfirm<TUser, TRole, TOptions>(IRepository<TUser> users) : MessageCommand
	where TUser : IUser<TUser, TRole>
	where TRole : IRole<TUser, TRole>
	where TOptions : IBroadcastOptions {
	internal BroadcastRequest<TOptions>? Request { get; set; }

	protected bool IsValidOptions(IBroadcastOptions options) {
		return options.RolesInfo().Select(info => (bool)info.GetValue(options)!).Any(value => value);
	}

	protected async Task<HashSet<long>> GetUserIds(IBroadcastOptions options) {
		var receivers = new HashSet<long>();

		if (options.ToUsers) {
			receivers.UnionWith(
				await users.AllAsync()
						   .Where(x => !x.Roles.Any())
						   .Select(x => x.Id)
						   .ToHashSetAsync()
			);
		}

		foreach (var role_name in options.AllowedRoles()) {
			receivers.UnionWith(
					await users.AllAsync()
							   .Where(x => x.Roles.Any(x => x.Name == role_name))
							   .Select(x => x.Id)
							   .ToHashSetAsync()
			);
		}

		return receivers;
	}

	private List<List<InlineKeyboardButton>> GetMarkup(Update update) {
		var markup = new List<List<InlineKeyboardButton>>();

		// Get rows
		foreach (var row in update.Message.Text.Split('\n')) {
			var markup_row = new List<InlineKeyboardButton>();

			// Get buttons
			foreach (var button in row.Split('|')) {
				// Get button information
				// 1 - text
				// 2 - action
				var info = button.Split('-')
								 .Select(x => x.Trim())
								 .ToList();

				if (info.Count != 2) {
					continue;
				}

				// Add button
				if (Uri.TryCreate(info[1], UriKind.Absolute, out _)) {
					markup_row.Add(InlineKeyboardButton.WithUrl(info[0], info[1]));
				}
				else {
					markup_row.Add(InlineKeyboardButton.WithCallbackData(info[0], info[1]));
				}
			}

			markup.Add(markup_row);
		}

		return markup;
	}

	public override async Task ExecuteAsync(Update update) {
		Request ??= await ContextAsync.GetAsync<BroadcastRequest<TOptions>>(update.Message.From.Id);

		if (Request is null) {
			throw new ArgumentNullException(nameof(Request));
		}

		if (!IsValidOptions(Request.Options)) {
			await Bot.SendMessage(update.Message.From.Id, "Получатели не выбраны", replyMarkup: new InlineKeyboardMarkup("Вернуться", "broadcast"));

			return;
		}

		Request.Markup = GetMarkup(update);

		// Send
		var receivers = await GetUserIds(Request.Options);

		if (!receivers.Any()) {
			return;
		}

		foreach (var id in receivers) {
			try {
				// TODO Implement image loading from 'Request.ImageStream'
				await Bot.SendMessage(id, Request.Text, Request.ParseMode, replyMarkup: new InlineKeyboardMarkup(Request.Markup));
			}
			catch {
				// ignored
			}
		}

		await Bot.SendMessage(update.Message.From.Id, "Сообщения успешно отправлены", replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Вернуться", "broadcast")));
	}
}