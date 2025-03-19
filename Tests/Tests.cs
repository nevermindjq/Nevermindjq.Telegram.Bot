using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.States;
using FileOptions=Nevermindjq.Models.Services.States.Options.FileOptions;

namespace Tests;

public class Tests {
	private BotStateRepository _state;

	[OneTimeSetUp] public void OneTimeSetUp() {
		_state = new BotStateRepository(new FileOptions("Data", "options")) {
			Model = new BotState {
				Offset = 50
			}
		};
	}

	[Test] public async Task TestStart() {
		if (Directory.Exists("Data")) {
			Directory.Delete("Data", true);
		}

		await _state.StartAsync(default);

		Assert.That(Directory.Exists(_state.Options.Root), Is.True);
		Assert.That(_state.Model.Offset, Is.EqualTo(50));
	}

	[Test] public async Task TestStop() {
		await _state.StopAsync(default);

		Assert.That(File.Exists(Path.Combine(_state.Options.Root, _state.Options.FileName)), Is.True);
	}
}