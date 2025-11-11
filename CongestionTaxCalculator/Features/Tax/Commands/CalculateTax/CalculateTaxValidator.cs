using CongestionTaxCalculator.Features.Tax.Dtos.Requests;
using FluentValidation;

namespace CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;

public sealed class CalculateTaxValidator : AbstractValidator<CalculateTaxRequestDto>
{
    public CalculateTaxValidator()
    {
        RuleFor(x => x.VehicleType)
            .IsInEnum()
            .WithMessage("Vehicle type must be a valid vehicle type");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required");

        RuleFor(x => x.PassageTimes)
            .NotEmpty()
            .WithMessage("At least one passage time is required")
            .Must(times => times.All(t => t != default))
            .WithMessage("All passage times must be valid dates");
    }
}
