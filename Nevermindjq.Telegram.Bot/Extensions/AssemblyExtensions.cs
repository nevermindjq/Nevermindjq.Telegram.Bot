using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Extensions;

public static class AssemblyExtensions {
	public static (Type type, IEnumerable<PathAttribute> attrs) CommandInfo(this Type type) {
		if (type.GetInterfaces().All(x => x != typeof(ICommand))) {
			throw new ArgumentException("Given type does not implement ICommand interface", nameof(type));
		}
		
		return (type, attrs: type.GetCustomAttributes<PathAttribute>(true));
	}

	internal static (Type type, IEnumerable<PathAttribute> attrs) CommandInfo<TCommand>() where TCommand : ICommand {
		return CommandInfo(typeof(TCommand));
	}

	internal static IEnumerable<(Type type, IEnumerable<PathAttribute> attrs)> Commands(this Assembly assembly) {
		return assembly.GetTypes()
					   .Where(x => x is { IsClass: true, IsAbstract: false })
					   .Where(x => x.GetInterfaces().Contains(typeof(ICommand)))
					   .Select(CommandInfo);
	}
}