using CongestionTaxCalculator.Domain.Enums;

namespace CongestionTaxCalculator.Features.Tax.Dtos.Responses;

public sealed record CalculateTaxResponseDto(
    int TotalTax,
    string City,
    VehicleType VehicleType,
    int NumberOfPassages,
    TaxBreakdownDto[] Breakdowns);
    
public sealed record TaxBreakdownDto(
    DateTime PassageTime,
    int Fee,
    string Reason);