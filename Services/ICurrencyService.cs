using System;
using CurrencyConverterAPI.Models;

namespace CurrencyConverterAPI.Services;

public interface ICurrencyService
{
    Task<decimal> ConvertCurrency(string baseCurrency, string targetCurrency, decimal amount);
    Task<List<ConversionHistory>> GetConversionHistory();
}