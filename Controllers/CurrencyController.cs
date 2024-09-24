using CurrencyConverterAPI.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
// [Route("[controller]")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrencyController> _logger;

    public CurrencyController(ICurrencyService currencyService, ILogger<CurrencyController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert(string baseCurrency, string targetCurrency, decimal amount)
    {
        try
        {
            // Validate input parameters
            if (string.IsNullOrWhiteSpace(baseCurrency) || string.IsNullOrWhiteSpace(targetCurrency) || amount <= 0)
            {
                return BadRequest("Invalid input parameters. Please provide valid baseCurrency, targetCurrency, and a positive amount.");
            }

            // Attempt to convert the currency using the service
            var convertedAmount = await _currencyService.ConvertCurrency(baseCurrency, targetCurrency, amount);

            // Return the converted amount as a successful response
            return Ok(convertedAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while converting currency from {BaseCurrency} to {TargetCurrency} with amount {Amount}.", baseCurrency, targetCurrency, amount);

            // Return a user-friendly error message
            return StatusCode(500, "An unexpected error occurred while processing your request. Please try again later.");
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        try
        {
            // Fetch the conversion history from the service
            var history = await _currencyService.GetConversionHistory();

            // Return the conversion history as a successful response
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the conversion history.");

            // Return a user-friendly error message
            return StatusCode(500, "An unexpected error occurred while retrieving the conversion history. Please try again later.");
        }
    }
}
