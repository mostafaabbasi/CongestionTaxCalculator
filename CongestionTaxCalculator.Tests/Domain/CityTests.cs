using CongestionTaxCalculator.Domain.Entities;
using FluentAssertions;

namespace CongestionTaxCalculator.Tests.Domain;

public class CityTests
{
    [Fact]
    public void Constructor_ShouldCreateCity_WithValidParameters()
    {
        var city = new City("Gothenburg", 60, 60, isJulyTollFree: true);

        city.Name.Should().Be("Gothenburg");
        city.MaxDailyFee.Should().Be(60);
        city.SingleChargeIntervalMinutes.Should().Be(60);
        city.IsJulyTollFree.Should().BeTrue();
    }

    [Theory]
    [InlineData("", 60, 60)]
    [InlineData("  ", 60, 60)]
    [InlineData(null!, 60, 60)]
    public void Constructor_ShouldThrowException_WhenNameIsInvalid(string? name, int maxDailyFee, int interval)
    {
        var act = () => new City(name, maxDailyFee, interval, true);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*City name cannot be empty*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenMaxDailyFeeIsZeroOrNegative()
    {
        var act = () => new City("Gothenburg", 0, 60, true);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Max daily fee must be positive*");
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenIntervalIsZeroOrNegative()
    {
        var act = () => new City("Gothenburg", 60, -1, true);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Single charge interval must be positive*");
    }

    [Fact]
    public void AddTollFeeSchedule_ShouldAddSchedule_WithValidParameters()
    {
        var city = new City("Gothenburg", 60, 60, true);
        var startTime = new TimeOnly(6, 0);
        var endTime = new TimeOnly(6, 29);

        city.AddTollFeeSchedule(startTime, endTime, 8);

        city.TollFeeSchedules.Should().HaveCount(1);
        city.TollFeeSchedules.First().Fee.Should().Be(8);
    }

    [Fact]
    public void AddTollFeeSchedule_ShouldThrowException_WhenFeeIsNegative()
    {
        var city = new City("Gothenburg", 60, 60, true);
        var startTime = new TimeOnly(6, 0);
        var endTime = new TimeOnly(6, 29);

        var act = () => city.AddTollFeeSchedule(startTime, endTime, -5);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Fee cannot be negative*");
    }

    [Fact]
    public void AddTollFeeSchedule_ShouldThrowException_WhenEndTimeIsBeforeStartTime()
    {
        var city = new City("Gothenburg", 60, 60, true);
        var startTime = new TimeOnly(6, 29);
        var endTime = new TimeOnly(6, 0);

        var act = () => city.AddTollFeeSchedule(startTime, endTime, 8);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*End time must be after start time*");
    }

    [Fact]
    public void AddTollFreeDate_ShouldAddDate_WithValidParameters()
    {
        var city = new City("Gothenburg", 60, 60, true);

        city.AddTollFreeDate(2025, 12, 25, false, "Christmas");

        city.TollFreeDates.Should().HaveCount(1);
        city.TollFreeDates.First().Description.Should().Be("Christmas");
    }

    [Theory]
    [InlineData(1899, 12, 25)]
    [InlineData(2101, 12, 25)]
    public void AddTollFreeDate_ShouldThrowException_WhenYearIsInvalid(int year, int month, int day)
    {
        var city = new City("Gothenburg", 60, 60, true);

        var act = () => city.AddTollFreeDate(year, month, day, false, "Holiday");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid year*");
    }

    [Theory]
    [InlineData(2025, 0, 25)]
    [InlineData(2025, 13, 25)]
    public void AddTollFreeDate_ShouldThrowException_WhenMonthIsInvalid(int year, int month, int day)
    {
        var city = new City("Gothenburg", 60, 60, true);

        var act = () => city.AddTollFreeDate(year, month, day, false, "Holiday");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid month*");
    }

    [Theory]
    [InlineData(2025, 12, 0)]
    [InlineData(2025, 12, 32)]
    public void AddTollFreeDate_ShouldThrowException_WhenDayIsInvalid(int year, int month, int day)
    {
        var city = new City("Gothenburg", 60, 60, true);

        var act = () => city.AddTollFreeDate(year, month, day, false, "Holiday");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid day*");
    }

    [Theory]
    [InlineData(2025, 11, 15)]
    [InlineData(2025, 11, 16)]
    public void IsTollFreeDate_ShouldReturnTrue_ForWeekends(int year, int month, int day)
    {
        var city = new City("Gothenburg", 60, 60, true);
        var date = new DateTime(year, month, day);


        var result = city.IsTollFreeDate(date);


        result.Should().BeTrue();
    }

    [Fact]
    public void IsTollFreeDate_ShouldReturnTrue_ForJulyWhenConfigured()
    {
        var city = new City("Gothenburg", 60, 60, isJulyTollFree: true);
        var date = new DateTime(2025, 7, 15);


        var result = city.IsTollFreeDate(date);


        result.Should().BeTrue();
    }

    [Fact]
    public void IsTollFreeDate_ShouldReturnFalse_ForJulyWhenNotConfigured()
    {
        var city = new City("Gothenburg", 60, 60, isJulyTollFree: false);
        var date = new DateTime(2025, 7, 15);


        var result = city.IsTollFreeDate(date);


        result.Should().BeFalse();
    }

    [Fact]
    public void IsTollFreeDate_ShouldReturnTrue_ForAddedHolidays()
    {
        var city = new City("Gothenburg", 60, 60, true);
        city.AddTollFreeDate(2025, 12, 25, false, "Christmas");
        var date = new DateTime(2025, 12, 25);


        var result = city.IsTollFreeDate(date);


        result.Should().BeTrue();
    }

    [Fact]
    public void GetTollFee_ShouldReturnZero_ForTollFreeDate()
    {
        var city = new City("Gothenburg", 60, 60, true);
        var date = new DateTime(2025, 11, 15);


        var fee = city.GetTollFee(date);


        fee.Should().Be(0);
    }

    [Fact]
    public void GetTollFee_ShouldReturnCorrectFee_ForScheduledTime()
    {
        var city = new City("Gothenburg", 60, 60, true);
        city.AddTollFeeSchedule(new TimeOnly(6, 0), new TimeOnly(6, 29), 8);
        var date = new DateTime(2025, 11, 13, 6, 15, 0);


        var fee = city.GetTollFee(date);


        fee.Should().Be(8);
    }

    [Fact]
    public void GetTollFee_ShouldReturnZero_WhenNoScheduleMatches()
    {
        var city = new City("Gothenburg", 60, 60, true);
        city.AddTollFeeSchedule(new TimeOnly(6, 0), new TimeOnly(6, 29), 8);
        var date = new DateTime(2025, 11, 13, 5, 59, 0);


        var fee = city.GetTollFee(date);


        fee.Should().Be(0);
    }

    [Fact]
    public void GetFeeReason_ShouldReturnWeekendReason_ForSaturday()
    {
        var city = new City("Gothenburg", 60, 60, true);
        var date = new DateTime(2025, 11, 15);


        var reason = city.GetFeeReason(date);


        reason.Should().Be("Weekend (toll-free)");
    }

    [Fact]
    public void GetFeeReason_ShouldReturnJulyReason_ForJulyDate()
    {
        var city = new City("Gothenburg", 60, 60, isJulyTollFree: true);
        var date = new DateTime(2025, 7, 15);


        var reason = city.GetFeeReason(date);


        reason.Should().Be("July (toll-free)");
    }

    [Fact]
    public void GetFeeReason_ShouldReturnHolidayReason_ForRegisteredHoliday()
    {
        var city = new City("Gothenburg", 60, 60, true);
        city.AddTollFreeDate(2025, 12, 25, false, "Christmas");
        var date = new DateTime(2025, 12, 25);


        var reason = city.GetFeeReason(date);


        reason.Should().Contain("Holiday: Christmas");
    }
}