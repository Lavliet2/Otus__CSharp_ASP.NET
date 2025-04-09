using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using PreferenceEntity = Pcf.ReceivingFromPartner.Core.Domain.Preference;

namespace Pcf.ReceivingFromPartner.Integration
{
    public class PreferenceGateway
    {
        private readonly HttpClient _httpClient;

        public PreferenceGateway(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PreferenceEntity>> GetPreferencesAsync()
        {
            var redisPreferences = await _httpClient.GetFromJsonAsync<List<RedisPreference>>("api/Preference");

            return redisPreferences?
                .Select(p => new PreferenceEntity
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToList() ?? new List<PreferenceEntity>();
        }

        private class RedisPreference
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
