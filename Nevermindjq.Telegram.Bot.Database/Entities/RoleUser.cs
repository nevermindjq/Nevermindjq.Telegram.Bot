namespace Nevermindjq.Telegram.Bot.Database.Entities;

public class RoleUser {
	public string RolesId { get; set; }
	public long UsersId { get; set; }
	public Role Role { get; set; }
	public User User { get; set; }
}