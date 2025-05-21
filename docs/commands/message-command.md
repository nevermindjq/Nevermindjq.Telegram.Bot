# Message Command

## MessageCommand

`MessageCommand` is an abstract base class for creating commands that respond to incoming messages in a Telegram bot built with the **Nevermindjq.Telegram.Bot** library.

### Purpose

`MessageCommand` is designed to handle user text messages. By inheriting from this class, you implement your own logic for processing commands received via messages.

### Inheritance

* **Base class:** `Command`
* **Implements:** Abstract method `ExecuteAsync(Update update)`

### Features

* The `CanExecuteAsync(Update update)` method returns `true` only if the `Update` object contains a message (`update.Message` is not `null`).
* Makes it easy to create commands that respond only to messages, ignoring other update types (such as callbacks, inline queries, etc.).

### Usage Example

```csharp
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Telegram.Bot.Types;

[Path("/start")]
public class StartCommand : MessageCommand {
    public override Task ExecuteAsync(Update update) {
        return Bot.SendMessage(
            update.Message.From.Id,
            $"Hello, {update.Message.From.Username}! You sent: {update.Message.Text}"
        );
    }
}
```

### How It Works

1. **Automatic registration:** All classes inheriting from `MessageCommand` are automatically registered in the DI container.
2. **Filtering:** Before executing the command, `CanExecuteAsync` is called to check for the presence of a message.
3. **Execution:** Your logic is implemented in the `ExecuteAsync` method.

### When to Use

Use `MessageCommand` if your command should respond only to regular user messages (text, stickers, etc.), and not to other types of updates.
