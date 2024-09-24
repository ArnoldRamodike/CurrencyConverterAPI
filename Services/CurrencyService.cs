using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Repositories;
using CurrencyConverterAPI.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

public class CurrencyService : ICurrencyService
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CurrencyService> _logger;
    private const string CacheKeyPrefix = "CurrencyRate_";

    public CurrencyService(
        ICurrencyRepository currencyRepository,
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        ILogger<CurrencyService> logger)
    {
        _currencyRepository = currencyRepository;
        _cache = cache;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public async Task<decimal> ConvertCurrency(string baseCurrency, string targetCurrency, decimal amount)
    {
        try
        {
            // Construct the cache key
            var cacheKey = $"{CacheKeyPrefix}{baseCurrency}_{targetCurrency}";

            // Check Redis cache for the rate
            var cachedRate = await _cache.GetStringAsync(cacheKey);
            decimal rate;

            if (!string.IsNullOrEmpty(cachedRate))
            {
                rate = decimal.Parse(cachedRate);
                _logger.LogInformation($"Rate fetched from Redis cache: {rate}");
            }
            else
            {
                // Fetch the rate from the external API
                rate = await FetchRateFromExternalAPI(baseCurrency, targetCurrency);

                // Store the rate in Redis with a 15-minute expiration
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
                };
                await _cache.SetStringAsync(cacheKey, rate.ToString(), cacheOptions);

                // Save the rate to the MySQL database
                var currencyRate = new CurrencyRate
                {
                    BaseCurrency = baseCurrency,
                    TargetCurrency = targetCurrency,
                    Rate = rate,
                    Date = DateOnly.FromDateTime(DateTime.UtcNow)
                };
                await _currencyRepository.SaveCurrencyRateAsync(currencyRate);

                _logger.LogInformation($"Rate fetched from external API and stored in MySQL and Redis: {rate}");
            }

            // Calculate the converted amount
            var convertedAmount = rate * amount;

            // Save the conversion history
            var conversionHistory = new ConversionHistory
            {
                BaseCurrency = baseCurrency,
                TargetCurrency = targetCurrency,
                Amount = amount,
                ConvertedAmount = convertedAmount,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _currencyRepository.SaveConversionHistoryAsync(conversionHistory);

            return convertedAmount;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in converting currency: {ex.Message}");
            throw new ApplicationException("An error occurred while converting the currency. Please try again later.");
        }
    }

    public async Task<List<ConversionHistory>> GetConversionHistory()
    {
        try
        {
            return await _currencyRepository.GetConversionHistory();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching conversion history: {ex.Message}");
            throw new ApplicationException("An error occurred while fetching the conversion history.");
        }
    }

    private async Task<decimal> FetchRateFromExternalAPI(string baseCurrency, string targetCurrency)
    {
        try
        {
            var apiUrl = $"https://api.exchangerate-api.com/v4/latest/{baseCurrency}"; // Example API endpoint
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(content);

                if (jsonDocument.RootElement.TryGetProperty("rates", out JsonElement ratesElement) &&
                    ratesElement.TryGetProperty(targetCurrency, out JsonElement rateElement))
                {
                    return rateElement.GetDecimal();
                }

                throw new Exception("Rate not found for the target currency.");
            }

            throw new Exception($"Failed to fetch rate from external API. Status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching rate from external API: {ex.Message}");
            throw new ApplicationException("An error occurred while fetching the currency rate. Please try again later.");
        }
    }
}
