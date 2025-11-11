namespace CongestionTaxCalculator.Domain.Entities;

public class City
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int MaxDailyFee { get; private set; }
    public int SingleChargeIntervalMinutes { get; private set; }
    public bool IsJulyTollFree { get; private set; }
    
    private readonly List<TollFeeSchedule> _tollFeeSchedules = new();
    private readonly List<TollFreeDate> _tollFreeDates = new();
    
    public IReadOnlyCollection<TollFeeSchedule> TollFeeSchedules => _tollFeeSchedules.AsReadOnly();
    public IReadOnlyCollection<TollFreeDate> TollFreeDates => _tollFreeDates.AsReadOnly();

    private City() { Name = string.Empty; }

    public City(string name, int maxDailyFee, int singleChargeIntervalMinutes, bool isJulyTollFree)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be empty", nameof(name));
        
        if (maxDailyFee <= 0)
            throw new ArgumentException("Max daily fee must be positive", nameof(maxDailyFee));
        
        if (singleChargeIntervalMinutes <= 0)
            throw new ArgumentException("Single charge interval must be positive", nameof(singleChargeIntervalMinutes));

        Name = name;
        MaxDailyFee = maxDailyFee;
        SingleChargeIntervalMinutes = singleChargeIntervalMinutes;
        IsJulyTollFree = isJulyTollFree;
    }

    public void AddTollFeeSchedule(TimeOnly startTime, TimeOnly endTime, int fee)
    {
        if (fee < 0)
            throw new ArgumentException("Fee cannot be negative", nameof(fee));
        
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        _tollFeeSchedules.Add(new TollFeeSchedule(Id, startTime, endTime, fee));
    }

    public void AddTollFreeDate(int year, int month, int day, bool isDayBeforeHoliday, string? description)
    {
        if (year < 1900 || year > 2100)
            throw new ArgumentException("Invalid year", nameof(year));
        
        if (month < 1 || month > 12)
            throw new ArgumentException("Invalid month", nameof(month));
        
        if (day < 1 || day > 31)
            throw new ArgumentException("Invalid day", nameof(day));

        _tollFreeDates.Add(new TollFreeDate(Id, year, month, day, isDayBeforeHoliday, description));
    }
    
    public bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return true;

        if (IsJulyTollFree && date.Month == 7)
            return true;

        return _tollFreeDates.Any(d =>
            d.Year == date.Year && d.Month == date.Month && d.Day == date.Day);
    }

    public int GetTollFee(DateTime dateTime)
    {
        if (IsTollFreeDate(dateTime))
            return 0;

        var time = TimeOnly.FromDateTime(dateTime);
        var schedule = _tollFeeSchedules
            .FirstOrDefault(s => time >= s.StartTime && time <= s.EndTime);

        return schedule?.Fee ?? 0;
    }

    public string GetFeeReason(DateTime dateTime)
    {
        if (dateTime.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return "Weekend (toll-free)";

        if (IsJulyTollFree && dateTime.Month == 7)
            return "July (toll-free)";

        var tollFreeDate = _tollFreeDates.FirstOrDefault(d =>
            d.Year == dateTime.Year && d.Month == dateTime.Month && d.Day == dateTime.Day);
        
        if (tollFreeDate != null)
            return $"Holiday: {tollFreeDate.Description}";

        var time = TimeOnly.FromDateTime(dateTime);
        var schedule = _tollFeeSchedules
            .FirstOrDefault(s => time >= s.StartTime && time <= s.EndTime);

        return schedule != null
            ? $"Peak hour ({schedule.StartTime:HH:mm}-{schedule.EndTime:HH:mm}): {schedule.Fee} SEK"
            : "Outside toll hours (toll-free)";
    }
}
