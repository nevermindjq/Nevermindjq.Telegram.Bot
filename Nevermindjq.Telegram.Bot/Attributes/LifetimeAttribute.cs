using Microsoft.Extensions.DependencyInjection;

namespace Nevermindjq.Telegram.Bot.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class LifetimeAttribute(ServiceLifetime lifetime) : Attribute {
	public ServiceLifetime Lifetime { get; set; } = lifetime;
}