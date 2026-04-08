namespace LearnCSharp.Application.Interfaces
{
    public interface IRedisCacheService
    {
        Task<T> GetAsync<T>(string cacheKey);
        Task SetAsyc<T>(string cacheKey, T value, TimeSpan? expiry = null);
        Task RemoveAsync(string cacheKey);
    }
}