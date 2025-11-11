using CongestionTaxCalculator.Domain.Enums;
using CongestionTaxCalculator.Domain.ValueObjects;

namespace CongestionTaxCalculator.Domain.Entities;

public class CongestionTaxCalculation
{
    private readonly City _city;
    private readonly VehicleType _vehicleType;
    private readonly List<Passage> _passages = new();

    public CongestionTaxCalculation(City city, VehicleType vehicleType, IEnumerable<DateTime> passageTimes)
    {
        _city = city ?? throw new ArgumentNullException(nameof(city));
        _vehicleType = vehicleType;
        
        var sortedTimes = passageTimes.OrderBy(t => t).ToList();
        
        if (sortedTimes.Count == 0)
            return;

        ProcessPassages(sortedTimes);
    }

    private void ProcessPassages(List<DateTime> sortedTimes)
    {
        if (_vehicleType.IsTollFree())
        {
            foreach (var time in sortedTimes)
            {
                _passages.Add(new Passage(time, 0, "Toll-free vehicle"));
            }
            return;
        }

        var intervalStart = sortedTimes[0];
        var intervalMaxFee = _city.GetTollFee(intervalStart);
        var intervalMaxFeeTime = intervalStart;
        var intervalPassages = new List<(DateTime Time, int Fee, string Reason)>
        {
            (intervalStart, intervalMaxFee, _city.GetFeeReason(intervalStart))
        };

        for (var i = 1; i < sortedTimes.Count; i++)
        {
            var current = sortedTimes[i];
            var fee = _city.GetTollFee(current);
            var reason = _city.GetFeeReason(current);
            var minutesDiff = (current - intervalStart).TotalMinutes;

            if (minutesDiff <= _city.SingleChargeIntervalMinutes)
            {
                intervalPassages.Add((current, fee, reason));
                
                if (fee <= intervalMaxFee) continue;
                
                intervalMaxFee = fee;
                intervalMaxFeeTime = current;
            }
            else
            {
                FinalizeInterval(intervalPassages, intervalMaxFeeTime, intervalMaxFee);

                intervalStart = current;
                intervalMaxFee = fee;
                intervalMaxFeeTime = current;
                intervalPassages = new List<(DateTime, int, string)> { (current, fee, reason) };
            }
        }

        FinalizeInterval(intervalPassages, intervalMaxFeeTime, intervalMaxFee);
    }

    private void FinalizeInterval(
        List<(DateTime Time, int Fee, string Reason)> intervalPassages, 
        DateTime chargedTime, 
        int maxFee)
    {
        foreach (var passage in intervalPassages)
        {
            var isCharged = passage.Time == chargedTime;
            var reason = isCharged 
                ? passage.Reason 
                : $"Within {_city.SingleChargeIntervalMinutes}-min interval (max: {maxFee} SEK)";
            
            _passages.Add(new Passage(
                passage.Time,
                isCharged ? maxFee : 0,
                reason
            ));
        }
    }

    public int GetTotalTax()
    {
        var totalFee = _passages.Sum(p => p.Fee);
        return Math.Min(totalFee, _city.MaxDailyFee);
    }

    public IReadOnlyList<Passage> GetPassages() => _passages.AsReadOnly();
}
