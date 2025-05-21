# InformationCommand

## InformationCommand

`InformationCommand` is an abstract base class in the **Nevermindjq.Telegram.Bot** library for creating commands that provide informational responses to users. It implements the `IInformationCommand` interface and extends the `Command` base class.

### Purpose

The class is designed to simplify the creation of commands that send or edit messages with text and inline keyboards, supporting both standard messages and callback queries.

### Key Features

* **ParseMode Support:** Allows specifying the parse mode (e.g., HTML) for message formatting.
* **Flexible Response:** Handles both incoming messages and callback queries, sending or editing messages as appropriate.
* **Customizable Content:** Requires implementation of methods to build the response text and markup.

### Methods

* `Task ProcessAsync(Update update)`\
  Optional method for pre-processing before sending a response. Default implementation does nothing.
* `abstract string BuildText(Update update)`\
  Must be implemented to generate the text content for the response.
* `abstract InlineKeyboardMarkup BuildMarkup(Update update)`\
  Must be implemented to generate the inline keyboard markup for the response.
* `override Task ExecuteAsync(Update update)`\
  Executes the command: processes the update, then sends or edits a message with the built text and markup.
* `override Task<bool> CanExecuteAsync(Update update)`\
  Returns `true` if the update contains a message or callback query.
* `long GetUserId(Update update)`\
  Helper method to extract the user ID from the update.

### Usage Example

```csharp
public class HelpCommand : InformationCommand {
    public override string BuildText(Update update) => "This is the help message.";
    public override InlineKeyboardMarkup BuildMarkup(Update update) => new InlineKeyboardMarkup(...);
}
```

### When to Use

Use `InformationCommand` when you need to create commands that respond with formatted text and inline keyboards, handling both messages and callback queries in a unified way.
