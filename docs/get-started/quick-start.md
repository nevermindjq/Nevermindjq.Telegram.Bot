# 🚀 Quick start

## Install packages via NuGet

#### Package Manager

```powershell
# Core package
Install-Package Nevermindjq.Telegram.Bot -Source GitHub/Nevermindjq

# Database extensions with UI (user auth)
Install-Package Nevermindjq.Telegram.Bot.Database -Source GitHub/Nevermindjq
```

***

#### .NET CLI

```bash
# Core package
dotnet add package Nevermindjq.Telegram.Bot --source GitHub/Nevermindjq

# Database extensions with UI and user authentication/authorization
dotnet add package Nevermindjq.Telegram.Bot.Database --source GitHub/Nevermindjq
```

> Need to add the custom NuGet source?\
> Check out [How to append NuGet source](how-to-add-nuget-source.md) for step-by-step instructions.

***

## Register in DI

In your `Program.cs` (or `Startup.cs`), register the bot services:

```csharp
builder.Services.AddTelegramBot(builder.Configuration["Bot:Token"]!, use_caching_user_context: false);
```

#### Parameters of `AddTelegramBot` Extension Method

1. **token**: The token of your Telegram bot, which can be obtained from the @BotFather on Telegram. Ensure you replace `"Bot:Token"` with your actual bot token in the configuration to enable communication with the Telegram API.
2. **use\_caching\_user\_context:** Enable caching for `UserContext` using `IDistributedCache`.

***

## Create command

After registration and configuration, create your own command by inheriting from the abstract `MessageCommand` class:

```csharp
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nevermindjq.Telegram.Bot.Example.Commands;

[Path("/start")]
public class StartCommand : MessageCommand {
	public override Task ExecuteAsync(Update update) {
		return Bot.SendMessage(update.Message.From.Id, $"Hello {update.Message.From.Username}! You send: {update.Message.Text}");
	}
}
```

#### Explanation of StartCommand Code

* **Attribute**:
  * `[Path("/start")]`: This attribute associates the command with the `/start` path. It listens for messages with this specific command.
* **Class Definition**:
  * `public class StartCommand : MessageCommand`: `StartCommand` inherits from `MessageCommand`, which allows it to receive non-null messages from `Telegram.Bot.Update`.
* **ExecuteAsync Method**:
  * `public override Task ExecuteAsync(Update update)`: This overrides the base method from `MessageCommand` to define custom execution logic.
  * `return Bot.SendMessage(update.Message.From.Id, $"Hello {update.Message.From.Username}! You send: {update.Message.Text}");`: Sends a formatted message back to the user, acknowledging receipt of their message and including their username and text.

Commands in the `Nevermindjq.Telegram.Bot` library are automatically registered in the Dependency Injection (DI) container. This is achieved by making use of reflection to scan for all classes that inherit from `ICommand`.
