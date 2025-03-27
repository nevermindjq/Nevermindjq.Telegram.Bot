using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;

namespace Nevermindjq.Telegram.Bot.Extensions;

internal static class AssemblyExtensions {
	internal static IEnumerable<(Type type, PathAttribute attr)> Commands(this Assembly assembly) {
		return assembly.GetTypes()
					   .Select(x => (type: x, attr: x.GetCustomAttribute<PathAttribute>()))
					   .Where(x => x.attr is not null)
					   .Cast<(Type type, PathAttribute attr)>();
	}
}