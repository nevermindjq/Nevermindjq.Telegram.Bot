using System.Reflection;

using Nevermindjq.Telegram.Bot.Database.Models.Entities.Abstractions;

namespace Nevermindjq.Telegram.Bot.Database.Extensions;

internal static class BroadcastExtensions {
	internal static IEnumerable<PropertyInfo> RolesInfo(this IBroadcastOptions options) {
		return options.GetType()
				  .GetProperties()
				  .Where(x => x.Name.StartsWith("To") && x.PropertyType == typeof(bool));
	}

	internal static IEnumerable<string> Roles(this IBroadcastOptions options) {
		return options.RolesInfo()
					  .Select(x => x.Name.Substring("To".Length))
					  .SelectMany(x => new[] {x, x[..^1]});
	}

	internal static IEnumerable<string> AllowedRoles(this IBroadcastOptions options) {
		return options.RolesInfo()
					  .Where(x => (bool)x.GetValue(options)!)
					  .Select(x => x.Name.Substring("To".Length))
					  .SelectMany(x => new[] {x, x[..^1]});
	}
}