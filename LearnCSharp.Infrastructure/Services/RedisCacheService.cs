using LearnCSharp.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace LearnCSharp.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _db;
        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _db=connectionMultiplexer.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var value = await _db.StringGetAsync(cacheKey);
            if(value.IsNullOrEmpty)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task RemoveAsync(string cacheKey)
        {
            await _db.KeyDeleteAsync(cacheKey);
        }

        public async Task SetAsyc<T>(string cacheKey, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(cacheKey, json, expiry ?? TimeSpan.FromMinutes(60)); 
        }
    }
}