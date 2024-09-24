using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Models
{
    public class ConversionHistory
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateOnly Date { get; set; }
    }
}