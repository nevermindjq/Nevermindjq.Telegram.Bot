namespace Nevermindjq.Telegram.Bot.Entities.Abstractions {
	public interface IEntity<T> {
		public T Id { get; set; }
	}
}