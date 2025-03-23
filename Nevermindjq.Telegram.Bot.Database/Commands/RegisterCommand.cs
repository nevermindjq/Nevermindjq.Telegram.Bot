using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Nevermindjq.Telegram.Bot.Database.Commands.Abstractions;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Database.Commands;

public class RegisterCommand(ITelegramBotClient bot, DbContext context) : MessageCommand, IRegisterCommand {
	public override async Task ExecuteAsync(Update update) {
		await RegisterAsync(update);

		await bot.SendMessage(update.Message.Chat.Id, "You have been successfully registered. Try again.");
	}

	// IRegisterCommand
	public async Task<bool> RegisterAsync(Update update) {
		var role = await context.FindAsync<Role>("1")!;
		var transaction = await context.Database.BeginTransactionAsync();

		try {
			await context.AddAsync(new User {
				Id = update.Message.From.Id,
				Roles = [role]
			});

			await context.SaveChangesAsync();
			await transaction.CommitAsync();
		}
		catch {
			await transaction.RollbackAsync();

			return false;
		}
		finally {
			await transaction.DisposeAsync();
		}

		return true;
	}
}