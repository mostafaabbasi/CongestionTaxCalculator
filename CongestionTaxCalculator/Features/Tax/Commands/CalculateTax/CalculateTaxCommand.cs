using CongestionTaxCalculator.Domain.Enums;
using MediatR;

namespace CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;

public sealed record CalculateTaxCommand(
    VehicleType VehicleType,
    string City,
    DateTime[] PassageTimes) : IRequest<CalculateTaxResponse>;