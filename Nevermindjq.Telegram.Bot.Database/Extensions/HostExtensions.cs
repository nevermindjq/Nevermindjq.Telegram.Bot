using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Models.Repositories.Abstractions;
using Nevermindjq.Telegram.Bot.Commands;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Entities.Identity.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Middlewares;
using Nevermindjq.Telegram.Bot.Database.Models.Abstractions;
using Nevermindjq.Telegram.Bot.Database.Models.Configurations;
using Nevermindjq.Telegram.Bot.Database.Models.Context;
using Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;
using Nevermindjq.Telegram.Bot.Extensions;
using Nevermindjq.Telegram.Bot.Middlewares.Abstractions;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Database.Extensions;

public static class HostExtensions {
	public static void AddTelegramAuthentication<TUser, TRole, TRedirect>(this IServiceCollection services)
		where TUser : IUser<TUser, TRole>
		where TRole : IRole<TUser, TRole>
		where TRedirect : ICommand {
		services.AddTransient<ICommandMiddleware<IAuthenticated<TUser, TRole>>, AuthenticationMiddleware<TUser, TRole, TRedirect>>();
	}

	public static void AddTelegramAuthorization<TUser, TRole, TRedirect>(this IServiceCollection services)
		where TUser : IUser<TUser, TRole>
		where TRole : IRole<TUser, TRole>
		where TRedirect : ICommand {
		services.AddTransient<IAttributedMiddleware<AuthorizeAttribute, IAuthenticated<TUser, TRole>>, AuthorizationMiddleware<TUser, TRole, TRedirect>>();
	}
	
	#region UI

	public static void AddTelegramUI<TBroadcastOptions>(
		this IServiceCollection services,
		Action<AdminConfiguration>? admin = null,
		Action<BroadcastConfiguration<TBroadcastOptions>>? broadcast = null
	) where TBroadcastOptions : class, IBroadcastOptions {
		services.AddAdmin(admin);
		services.AddBroadcast(broadcast);
	}

	private static void AddAdmin(this IServiceCollection services, Action<AdminConfiguration>? admin) {
		if (admin is null) {
			return;
		}

		var admin_configuration = new AdminConfiguration();

		admin(admin_configuration);

		if (admin_configuration is { Enabled: true, AdminType: null }) {
			throw new ArgumentException($"Admin panel was enabled, but '{nameof(AdminConfiguration.AdminType)}' is not set. Use {nameof(AdminConfiguration.AdminType)} to set.", nameof(admin));
		}

		services.AddSingleton(admin_configuration);
		services.AddCommand(admin_configuration.AdminType!.CommandInfo());
	}

	private static void AddBroadcast<TOptions>(this IServiceCollection services, Action<BroadcastConfiguration<TOptions>>? broadcast)
		where TOptions : class, IBroadcastOptions {
		if (broadcast is null) {
			return;
		}

		var broadcast_configuration = new BroadcastConfiguration<TOptions>();

		broadcast(broadcast_configuration);

		if (!broadcast_configuration.Enabled) {
			return;
		}

		if (broadcast_configuration is { Enabled: true, BroadcastType: null }) {
			throw new ArgumentException($"Broadcast was enabled, but '{nameof(BroadcastConfiguration<TOptions>.BroadcastSendConfirmType)}' is not set. Use {nameof(BroadcastConfiguration<TOptions>.SetBroadcastType)} to set.", nameof(broadcast));
		}

		// Register commands
		services.AddSingleton(broadcast_configuration);
		services.AddCommand(broadcast_configuration.BroadcastType!.CommandInfo());
		services.AddCommand(broadcast_configuration.BroadcastSendConfirmType!.CommandInfo());

		if (broadcast_configuration.UseTest) {
			services.AddCommand(broadcast_configuration.BroadcastTestType!.CommandInfo());
		}

		// Register switchers
		foreach (var config in broadcast_configuration.Switchers) {
			services.AddSwitcher($"brd:{config.Role.ToLower()}", (s, _) => {
				return new Switcher<TOptions>(s.GetRequiredService<IRepository<TOptions>>()) {
					Text = config.Text,
					GetModel = config.GetModel,
					Toggle = config.Toggle
				};
			});
		}

		// Register intermediate commands
		services.AddCommand<ICommand>("brd:send:set:text", (services, _) => {
			return new SetValueStartCallback<BroadcastRequest<TOptions>>("brd:send:set:parse_mode", services) {
				Text = """
				*Введите текст рассылки:*
				Пожалуйста, отправьте текст сообщения, которое вы хотите разослать\.
				Вы можете использовать разметку Telegram \(*жирный*, _курсив_, [ссылки](https://www\.example\.com/) и т\.д\.\)\.
				""",
				InitializeValue = async (update, _, provider) => new BroadcastRequest<TOptions> {
					Options = await provider.GetRequiredService<IRepository<TOptions>>().FindAsync(x => x.UserId == update.CallbackQuery.From.Id).FirstOrDefaultAsync()
				},
				Markup = new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("Вернуться", "start"))
			};
		});
		
		services.AddCommand<ICommand>("brd:send:set:parse_mode", (_, _) => {
			return new SetValueMessage<BroadcastRequest<TOptions>>("brd:send:set:markup") {
				Text = $$"""
				Укажите тип *parse mode*, чтобы определить форматирование текста\.
				Доступные варианты:
				{{ParseMode.None.ToString()}}		\- {{(int)ParseMode.None}}
				{{ParseMode.Markdown.ToString()}}	\- {{(int)ParseMode.Markdown}}
				{{ParseMode.Html.ToString()}}		\- {{(int)ParseMode.Html}}
				{{ParseMode.MarkdownV2.ToString()}}	\- {{(int)ParseMode.MarkdownV2}}
				""",
				SetValue = (update, model) => model.Text = update.Message.Text
			};
		});

		services.AddCommand<ICommand>("brd:send:set:markup", (_, _) => {
			return new SetValueMessage<BroadcastRequest<TOptions>>("brd:send:confirm") {
				Text = """
				*Введите inline\-кнопки:*
				Теперь добавьте inline\-кнопки с помощью следующего синтаксиса:
				`Текст кнопки \- URL или callback_data`
				Каждая кнопка \- с новой строки\.
				Если вы хотите разместить кнопки в одной строке, объедините их через \|\.
				""",
				SetValue = (update, model) => {
					if (int.TryParse(update.Message.Text, out var @case)) {
						model.ParseMode = (ParseMode)@case;
					}
					else {
						model.ParseMode = ParseMode.MarkdownV2;
					}
				}
			};
		});
	}

	#endregion
}