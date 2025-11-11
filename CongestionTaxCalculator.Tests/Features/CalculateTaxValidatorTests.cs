using CongestionTaxCalculator.Domain.Enums;
using CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;
using CongestionTaxCalculator.Features.Tax.Dtos.Requests;
using FluentValidation.TestHelper;

namespace CongestionTaxCalculator.Tests.Features;

public class CalculateTaxValidatorTests
{
    private readonly CalculateTaxValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WithValidDto()
    {
        var dto = new CalculateTaxRequestDto(
            VehicleType.Car,
            "Gothenburg",
            [new DateTime(2025, 11, 13, 7, 0, 0)]
        );


        var result = _validator.TestValidate(dto);


        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData(null!)]
    public void Validate_ShouldFail_WhenCityIsEmpty(string? city)
    {
        var dto = new CalculateTaxRequestDto(
            VehicleType.Car,
            city!,
            [new DateTime(2025, 11, 13, 7, 0, 0)]
        );

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.City);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPassageTimesIsEmpty()
    {
        var dto = new CalculateTaxRequestDto(
            VehicleType.Car,
            "Gothenburg",
            []
        );

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.PassageTimes);
    }

    [Fact]
    public void Validate_ShouldPass_WithMultiplePassages()
    {
        var dto = new CalculateTaxRequestDto(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 11, 13, 7, 0, 0),
                new DateTime(2025, 11, 13, 12, 0, 0),
                new DateTime(2025, 11, 13, 17, 0, 0)
            ]
        );

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(VehicleType.Car)]
    [InlineData(VehicleType.Motorcycle)]
    [InlineData(VehicleType.Bus)]
    [InlineData(VehicleType.Tractor)]
    [InlineData(VehicleType.Emergency)]
    [InlineData(VehicleType.Diplomat)]
    [InlineData(VehicleType.Foreign)]
    [InlineData(VehicleType.Military)]
    public void Validate_ShouldPass_WithAllVehicleTypes(VehicleType vehicleType)
    {
        var dto = new CalculateTaxRequestDto(
            vehicleType,
            "Gothenburg",
            [new DateTime(2025, 11, 13, 7, 0, 0)]
        );

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPassageTimesContainDefaultDate()
    {
        var dto = new CalculateTaxRequestDto(
            VehicleType.Car,
            "Gothenburg",
            [default(DateTime), new DateTime(2025, 11, 13, 7, 0, 0)]
        );

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(d => d.PassageTimes);
    }
}