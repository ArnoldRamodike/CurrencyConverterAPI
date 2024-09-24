using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Models
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Rate { get; set; }
        public DateOnly Date { get; set; }
    }
}