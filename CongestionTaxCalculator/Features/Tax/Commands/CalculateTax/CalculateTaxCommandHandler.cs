using CongestionTaxCalculator.Domain.Entities;
using CongestionTaxCalculator.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;

internal sealed class CalculateTaxCommandHandler(CongestionTaxDbContext dbContext) : IRequestHandler<CalculateTaxCommand, CalculateTaxResponse>
{
    public async Task<CalculateTaxResponse> Handle(CalculateTaxCommand request, CancellationToken cancellationToken)
    {
        var city = await dbContext.Cities
                       .Include(c => c.TollFeeSchedules)
                       .Include(c => c.TollFreeDates)
                       .FirstOrDefaultAsync(c => c.Name == request.City, cancellationToken)
                   ?? throw new InvalidOperationException($"City '{request.City}' not found");

        var calculation = new CongestionTaxCalculation(city, request.VehicleType, request.PassageTimes);
        
        var passages = calculation.GetPassages();
        var breakdowns = passages.Select(p => new TaxBreakdown(
            PassageTime: p.Time,
            Fee: p.Fee,
            Reason: p.Reason
        )).ToArray();

        return new CalculateTaxResponse(
            TotalTax: calculation.GetTotalTax(),
            City: request.City,
            VehicleType: request.VehicleType,
            NumberOfPassages: request.PassageTimes.Length,
            Breakdowns: breakdowns
        );
    }
}