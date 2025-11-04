namespace CongestionTaxCalculator;

public sealed class Motorcycle : Vehicle
{
    public override string GetVehicleType()
        => nameof(TollFreeVehicles.Motorcycle);
}