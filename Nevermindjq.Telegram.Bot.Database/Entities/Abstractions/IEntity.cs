namespace Nevermindjq.Telegram.Bot.Database.Entities.Abstractions;

public interface IEntity<T> {
	public T Id { get; set; }
}