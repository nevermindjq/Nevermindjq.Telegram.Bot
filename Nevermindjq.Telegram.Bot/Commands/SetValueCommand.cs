using System.Linq.Expressions;

using Microsoft.Extensions.DependencyInjection;

using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Nevermindjq.Telegram.Bot.Services.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nevermindjq.Telegram.Bot.Commands;

public interface ISetValueCommand<TContextModel> : IInjectedCommand where TContextModel : class {
	public string Text { get; set; }
	public ParseMode ParseMode { get; set; }
	public InlineKeyboardMarkup Markup { get; set; }

	public uint CacheTimeout { get; set; }
	public uint DeleteCount { get; set; }
}

public class SetValueStartCallback<TContextModel>(string redirect_key, IServiceProvider provider) : Callback, ISetValueCommand<TContextModel> where TContextModel : class {
	public required string Text { get; set; }
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;
	public InlineKeyboardMarkup Markup { get; set; } = new(DeleteMessage.Button);

	public uint CacheTimeout { get; set; } = 60 * 5;
	public uint DeleteCount { get; set; } = 2;
	public required Func<Update, IUserContextAsync, IServiceProvider, Task<TContextModel>> InitializeValue { get; set; }

	public override async Task ExecuteAsync(Update update) {
		//
		await ContextAsync.SetAsync(update.CallbackQuery.From.Id, await InitializeValue(update, ContextAsync, provider), 30);

		//
		Mediator.AddNext<ICommand>(update.CallbackQuery.From.Id, redirect_key, DeleteCount);

		//
		await Bot.EditMessageText(
			update.CallbackQuery.From.Id,
			update.CallbackQuery.Message.MessageId,
			Text, ParseMode,
			replyMarkup: Markup
		);
	}
}

public class SetValueMessage<TContextModel>(string redirect_key) : MessageCommand, ISetValueCommand<TContextModel> where TContextModel : class {
	public required string Text { get; set; }
	public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;
	public InlineKeyboardMarkup Markup { get; set; } = new(DeleteMessage.Button);

	public uint CacheTimeout { get; set; } = 30;
	public uint DeleteCount { get; set; } = 2;
	public required Action<Update, TContextModel> SetValue { get; set; }

	public override async Task ExecuteAsync(Update update) {
		var id = update.Message?.From.Id ?? update.CallbackQuery.From.Id;

		//
		if (await ContextAsync.GetAsync<TContextModel>(id, false) is not { } model) {
			throw new ArgumentNullException($"{nameof(ContextAsync)}.BroadcastRequest");
		}

		SetValue(update, model);

		await ContextAsync.SetAsync(id, model, CacheTimeout);

		//
		Mediator.AddNext<ICommand>(id, redirect_key, DeleteCount);

		//
		await Bot.SendMessage(
			id,
			Text, ParseMode,
			replyMarkup: Markup
		);
	}
}