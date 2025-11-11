namespace CongestionTaxCalculator.Domain.Entities;

public class TollFeeSchedule
{
    public int Id { get; private set; }
    public int CityId { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int Fee { get; private set; }
    
    public City City { get; private set; } = null!;

    private TollFeeSchedule() { }

    public TollFeeSchedule(int cityId, TimeOnly startTime, TimeOnly endTime, int fee)
    {
        if (fee < 0)
            throw new ArgumentException("Fee cannot be negative", nameof(fee));
        
        if (endTime <= startTime)
            throw new ArgumentException("End time must be after start time");

        CityId = cityId;
        StartTime = startTime;
        EndTime = endTime;
        Fee = fee;
    }
}
