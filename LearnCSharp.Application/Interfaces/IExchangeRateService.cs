namespace LearnCSharp.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<decimal> ConvertVndToUsdAsync(decimal amountVnd);
    }
}