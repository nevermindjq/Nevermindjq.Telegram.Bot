# SetValue Commands

## SetValue Commands

`SetValueStartCallback<TContextModel>` and `SetValueMessage<TContextModel>` are generic command classes in the `Nevermindjq.Telegram.Bot` library designed to facilitate multi-step value input and storage in user context.

### Purpose

These commands help implement workflows where a value must be initialized, set, and stored in a user-specific context, supporting both callback and message-based interactions.

***

### SetValueStartCallback\<TContextModel>

* **Type:** Callback command
* **Usage:** Starts the value input process via a callback button.
* **Key Properties:**
  * `Text`: Message text to display.
  * `ParseMode`: Formatting mode for the message.
  * `Markup`: Inline keyboard markup (default includes a delete button).
  * `CacheTimeout`: How long to cache the context value (seconds).
  * `DeleteCount`: How many steps to keep in the mediator chain.
  * `InitializeValue`: Function to initialize the context model asynchronously.
* **Main Method:**
  * `ExecuteAsync(Update update)`: Initializes the value, stores it in the user context, sets up the next command, and edits the message.

***

### SetValueMessage\<TContextModel>

* **Type:** Message command
* **Usage:** Handles the user’s message to set the value in the context.
* **Key Properties:**
  * `Text`: Message text to display after setting the value.
  * `ParseMode`: Formatting mode for the message.
  * `Markup`: Inline keyboard markup (default includes a delete button).
  * `CacheTimeout`: How long to cache the context value (seconds).
  * `DeleteCount`: How many steps to keep in the mediator chain.
  * `SetValue`: Action to set the value in the context model.
* **Main Method:**
  * `ExecuteAsync(Update update)`: Retrieves the context model, applies the value, updates the context, sets up the next command, and sends a confirmation message.

***

### When to Use

Use `SetValue` commands to implement multi-step forms or wizards in Telegram bots, where you need to collect and store user input across several steps, leveraging both callback and message-based interactions.
