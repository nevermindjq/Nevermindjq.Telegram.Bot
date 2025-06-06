# 👋 Welcome

## Welcome to **Nevermindjq.Telegram.Bot** 👋

A C# library built on top of the [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) package that streamlines the creation of Telegram bots. Whether you need just the core messaging features or full database-backed user management (authentication, authorization) and basic UI, we’ve got you covered!

***

## 🚀 Get Started

### 1. Install via NuGet

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
> Check out [How to append NuGet source](get-started/how-to-add-nuget-source.md) for step-by-step instructions.

***

### 2. Register in DI

In your `Program.cs` (or `Startup.cs`), register the bot services:

```csharp
builder.Services.AddTelegramBot(builder.Configuration["Bot:Token"]!, use_caching_user_context: false);
```

***

### 3. Usage

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

Commands in the `Nevermindjq.Telegram.Bot` library are automatically registered in the Dependency Injection (DI) container. This is achieved by making use of reflection to scan for all classes that inherit from `ICommand`.

***

## 📖 Full Documentation

For the complete guide, advanced topics, and API reference, visit our GitBook:\
🔗 [Nevermindjq.Telegram.Bot Documentation](https://nevermindjqs-organization.gitbook.io/nevermindjq.telegram.bot/)
