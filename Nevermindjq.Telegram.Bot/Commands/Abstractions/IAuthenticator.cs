namespace Nevermindjq.Telegram.Bot.Commands.Abstractions {
	public interface IAuthenticator {
		public User? User { get; }

		public Task<User?> AuthenticateAsync(Update update);
	}
}