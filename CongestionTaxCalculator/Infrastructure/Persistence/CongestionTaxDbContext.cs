using CongestionTaxCalculator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CongestionTaxCalculator.Infrastructure.Persistence;

public sealed class CongestionTaxDbContext(DbContextOptions<CongestionTaxDbContext> options) : DbContext(options)
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<TollFeeSchedule> TollFeeSchedules => Set<TollFeeSchedule>();
    public DbSet<TollFreeDate> TollFreeDates => Set<TollFreeDate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<TollFeeSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StartTime).IsRequired();
            entity.Property(e => e.EndTime).IsRequired();
            entity.Property(e => e.Fee).IsRequired();
        });

        modelBuilder.Entity<TollFreeDate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Year).IsRequired();
            entity.Property(e => e.Month).IsRequired();
            entity.Property(e => e.Day).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<City>().HasData(
            new
            {
                Id = 1,
                Name = "Gothenburg",
                MaxDailyFee = 60,
                SingleChargeIntervalMinutes = 60,
                IsJulyTollFree = true
            }
        );

        modelBuilder.Entity<TollFeeSchedule>().HasData(
            new { Id = 1, CityId = 1, StartTime = new TimeOnly(6, 0), EndTime = new TimeOnly(6, 29), Fee = 8 },
            new { Id = 2, CityId = 1, StartTime = new TimeOnly(6, 30), EndTime = new TimeOnly(6, 59), Fee = 13 },
            new { Id = 3, CityId = 1, StartTime = new TimeOnly(7, 0), EndTime = new TimeOnly(7, 59), Fee = 18 },
            new { Id = 4, CityId = 1, StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 29), Fee = 13 },
            new { Id = 5, CityId = 1, StartTime = new TimeOnly(8, 30), EndTime = new TimeOnly(14, 59), Fee = 8 },
            new { Id = 6, CityId = 1, StartTime = new TimeOnly(15, 0), EndTime = new TimeOnly(15, 29), Fee = 13 },
            new { Id = 7, CityId = 1, StartTime = new TimeOnly(15, 30), EndTime = new TimeOnly(16, 59), Fee = 18 },
            new { Id = 8, CityId = 1, StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(17, 59), Fee = 13 },
            new { Id = 9, CityId = 1, StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(18, 29), Fee = 8 }
        );

        modelBuilder.Entity<TollFreeDate>().HasData(
            new { Id = 1, CityId = 1, Year = 2013, Month = 1, Day = 1, IsDayBeforeHoliday = false, Description = "New Year's Day" },
            new { Id = 2, CityId = 1, Year = 2013, Month = 3, Day = 28, IsDayBeforeHoliday = false, Description = "Maundy Thursday" },
            new { Id = 3, CityId = 1, Year = 2013, Month = 3, Day = 29, IsDayBeforeHoliday = false, Description = "Good Friday" },
            new { Id = 4, CityId = 1, Year = 2013, Month = 4, Day = 1, IsDayBeforeHoliday = false, Description = "Easter Monday" },
            new { Id = 5, CityId = 1, Year = 2013, Month = 4, Day = 30, IsDayBeforeHoliday = false, Description = "Day before May 1st" },
            new { Id = 6, CityId = 1, Year = 2013, Month = 5, Day = 1, IsDayBeforeHoliday = false, Description = "May Day" },
            new { Id = 7, CityId = 1, Year = 2013, Month = 5, Day = 8, IsDayBeforeHoliday = false, Description = "Ascension Day Eve" },
            new { Id = 8, CityId = 1, Year = 2013, Month = 5, Day = 9, IsDayBeforeHoliday = false, Description = "Ascension Day" },
            new { Id = 9, CityId = 1, Year = 2013, Month = 6, Day = 5, IsDayBeforeHoliday = false, Description = "National Day Eve" },
            new { Id = 10, CityId = 1, Year = 2013, Month = 6, Day = 6, IsDayBeforeHoliday = false, Description = "National Day" },
            new { Id = 11, CityId = 1, Year = 2013, Month = 6, Day = 21, IsDayBeforeHoliday = false, Description = "Midsummer Eve" },
            new { Id = 12, CityId = 1, Year = 2013, Month = 11, Day = 1, IsDayBeforeHoliday = false, Description = "All Saints' Day" },
            new { Id = 13, CityId = 1, Year = 2013, Month = 12, Day = 24, IsDayBeforeHoliday = false, Description = "Christmas Eve" },
            new { Id = 14, CityId = 1, Year = 2013, Month = 12, Day = 25, IsDayBeforeHoliday = false, Description = "Christmas Day" },
            new { Id = 15, CityId = 1, Year = 2013, Month = 12, Day = 26, IsDayBeforeHoliday = false, Description = "Boxing Day" },
            new { Id = 16, CityId = 1, Year = 2013, Month = 12, Day = 31, IsDayBeforeHoliday = false, Description = "New Year's Eve" },
            new { Id = 17, CityId = 1, Year = 2012, Month = 12, Day = 31, IsDayBeforeHoliday = true, Description = "Day before New Year" },
            new { Id = 18, CityId = 1, Year = 2013, Month = 3, Day = 27, IsDayBeforeHoliday = true, Description = "Day before Maundy Thursday" },
            new { Id = 19, CityId = 1, Year = 2013, Month = 3, Day = 31, IsDayBeforeHoliday = true, Description = "Day before Easter Monday" },
            new { Id = 20, CityId = 1, Year = 2013, Month = 4, Day = 29, IsDayBeforeHoliday = true, Description = "Day before April 30" },
            new { Id = 21, CityId = 1, Year = 2013, Month = 5, Day = 7, IsDayBeforeHoliday = true, Description = "Day before May 8" },
            new { Id = 22, CityId = 1, Year = 2013, Month = 6, Day = 4, IsDayBeforeHoliday = true, Description = "Day before National Day Eve" },
            new { Id = 23, CityId = 1, Year = 2013, Month = 6, Day = 20, IsDayBeforeHoliday = true, Description = "Day before Midsummer Eve" },
            new { Id = 24, CityId = 1, Year = 2013, Month = 10, Day = 31, IsDayBeforeHoliday = true, Description = "Day before All Saints" },
            new { Id = 25, CityId = 1, Year = 2013, Month = 12, Day = 23, IsDayBeforeHoliday = true, Description = "Day before Christmas Eve" },
            new { Id = 26, CityId = 1, Year = 2013, Month = 12, Day = 30, IsDayBeforeHoliday = true, Description = "Day before New Year's Eve" }
        );
    }
}
