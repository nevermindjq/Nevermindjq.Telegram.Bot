# DeleteMessage

## DeleteMessage

`DeleteMessage` is a command class in the `Nevermindjq.Telegram.Bot` library that handles the deletion of messages via a callback button in Telegram.

### Purpose

This class allows users to delete a message by pressing an inline keyboard button. It is typically used to provide a "Delete" option under bot messages.

### Key Features

* **Inline Button:** Provides a static `Button` property for easy reuse, which creates an inline button labeled "❌ Удалить" with callback data `"msg:delete"`.
* **Callback Handling:** Inherits from `Callback`, so it is triggered by a callback query.
* **Message Deletion:** Deletes the message associated with the callback query.
* **Mediator Integration:** Calls `Mediator.GetNext` to advance the update handling flow after deletion.

### Methods

* `Task ExecuteAsync(Update update)`\
  Handles the callback query, advances the mediator, and deletes the message.

### Usage Example

Add the `DeleteMessage.Button` to your inline keyboard markup to allow users to delete messages:

```csharp
var keyboard = new InlineKeyboardMarkup(DeleteMessage.Button);
```

When the button is pressed, the `DeleteMessage` command will be executed, and the message will be deleted.
