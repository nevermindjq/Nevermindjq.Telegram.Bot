namespace Nevermindjq.Telegram.Bot.Extensions;

public static class StringExtensions {
	internal static string UpTo(this string value, int start_index = 0, string delimiter = " ") {
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (delimiter == null)
			throw new ArgumentNullException(nameof(delimiter));

		var index = value.IndexOf(delimiter, start_index, StringComparison.OrdinalIgnoreCase);

		return index == -1 ? value : value[start_index..index];
	}

	public static string? After(this string value, int start_index = 0, string delimiter = " ") {
		if (value == null)
			throw new ArgumentNullException(nameof(value));
		if (delimiter == null)
			throw new ArgumentNullException(nameof(delimiter));

		var index = value.IndexOf(delimiter, start_index, StringComparison.OrdinalIgnoreCase);

		return index == -1 ? null : value[index..];
	}
}