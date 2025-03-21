namespace Nevermindjq.Telegram.Bot.Extensions;

internal static class StringExtensions {
	internal static string UpTo(this string value, int startIndex, string delimiter) {
		var index = value.IndexOf(delimiter, StringComparison.Ordinal);

		if (index == -1 || index > startIndex) {
			return value;
		}

		return value.Substring(startIndex, index);
	}
}