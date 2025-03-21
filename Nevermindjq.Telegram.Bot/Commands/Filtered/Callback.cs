using System.Text;
using Microsoft.EntityFrameworkCore;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Commands.Filtered {
	public abstract class Callback : Command {
		private readonly string _data;

		public Callback(string data, DbContext? context = null) : base(context) {
			if (Encoding.UTF8.GetBytes(data) is {Length: > 64}){
				throw new Exception("Data length is more than 64 bytes.");
			}

			_data = data;
		}

		public override Task<bool> CanExecuteAsync(Update update) => Task.FromResult(update.CallbackQuery is { } callback && callback.Data == _data);

		protected override long GetUserId(Update update) => update.CallbackQuery.From.Id;
	}
}