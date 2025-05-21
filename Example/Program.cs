using Nevermindjq.Telegram.Bot.Extensions;

// Build
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTelegramBot(builder.Configuration["Bot:Token"]!, use_caching_user_context: false);

// Run
builder.Build().Run();