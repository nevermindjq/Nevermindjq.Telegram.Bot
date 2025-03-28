using Nevermindjq.Models.Services.States;
using Nevermindjq.Telegram.Bot.States;

namespace Nevermindjq.Telegram.Bot.Services;

public class BotStateRepository(FileOptions options) : HostedStorableStateFile<BotState, FileOptions>(options);