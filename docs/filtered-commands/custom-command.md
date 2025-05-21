# Custom Command

## Creating a Custom Filtered Command

You can create your own filtered command by inheriting from the base `Command` class and overriding the `CanExecuteAsync` method to filter specific update types. This allows you to handle only the updates you need, similar to how `MessageCommand` and `Callback` work.

### Example: InlineQueryCommand

Suppose you want to handle only inline queries. You can create an `InlineQueryCommand` as follows:

```csharp
using Nevermindjq.Telegram.Bot.Commands.Abstractions;
using Telegram.Bot.Types;

public abstract class InlineQueryCommand : Command {
    public override Task<bool> CanExecuteAsync(Update update) =>
        Task.FromResult(update.InlineQuery is { });
}
```

### Usage Example

```csharp
using Nevermindjq.Telegram.Bot.Attributes;
using Telegram.Bot.Types;

[Path("my_inline_query")]
public class MyInlineQueryCommand : InlineQueryCommand {
    public override Task ExecuteAsync(Update update) {
        // Your logic for handling inline queries
        return Bot.AnswerInlineQuery(
            update.InlineQuery.Id,
            results: new[] { /* your InlineQueryResult array */ }
        );
    }
}
```

### How It Works

1. **Filtering:** The `CanExecuteAsync` method checks if the update contains an `InlineQuery`. Only then will the command execute.
2. **Implementation:** Place your handling logic in the `ExecuteAsync` method.
3. **Registration:** All commands inheriting from `Command` are automatically registered in the DI container.

### When to Use

Create a custom filtered command when you need to handle a specific type of update (e.g., inline queries, edited messages, etc.) and want to keep your command logic clean and focused.
