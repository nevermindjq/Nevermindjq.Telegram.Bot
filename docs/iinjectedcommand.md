# IInjectedCommand

## IInjectedCommand

`IInjectedCommand` extends `ICommand` and adds properties for dependency injection. It provides access to essential services and context required for command execution.

### Inheritance

* **Base interface:** `ICommand`

### Properties

* `IUserContextAsync? ContextAsync`\
  Provides access to the asynchronous user context, allowing commands to retrieve or store user-specific data.
* `IUpdateMediator<long> Mediator`\
  Mediator for coordinating update handling and communication between components.
* `ITelegramBotClient Bot`\
  The Telegram bot client used to interact with the Telegram Bot API (sending messages, answering queries, etc.).
