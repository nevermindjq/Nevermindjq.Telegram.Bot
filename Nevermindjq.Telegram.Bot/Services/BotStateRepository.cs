using Nevermindjq.Models.Services.States;
using Nevermindjq.Telegram.Bot.States;
using FileOptions=Nevermindjq.Models.Services.States.Options.FileOptions;

namespace Nevermindjq.Telegram.Bot.Services {
	public class BotStateRepository(FileOptions options) : HostedStateFile<BotState, FileOptions>(options);
}