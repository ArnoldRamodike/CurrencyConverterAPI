using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Repositories
{
    public interface ICurrencyRepository
    {
        Task SaveCurrencyRateAsync(CurrencyRate currencyRate);
        Task SaveConversionHistoryAsync(ConversionHistory conversionHistory);
        Task<List<ConversionHistory>> GetConversionHistory();
    }
}
