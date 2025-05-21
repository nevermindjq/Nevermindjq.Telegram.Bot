# PaginationCallback

## PaginationCallback

`PaginationCallback` is an abstract base class in the `Nevermindjq.Telegram.Bot` library for implementing paginated callback commands in Telegram bots. It inherits from `Callback` and implements the `IPaginationCallback` interface.

### Purpose

This class is designed to simplify the creation of commands that display data in pages, allowing users to navigate between pages using inline keyboard buttons.

### Key Features

* **Pagination Logic:** Handles page calculation and navigation (next/previous).
* **Customizable Content:** Requires implementation of methods to generate page text and markup.
* **Back Button Support:** Allows adding a custom back button to the keyboard.

### Abstract Members

* `InlineKeyboardButton BackButton { get; set; }`\
  The button used to return to the previous menu or exit pagination.
* `Task<string> Text(Update update, Range range)`\
  Returns the text to display for the current page.
* `Task<List<List<InlineKeyboardButton>>> Markup(Update update, Range range)`\
  Returns the inline keyboard layout for the current page.

### Main Method

* `Task ExecuteAsync(Update update, int per_page, int max)`\
  Handles the callback, calculates the current page, builds the markup, and edits the message to display the correct page and navigation buttons.

### Usage Example

To use `PaginationCallback`, inherit from it and implement the required members to define how each page's content and markup are built.

***

**When to Use:**\
Use `PaginationCallback` when you need to present large lists or datasets in a paginated format within Telegram, allowing users to navigate using inline buttons.
