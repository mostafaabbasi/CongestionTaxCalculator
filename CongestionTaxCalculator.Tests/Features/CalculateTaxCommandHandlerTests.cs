using CongestionTaxCalculator.Domain.Entities;
using CongestionTaxCalculator.Domain.Enums;
using CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;
using CongestionTaxCalculator.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CongestionTaxCalculator.Tests.Features;

public class CalculateTaxCommandHandlerTests : IDisposable
{
    private readonly CongestionTaxDbContext _context;
    private readonly CalculateTaxCommandHandler _handler;

    public CalculateTaxCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CongestionTaxDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new CongestionTaxDbContext(options);
        _handler = new CalculateTaxCommandHandler(_context);

        SeedTestData();
    }

    private void SeedTestData()
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

        _context.Cities.Add(city);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Handle_ShouldCalculateTax_ForValidRequest()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 11, 13, 7, 0, 0),
                new DateTime(2025, 11, 13, 12, 0, 0)
            ]
        );

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().NotBeNull();
        response.TotalTax.Should().Be(26);
        response.City.Should().Be("Gothenburg");
        response.VehicleType.Should().Be(VehicleType.Car);
        response.NumberOfPassages.Should().Be(2);
        response.Breakdowns.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_ForTollFreeVehicle()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Motorcycle,
            "Gothenburg",
            [
                new DateTime(2025, 11, 13, 7, 0, 0),
                new DateTime(2025, 11, 13, 12, 0, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(0);
        response.Breakdowns.Should().AllSatisfy(b => b.Fee.Should().Be(0));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenCityNotFound()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "NonExistentCity",
            [
                new DateTime(2025, 11, 13, 7, 0, 0)
            ]
        );


        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);


        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*City 'NonExistentCity' not found*");
    }

    [Fact]
    public async Task Handle_ShouldRespectMaxDailyFee()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 11, 13, 7, 0, 0),
                new DateTime(2025, 11, 13, 9, 0, 0),
                new DateTime(2025, 11, 13, 11, 0, 0),
                new DateTime(2025, 11, 13, 13, 0, 0),
                new DateTime(2025, 11, 13, 15, 0, 0),
                new DateTime(2025, 11, 13, 17, 0, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(60);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_ForWeekend()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 11, 15, 7, 0, 0),
                new DateTime(2025, 11, 15, 12, 0, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_ForJuly()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 7, 15, 7, 0, 0),
                new DateTime(2025, 7, 15, 12, 0, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnZero_ForHoliday()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 12, 25, 7, 0, 0),
                new DateTime(2025, 12, 25, 12, 0, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldIncludeBreakdowns_WithCorrectDetails()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            [
                new DateTime(2025, 11, 13, 7, 0, 0),
                new DateTime(2025, 11, 13, 7, 30, 0)
            ]
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.Breakdowns.Should().HaveCount(2);
        response.Breakdowns[0].PassageTime.Should().Be(new DateTime(2025, 11, 13, 7, 0, 0));
        response.Breakdowns[0].Fee.Should().Be(18);
        response.Breakdowns[1].PassageTime.Should().Be(new DateTime(2025, 11, 13, 7, 30, 0));
        response.Breakdowns[1].Fee.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldHandleEmptyPassages()
    {
        var command = new CalculateTaxCommand(
            VehicleType.Car,
            "Gothenburg",
            []
        );


        var response = await _handler.Handle(command, CancellationToken.None);


        response.TotalTax.Should().Be(0);
        response.NumberOfPassages.Should().Be(0);
        response.Breakdowns.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}