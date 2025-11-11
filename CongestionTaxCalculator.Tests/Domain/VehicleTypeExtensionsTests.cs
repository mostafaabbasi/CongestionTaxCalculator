using CongestionTaxCalculator.Domain.Enums;
using FluentAssertions;

namespace CongestionTaxCalculator.Tests.Domain;

public class VehicleTypeExtensionsTests
{
    [Theory]
    [InlineData(VehicleType.Motorcycle)]
    [InlineData(VehicleType.Bus)]
    [InlineData(VehicleType.Tractor)]
    [InlineData(VehicleType.Emergency)]
    [InlineData(VehicleType.Diplomat)]
    [InlineData(VehicleType.Foreign)]
    [InlineData(VehicleType.Military)]
    public void IsTollFree_ShouldReturnTrue_ForTollFreeVehicles(VehicleType vehicleType)
    {
        var result = vehicleType.IsTollFree();


        result.Should().BeTrue();
    }

    [Fact]
    public void IsTollFree_ShouldReturnFalse_ForCar()
    {
        var vehicleType = VehicleType.Car;


        var result = vehicleType.IsTollFree();


        result.Should().BeFalse();
    }
}