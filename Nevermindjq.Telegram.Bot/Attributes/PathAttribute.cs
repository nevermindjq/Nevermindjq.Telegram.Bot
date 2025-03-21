namespace Nevermindjq.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class PathAttribute(string path) : Attribute {
	public string Path { get; set; }
}