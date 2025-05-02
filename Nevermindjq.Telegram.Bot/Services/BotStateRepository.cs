using Nevermindjq.Models.States;
using Nevermindjq.Telegram.Bot.States;

namespace Nevermindjq.Telegram.Bot.Services;

internal class BotStateRepository(FileOptions options) : HostedStorableStateFile<BotState, FileOptions>(options);