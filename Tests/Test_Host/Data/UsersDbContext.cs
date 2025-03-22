using Microsoft.EntityFrameworkCore;
using Nevermindjq.Telegram.Bot.Entities.Identity;

namespace Bot.Data {
	public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options) {
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);

			builder.Entity<User>(entity => {
				entity.HasKey(x => x.Id);
				entity.HasMany(x => x.Roles);
			});

			builder.Entity<Role>(entity => {
				entity.HasKey(x => x.Id);
				entity.HasMany(x => x.Users);
			});
		}
	}
}