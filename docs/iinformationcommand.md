---
hidden: true
---

# IInformationCommand

## IInformationCommand

`IInformationCommand` is an interface for commands that provide informational responses in the **Nevermindjq.Telegram.Bot** library. It extends `IInjectedCommand`, allowing access to injected services and context.

### Methods

* `Task ProcessAsync(Update update)`\
  Handles the main processing logic for the command when an update is received.
* `string BuildText(Update update)`\
  Builds and returns the text content to be sent in response to the update.
* `InlineKeyboardMarkup BuildMarkup(Update update)`\
  Builds and returns the inline keyboard markup to be sent with the response.

***

## IInformationCommandAsync

`IInformationCommandAsync` extends `IInformationCommand` and adds asynchronous methods for building text and markup.

### Additional Methods

* `Task<string> BuildTextAsync(Update update)`\
  Asynchronously builds and returns the text content for the response.
* `Task<InlineKeyboardMarkup> BuildMarkupAsync(Update update)`\
  Asynchronously builds and returns the inline keyboard markup for the response.

***

## **Summary**

Use `IInformationCommand` for informational commands with synchronous text and markup building. Use `IInformationCommandAsync` when you need to build text or markup asynchronously, for example, when fetching data from external sources. Both interfaces provide a structured way to handle and respond to updates with rich content.
