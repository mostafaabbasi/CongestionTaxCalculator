namespace CongestionTaxCalculator.Domain.Entities;

public class TollFreeDate
{
    public int Id { get; private set; }
    public int CityId { get; private set; }
    public int Year { get; private set; }
    public int Month { get; private set; }
    public int Day { get; private set; }
    public bool IsDayBeforeHoliday { get; private set; }
    public string? Description { get; private set; }
    
    public City City { get; private set; } = null!;

    private TollFreeDate() { }

    public TollFreeDate(int cityId, int year, int month, int day, bool isDayBeforeHoliday, string? description)
    {
        if (year < 1900 || year > 2100)
            throw new ArgumentException("Invalid year", nameof(year));
        
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month", nameof(month));
        
        if (day < 1 || day > 31)
            throw new ArgumentException("Invalid day", nameof(day));

        CityId = cityId;
        Year = year;
        Month = month;
        Day = day;
        IsDayBeforeHoliday = isDayBeforeHoliday;
        Description = description;
    }
}
