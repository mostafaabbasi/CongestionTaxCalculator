using CongestionTaxCalculator.Domain.Entities;
using CongestionTaxCalculator.Domain.Enums;
using FluentAssertions;

namespace CongestionTaxCalculator.Tests.Domain;

public class CongestionTaxCalculationTests
{
    private City CreateTestCity()
    {
        var city = new City("Gothenburg", 60, 60, isJulyTollFree: true);

        city.AddTollFeeSchedule(new TimeOnly(6, 0), new TimeOnly(6, 29), 8);
        city.AddTollFeeSchedule(new TimeOnly(6, 30), new TimeOnly(6, 59), 13);
        city.AddTollFeeSchedule(new TimeOnly(7, 0), new TimeOnly(7, 59), 18);
        city.AddTollFeeSchedule(new TimeOnly(8, 0), new TimeOnly(8, 29), 13);
        city.AddTollFeeSchedule(new TimeOnly(8, 30), new TimeOnly(14, 59), 8);
        city.AddTollFeeSchedule(new TimeOnly(15, 0), new TimeOnly(15, 29), 13);
        city.AddTollFeeSchedule(new TimeOnly(15, 30), new TimeOnly(16, 59), 18);
        city.AddTollFeeSchedule(new TimeOnly(17, 0), new TimeOnly(17, 59), 13);
        city.AddTollFeeSchedule(new TimeOnly(18, 0), new TimeOnly(18, 29), 8);

        city.AddTollFreeDate(2025, 1, 1, false, "New Year");
        city.AddTollFreeDate(2025, 12, 25, false, "Christmas");

        return city;
    }

    [Fact]
    public void Constructor_ShouldHandleEmptyPassages()
    {
        var city = CreateTestCity();
        var passages = Array.Empty<DateTime>();


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(0);
        calculation.GetPassages().Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenCityIsNull()
    {
        City? city = null;
        var passages = new[] { new DateTime(2025, 11, 13, 8, 0, 0) };


        var act = () => new CongestionTaxCalculation(city!, VehicleType.Car, passages);


        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Calculate_ShouldReturnZero_ForTollFreeVehicle()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 8, 0, 0),
            new DateTime(2025, 11, 13, 10, 0, 0),
            new DateTime(2025, 11, 13, 16, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Motorcycle, passages);


        calculation.GetTotalTax().Should().Be(0);
        calculation.GetPassages().Should().AllSatisfy(p => p.Fee.Should().Be(0));
        calculation.GetPassages().Should().AllSatisfy(p => p.Reason.Should().Be("Toll-free vehicle"));
    }

    [Fact]
    public void Calculate_ShouldReturnCorrectFee_ForSinglePassage()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 7, 30, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(18);
        calculation.GetPassages().Should().HaveCount(1);
    }

    [Fact]
    public void Calculate_ShouldChargeMaxFee_WithinSingleChargeInterval()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 7, 0, 0),
            new DateTime(2025, 11, 13, 7, 30, 0),
            new DateTime(2025, 11, 13, 7, 59, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(18);
        var passagesList = calculation.GetPassages();
        passagesList.Should().HaveCount(3);
        passagesList.Count(p => p.Fee > 0).Should().Be(1);
    }

    [Fact]
    public void Calculate_ShouldChargeMultipleFees_WhenOutsideSingleChargeInterval()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 7, 0, 0),
            new DateTime(2025, 11, 13, 8, 30, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(26);
    }

    [Fact]
    public void Calculate_ShouldRespectMaxDailyFee()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 7, 0, 0),
            new DateTime(2025, 11, 13, 9, 0, 0),
            new DateTime(2025, 11, 13, 11, 0, 0),
            new DateTime(2025, 11, 13, 13, 0, 0),
            new DateTime(2025, 11, 13, 15, 0, 0),
            new DateTime(2025, 11, 13, 17, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(60);
    }

    [Fact]
    public void Calculate_ShouldSortPassages_WhenNotInOrder()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 17, 0, 0),
            new DateTime(2025, 11, 13, 7, 0, 0),
            new DateTime(2025, 11, 13, 12, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(39);
        var passagesList = calculation.GetPassages();
        passagesList[0].Time.Hour.Should().Be(7);
        passagesList[1].Time.Hour.Should().Be(12);
        passagesList[2].Time.Hour.Should().Be(17);
    }

    [Fact]
    public void Calculate_ShouldReturnZero_ForWeekend()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 15, 7, 0, 0),
            new DateTime(2025, 11, 15, 12, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(0);
    }

    [Fact]
    public void Calculate_ShouldReturnZero_ForJuly()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 7, 15, 7, 0, 0),
            new DateTime(2025, 7, 15, 12, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(0);
    }

    [Fact]
    public void Calculate_ShouldReturnZero_ForHoliday()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 12, 25, 7, 0, 0),
            new DateTime(2025, 12, 25, 12, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(0);
    }

    [Fact]
    public void Calculate_ShouldChargeHighestFee_InSingleChargeInterval()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 6, 15, 0),
            new DateTime(2025, 11, 13, 6, 45, 0),
            new DateTime(2025, 11, 13, 7, 10, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);


        calculation.GetTotalTax().Should().Be(18);
        var passagesList = calculation.GetPassages();
        passagesList.Should().HaveCount(3);
        passagesList.Single(p => p.Fee == 18).Time.Hour.Should().Be(7);
    }

    [Fact]
    public void GetPassages_ShouldReturnReadOnlyList()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 7, 0, 0)
        };


        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);
        var result = calculation.GetPassages();


        result.Should().BeAssignableTo<IReadOnlyList<CongestionTaxCalculator.Domain.ValueObjects.Passage>>();
    }

    [Fact]
    public void Calculate_ComplexScenario_MultipleIntervalsAndMaxFee()
    {
        var city = CreateTestCity();
        var passages = new[]
        {
            new DateTime(2025, 11, 13, 6, 0, 0),
            new DateTime(2025, 11, 13, 6, 30, 0),
            new DateTime(2025, 11, 13, 8, 0, 0),
            new DateTime(2025, 11, 13, 9, 30, 0),
            new DateTime(2025, 11, 13, 15, 0, 0),
            new DateTime(2025, 11, 13, 16, 0, 0),
            new DateTime(2025, 11, 13, 17, 30, 0)
        };
        
        var calculation = new CongestionTaxCalculation(city, VehicleType.Car, passages);
        
        calculation.GetTotalTax().Should().Be(60);
    }
}