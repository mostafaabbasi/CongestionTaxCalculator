using System.Text.Json.Serialization;

namespace CongestionTaxCalculator.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VehicleType
{
    Car,
    Motorcycle,
    Bus,
    Tractor,
    Emergency,
    Diplomat,
    Foreign,
    Military
}

public static class VehicleTypeExtensions
{
    private static readonly HashSet<VehicleType> TollFreeVehicles = new()
    {
        VehicleType.Motorcycle,
        VehicleType.Bus,
        VehicleType.Tractor,
        VehicleType.Emergency,
        VehicleType.Diplomat,
        VehicleType.Foreign,
        VehicleType.Military
    };

    public static bool IsTollFree(this VehicleType vehicleType)
    {
        return TollFreeVehicles.Contains(vehicleType);
    }
}
