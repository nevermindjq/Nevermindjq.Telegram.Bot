using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;

using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;

namespace Nevermindjq.Telegram.Bot.Extensions;

public static class MessageBusBuilderExtensions {
	public static void AutoRegisterCommands(this MemoryMessageBusBuilder builder, Assembly assembly) {
		builder.AutoDeclareFrom(assembly, type => type.GetCustomAttribute<PathAttribute>() is null, _ => nameof(Update))
			   .AddServicesFromAssembly(assembly, type => type.GetCustomAttribute<PathAttribute>() is null);

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