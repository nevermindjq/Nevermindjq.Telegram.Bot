using Nevermindjq.Telegram.Bot.Services;
using Nevermindjq.Telegram.Bot.States;
using FileOptions=Nevermindjq.Models.Services.States.Options.FileOptions;

namespace Tests;

[TestFixture]
public class Tests_BotStateRepository {
	private BotStateRepository _state;

	[OneTimeSetUp] public void OneTimeSetUp() {
		_state = new BotStateRepository(new FileOptions("Data", "options")) {
			Model = new BotState {
				Offset = 50
			}
		};
	}

	[Test] public async Task Test_Start() {
		if (Directory.Exists("Data")) {
			Directory.Delete("Data", true);
		}

		await _state.StartAsync(default);

		Assert.That(Directory.Exists(_state.Options.Root), Is.True);
		Assert.That(_state.Model.Offset, Is.EqualTo(50));
	}

	[Test] public async Task Test_Stop() {
		await _state.StopAsync(default);

		Assert.That(File.Exists(Path.Combine(_state.Options.Root, _state.Options.FileName)), Is.True);
	}
}