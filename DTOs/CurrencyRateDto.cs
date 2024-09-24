using System.ComponentModel.DataAnnotations;

namespace CurrencyConverterAPI.Dtos;

public record class CreateCurrencyRateDto(
    [Required][StringLength(50)] string BaseCurrency,
    [Required][StringLength(50)] string TargetCurrency,
    [Required][Range(1, 100)] Decimal Rate,
    DateOnly Date
);

public record class CurrencyRateDetailsDto(
    int Id,
    string BaseCurrency,
    string TargetCurrency,
    Decimal Rate,
    DateOnly Date
);

public record class UpdateCurrencyRateDto(
    [Required][StringLength(50)] string BaseCurrency,
    [Required][StringLength(50)] string TargetCurrency,
    [Required][Range(1, 100)] Decimal Rate,
    DateOnly Date
);