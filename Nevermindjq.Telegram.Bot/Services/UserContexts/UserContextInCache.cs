using MemoryPack;

using Microsoft.Extensions.Caching.Distributed;

using Nevermindjq.Telegram.Bot.Services.Abstractions;

namespace Nevermindjq.Telegram.Bot.Services.UserContexts;

internal class UserContextInCache(IDistributedCache cache) : IUserContextAsync {
	public async Task<bool> SetAsync<T>(long key, T value, uint timeout = 1) where T : class {
		using (var memory = new MemoryStream()) {
			await MemoryPackSerializer.SerializeAsync(memory, value);

			try {
				await cache.SetAsync(key.ToString(), memory.ToArray(), new DistributedCacheEntryOptions {
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(timeout)
				});
			}
			catch {
				return false;
			}
		}

		return true;
	}

	public async Task<T?> GetAsync<T>(long key, bool remove = true) where T : class {
		if (await cache.GetAsync(key.ToString()) is { } bytes) {
			await using var memory = new MemoryStream(bytes);

			try {
				return await MemoryPackSerializer.DeserializeAsync<T>(memory);
			}
			finally {
				if (remove) {
					await cache.RemoveAsync(key.ToString());
				}
			}
		}

		return null;
	}
}