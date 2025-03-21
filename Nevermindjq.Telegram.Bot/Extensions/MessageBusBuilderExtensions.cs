using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;

using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;

namespace Nevermindjq.Telegram.Bot.Extensions;

public static class MessageBusBuilderExtensions {
	public static void AutoDeclareCommands(this MemoryMessageBusBuilder builder) {
		builder.AutoDeclareFrom(Assembly.GetExecutingAssembly(), type => type.GetCustomAttribute<PathAttribute>() is not null);

		builder.RegisterCommandsWithPath();
	}

	private static void RegisterCommandsWithPath(this MessageBusBuilder builder) {
		var consumerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<PathAttribute>() is not null);

		foreach (var consumerType in consumerTypes) {
			if (consumerType.GetCustomAttribute<PathAttribute>() is { Path: var path }) {
				builder.Consume<Update>(x => x.Path($"{nameof(Update)} {path}").WithConsumer(consumerType));
			}
		}
	}
}