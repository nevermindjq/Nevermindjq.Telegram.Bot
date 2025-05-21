# ICommand

## ICommand

`ICommand` is a core interface for all command types in the **Nevermindjq.Telegram.Bot** library. It defines the basic contract for handling and executing commands in response to Telegram updates.

### Methods

* `Task OnHandleAsync(Update update)`\
  Handles the incoming update. Typically, this method manages the command execution flow, including error handling.
* `Task ExecuteAsync(Update update)`\
  Contains the main logic for the command. This method should be implemented to define what the command does when triggered.
* `Task<bool> CanExecuteAsync(Update update)`\
  Determines whether the command should execute for the given update. Returns `true` if the command can be executed, otherwise `false`.
