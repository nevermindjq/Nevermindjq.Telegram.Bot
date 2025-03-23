using Microsoft.EntityFrameworkCore;

namespace Test_Host.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : Nevermindjq.Telegram.Bot.Database.Data.UsersDbContext<UsersDbContext>(options);