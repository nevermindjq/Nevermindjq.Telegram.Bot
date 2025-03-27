namespace Nevermindjq.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PathAttribute(string path) : Attribute {
	public string Path { get; set; } = path;
	public string? Description { get; set; }

	/// <summary>
	///		Set order in commands list.
	/// </summary>
	public int InformationOrder { get; set; } = 0;
}