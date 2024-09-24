using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;
using CurrencyConverterAPI.Repositories;

public class CurrencyServiceTests
{
    private readonly Mock<ICurrencyRepository> _currencyRepositoryMock;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly CurrencyService _currencyService;
    Mock<IHttpClientFactory> _httpClientFactoryMock;
    Mock<ILogger<CurrencyService>> _loggerMock;

    public CurrencyServiceTests()
    {
        _currencyRepositoryMock = new Mock<ICurrencyRepository>();
        _distributedCacheMock = new Mock<IDistributedCache>();
        _currencyService = new CurrencyService(
            _currencyRepositoryMock.Object,
            _distributedCacheMock.Object,
            _httpClientFactoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ConvertCurrency_ShouldReturnCachedValue_WhenCacheIsNotExpired()
    {
        // Arrange
        var baseCurrency = "USD";
        var targetCurrency = "EUR";
        var amount = 100;
        var cachedRate = 0.85M;
        var cacheKey = $"{baseCurrency}-{targetCurrency}";

        var cacheData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cachedRate));
        _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, default)).ReturnsAsync(cacheData);

        // Act
        var result = await _currencyService.ConvertCurrency(baseCurrency, targetCurrency, amount);

        // Assert
        Assert.Equal(85, result);
        _distributedCacheMock.Verify(x => x.GetAsync(cacheKey, default), Times.Once);
        _currencyRepositoryMock.Verify(x => x.SaveCurrencyRateAsync(It.IsAny<CurrencyRate>()), Times.Never);
    }

    [Fact]
    public async Task ConvertCurrency_ShouldFetchFromAPIAndCache_WhenCacheIsExpired()
    {
        // Arrange
        var baseCurrency = "USD";
        var targetCurrency = "EUR";
        var amount = 100;
        var rateFromApi = 0.9M;
        var cacheKey = $"{baseCurrency}-{targetCurrency}";

        // Cache miss
        _distributedCacheMock.Setup(x => x.GetAsync(cacheKey, default)).ReturnsAsync((byte[])null);

        // Mock repository to simulate fetching data from the external API
        _currencyRepositoryMock.Setup(x => x.SaveCurrencyRateAsync(It.IsAny<CurrencyRate>())).Returns(Task.CompletedTask);

        // Act
        var result = await _currencyService.ConvertCurrency(baseCurrency, targetCurrency, amount);

        // Assert
        Assert.Equal(90, result);
        _distributedCacheMock.Verify(x => x.GetAsync(cacheKey, default), Times.Once);
        _distributedCacheMock.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
    }

    [Fact]
    public async Task GetConversionHistory_ShouldReturnListOfHistory()
    {
        // Arrange
        var mockHistory = new List<ConversionHistory>
        {
            new ConversionHistory
            {
                Id = 1,
                BaseCurrency = "USD",
                TargetCurrency = "EUR",
                Amount = 100,
                ConvertedAmount = 85,
                Date = DateOnly.FromDateTime(DateTime.UtcNow)
            }
        };

        _currencyRepositoryMock.Setup(x => x.GetConversionHistory()).ReturnsAsync(mockHistory);

        // Act
        var result = await _currencyService.GetConversionHistory();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("USD", result[0].BaseCurrency);
        Assert.Equal("EUR", result[0].TargetCurrency);
        _currencyRepositoryMock.Verify(x => x.GetConversionHistory(), Times.Once);
    }
}
