using System.ComponentModel.DataAnnotations;

namespace CurrencyConverterAPI.Dtos;

public record class CreateConversionHistoryDto(
    [Required][StringLength(50)] string BaseCurrency,
    [Required][StringLength(50)] string TargetCurrency,
    [Required] Decimal Amount,
    [Required] Decimal ConversionAmount,
    DateOnly Date
);

public record class ConversionHistoryDetailsDto(
    int Id,
    string BaseCurrency,
    string TargetCurrency,
    Decimal Rate,
    DateOnly Date
);

public record class UpdateConversionHistoryDto(
    [Required][StringLength(50)] string BaseCurrency,
    [Required][StringLength(50)] string TargetCurrency,
    [Required] Decimal Amount,
    [Required] Decimal ConversionAmount,
    DateOnly Date
);