using LearnCSharp.Application.Interfaces;
using System.Text.Json;

namespace LearnCSharp.Infrastructure.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertVndToUsdAsync(decimal amountVnd)
        {
            var response = await _httpClient.GetAsync("https://open.er-api.com/v6/latest/USD");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            decimal vndRate = document.RootElement.GetProperty("rates").GetProperty("VND").GetDecimal();

            return Math.Round(amountVnd / vndRate, 2);
        }
    }
}