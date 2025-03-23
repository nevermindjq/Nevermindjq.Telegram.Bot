using Microsoft.EntityFrameworkCore;

using Nevermindjq.Telegram.Bot.Database.Entities;

namespace Nevermindjq.Telegram.Bot.Database.Data {
	public abstract class UsersDbContext<TContext>(DbContextOptions<TContext> options) : DbContext(options) where TContext : DbContext {
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			base.OnModelCreating(builder);

			builder.Entity<User>()
						.HasMany(x => x.Roles)
						.WithMany(x => x.Users)
						.UsingEntity<RoleUser>(
							x => x
								  .HasOne(c => c.Role)
								  .WithMany()
								  .HasForeignKey(c => c.RolesId),
							x => x
								  .HasOne(c => c.User)
								  .WithMany()
								  .HasForeignKey(c => c.UsersId)
						);
		}
	}
}