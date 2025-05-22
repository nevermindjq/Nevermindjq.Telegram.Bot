using Nevermindjq.Telegram.Bot.Services.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services;

public class UpdateResolver : IUpdateResolver {
	public T? Payload<T>(Update update) where T : class {
		object? result = null;

		switch (update) {
			case { Message: not null }:
				result = update.Message.Text;
				break;
			case { CallbackQuery: not null }:
				result = update.CallbackQuery.Data;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(update), update, "Unsupported update type");
		}

		if (result.GetType() != typeof(T)) {
			return null;
		}

		return result as T;
	}

	public int MessageId(Update update) {
		switch (update) {
			case { Message: not null }:
				return update.Message.MessageId;
			case { CallbackQuery: not null }:
				return update.CallbackQuery.Message.MessageId;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public long UserId(Update update) {
		switch (update) {
			case { Message: not null }:
				return update.Message.From.Id;
			case { CallbackQuery: not null }:
				return update.CallbackQuery.From.Id;
			default:
				throw new ArgumentOutOfRangeException(nameof(update), update, "Unsupported update type");
		}
	}
}