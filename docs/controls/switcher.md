# Switcher

## Switcher

`Switcher<T>` is a sealed generic command class in the `Nevermindjq.Telegram.Bot` library for toggling boolean-like options via inline keyboard buttons in Telegram bots. It inherits from `Callback`.

### Purpose

This class provides a reusable way to display and toggle on/off states for any entity, updating both the data and the button label in the inline keyboard.

### Key Features

* **Generic:** Works with any entity type `T` managed by an `IRepository<T>`.
* **Dynamic Button State:** Updates button text and callback data to reflect the current state.
* **Custom Logic:** Allows injection of model retrieval and toggle logic via delegates.
* **Inline Keyboard Integration:** Edits the inline keyboard in-place after toggling.

### Properties

* `Text`: The label for the switch button.
* `GetModel`: Delegate to fetch the target model from the repository.
* `Toggle`: Delegate to toggle the state of the model.
* `Key`: Internal string used for callback data prefix.

### Methods

* `Task ExecuteAsync(Update update)`: Handles the callback, toggles the value, updates the button text and callback data, and edits the message's reply markup.
* `InlineKeyboardButton Build<TObject>(TObject options, Expression<Func<TObject, bool>> current)`: Builds a switch button reflecting the current state.

### Usage Example

```csharp
services.AddSwitcher($"feature_switch", (s, _) => 
        new Switcher<T>(s.GetRequiredService<IRepository<T>>()) {
	    GetModel = async (repo, update) => await repo.GetByIdAsync(...),
            Toggle = async (repo, model) => { 
                model.Enabled = !model.Enabled;
                await repo.UpdateAsync(model);
            },
            Key = "feature_switch"
	}
);
```

### When to Use

Use `Switcher<T>` when you need to provide users with toggleable options (e.g., enable/disable features) directly in Telegram bot messages, with immediate UI and data updates.
