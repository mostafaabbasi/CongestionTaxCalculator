using CongestionTaxCalculator.Domain.Enums;

namespace CongestionTaxCalculator.Features.Tax.Dtos.Requests;

public record CalculateTaxRequestDto(
    VehicleType VehicleType,
    string City,
    DateTime[] PassageTimes
);