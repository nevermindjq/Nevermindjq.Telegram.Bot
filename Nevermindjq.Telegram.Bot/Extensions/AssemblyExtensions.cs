using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Extensions;

internal static class AssemblyExtensions {
	internal static IEnumerable<(Type type, PathAttribute? attr)> Commands(this Assembly assembly) {
		return assembly.GetTypes()
					   .Where(x => x.IsAssignableFrom(typeof(ICommand)))
					   .Select(x => (type: x, attr: x.GetCustomAttribute<PathAttribute>()));
	}
}