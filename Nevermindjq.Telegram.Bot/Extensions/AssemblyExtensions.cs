using System.Reflection;

using Nevermindjq.Telegram.Bot.Attributes;
using Nevermindjq.Telegram.Bot.Commands.Abstractions;

namespace Nevermindjq.Telegram.Bot.Extensions;

internal static class AssemblyExtensions {
	internal static IEnumerable<(Type type, IEnumerable<PathAttribute> attrs)> Commands(this Assembly assembly) {
		return assembly.GetTypes()
					   .Where(x => x is { IsClass: true, IsAbstract: false })
					   .Where(x => x.GetInterfaces().Contains(typeof(ICommand)))
					   .Select(x => (type: x, attr: x.GetCustomAttributes<PathAttribute>()));
	}
}