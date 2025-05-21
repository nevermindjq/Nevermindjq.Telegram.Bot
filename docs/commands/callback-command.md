# Callback Command

## Callback

`Callback` is an abstract base class for creating commands that respond to callback queries in a Telegram bot built with the **Nevermindjq.Telegram.Bot** library.

### Purpose

`Callback` is designed to handle user interactions with inline buttons (callback queries). By inheriting from this class, you implement your own logic for processing commands received via callback queries.

### Inheritance

* **Base class:** `Command`
* **Implements:** Abstract method `ExecuteAsync(Update update)`

### Features

* The `CanExecuteAsync(Update update)` method returns `true` only if the `Update` object contains a callback query (`update.CallbackQuery` is not `null`).
* Makes it easy to create commands that respond only to callback queries, ignoring other update types (such as messages, inline queries, etc.).

### Usage Example

```csharp
using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Filtered;
using Telegram.Bot.Types;

[Path("my_callback")]
public class MyCallbackCommand : Callback {
    public override Task ExecuteAsync(Update update) {
        return Bot.SendMessage(
            update.CallbackQuery.From.Id,
            $"You pressed a button with data: {update.CallbackQuery.Data}"
        );
    }
}
```

### How It Works

1. **Automatic registration:** All classes inheriting from `Callback` are automatically registered in the DI container.
2. **Filtering:** Before executing the command, `CanExecuteAsync` is called to check for the presence of a callback query.
3. **Execution:** Your logic is implemented in the `ExecuteAsync` method.

### When to Use

Use `Callback` if your command should respond only to callback queries from inline buttons, and not to other types of updates.
