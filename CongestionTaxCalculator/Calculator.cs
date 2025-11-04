namespace CongestionTaxCalculator;

public class Calculator
{
    private const int MaxDailyFee = 60;
    private const int SingleChargeIntervalMinutes = 60;

    public int GetTax(Vehicle vehicle, DateTime[] dates)
    {
        if (dates.Length == 0 || IsTollFreeVehicle(vehicle))
            return 0;

        var sortedDates = (DateTime[])dates.Clone();
        Array.Sort(sortedDates);

        var totalFee = 0;
        var intervalStart = sortedDates[0];
        var intervalMaxFee = GetTollFee(intervalStart, vehicle);

        for (var i = 1; i < sortedDates.Length; i++)
        {
            var current = sortedDates[i];
            var fee = GetTollFee(current, vehicle);

            var minutesDiff = (current - intervalStart).TotalMinutes;

            if (minutesDiff <= SingleChargeIntervalMinutes)
            {
                intervalMaxFee = Math.Max(intervalMaxFee, fee);
            }
            else
            {
                totalFee += intervalMaxFee;
                intervalStart = current;
                intervalMaxFee = fee;
            }
        }

        totalFee += intervalMaxFee;

        return Math.Min(totalFee, MaxDailyFee);
    }

    private bool IsTollFreeVehicle(Vehicle vehicle)
    {
        var type = vehicle.GetVehicleType();

        if (!Enum.TryParse(type, out TollFreeVehicles vehicleType)) return false;

        var tollFreeVehicles = Enum.GetValues<TollFreeVehicles>();
        return tollFreeVehicles.Contains(vehicleType);
    }

    private int GetTollFee(DateTime date, Vehicle vehicle)
    {
        if (IsTollFreeDate(date) || IsTollFreeVehicle(vehicle))
            return 0;

        var hour = date.Hour;
        var minute = date.Minute;

        switch (hour)
        {
            case 6 when minute <= 29:
                return 8;
            case 6 when minute <= 59:
                return 13;
            case 7:
                return 18;
            case 8 when minute <= 29:
                return 13;
            case 8 when minute >= 30:
            case >= 9 and <= 14:
                return 8;
            case 15 when minute <= 29:
                return 13;
            case 15 when minute >= 30:
            case 16:
                return 18;
            case 17:
                return 13;
            case 18 when minute <= 29:
                return 8;
            default:
                return 0;
        }
    }

    private bool IsTollFreeDate(DateTime date)
    {
        if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return true;

        if (date.Month == 7) return true;

        var day = date.Day;
        var month = date.Month;

        if (date.Year != 2013) return false;

        if ((month == 1 && day == 1) ||
            (month == 3 && day is 28 or 29) ||
            (month == 4 && day is 1 or 30) ||
            (month == 5 && day is 1 or 8 or 9) ||
            (month == 6 && day is 5 or 6 or 21) ||
            (month == 11 && day == 1) ||
            (month == 12 && day is 24 or 25 or 26 or 31))
        {
            return true;
        }

        var nextDay = date.AddDays(1);
        
        return nextDay is { Month: 1, Day: 1 } or
            { Month: 3, Day: 28 or 29 } or 
            { Month: 4, Day: 1 or 30 } or
            { Month: 5, Day: 1 or 8 or 9 } or 
            { Month: 6, Day: 5 or 6 or 21 } or 
            { Month: 11, Day: 1 } or
            { Month: 12, Day: 24 or 25 or 26 or 31 };
    }
}