using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Pcf.Preference.Core.Models;


namespace Pcf.Preference.DataAccess;

public class RedisPreferenceRepository
{
    private readonly IDistributedCache _cache;
    private const string PreferencesKey = "preferences";

    public RedisPreferenceRepository(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<List<UserPreference>> GetPreferencesAsync()
    {
        var data = await _cache.GetStringAsync(PreferencesKey);
        if (string.IsNullOrEmpty(data))
            return new List<UserPreference>();

        return JsonSerializer.Deserialize<List<UserPreference>>(data)!;
    }

    public async Task SetPreferencesAsync(List<UserPreference> preferences)
    {
        var serialized = JsonSerializer.Serialize(preferences);
        await _cache.SetStringAsync(PreferencesKey, serialized);
    }
}
