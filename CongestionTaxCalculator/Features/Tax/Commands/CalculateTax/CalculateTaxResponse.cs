using CongestionTaxCalculator.Domain.Enums;

namespace CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;

public sealed record CalculateTaxResponse(
    int TotalTax,
    string City,
    VehicleType VehicleType,
    int NumberOfPassages,
    TaxBreakdown[] Breakdowns);
    
public sealed record TaxBreakdown(
    DateTime PassageTime,
    int Fee,
    string Reason);