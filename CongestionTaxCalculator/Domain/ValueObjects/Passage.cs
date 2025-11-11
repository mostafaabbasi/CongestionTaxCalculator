namespace CongestionTaxCalculator.Domain.ValueObjects;

public record Passage(
    DateTime Time,
    int Fee,
    string Reason
);
