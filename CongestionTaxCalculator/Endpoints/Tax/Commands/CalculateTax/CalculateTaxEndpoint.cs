using Asp.Versioning;
using CongestionTaxCalculator.Features.Tax.Commands.CalculateTax;
using CongestionTaxCalculator.Features.Tax.Dtos.Requests;
using CongestionTaxCalculator.Features.Tax.Dtos.Responses;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CongestionTaxCalculator.Endpoints.Tax.Commands.CalculateTax;

public static class CalculateTaxEndpoint
{
    public static IEndpointRouteBuilder MapCalculateTaxEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var apiV1 = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        endpoints.MapPost("/api/v{version:apiVersion}/congestion-tax/calculate", CalculateTax)
            .WithName("CalculateCongestionTax")
            .WithTags("Congestion Tax")
            .WithSummary("Calculate congestion tax")
            .WithDescription(
                "Calculates the total congestion tax for a vehicle based on passage times through the city. " +
                "The calculation considers vehicle type, time of day, holidays, and applies the single charge rule " +
                "(only the highest fee is charged within 60 minutes) and daily maximum cap.")
            .Produces<CalculateTaxResponseDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .WithApiVersionSet(apiV1)
            .WithOpenApi();

        return endpoints;
    }

    private static async Task<IResult> CalculateTax(
        [FromBody] CalculateTaxRequestDto requestDto,
        [FromServices] IValidator<CalculateTaxRequestDto> validator,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(requestDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var command = new CalculateTaxCommand(
                requestDto.VehicleType,
                requestDto.City,
                requestDto.PassageTimes);

            var response = await mediator.Send(command, cancellationToken);
            
            return Results.Ok(new CalculateTaxResponseDto(
                response.TotalTax,
                response.City,
                response.VehicleType,
                response.NumberOfPassages,
                response.Breakdowns
                    .Select(s => new TaxBreakdownDto(
                        s.PassageTime,
                        s.Fee,
                        s.Reason))
                    .ToArray()));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Problem(
                title: "Not Found",
                detail: ex.Message,
                statusCode: StatusCodes.Status404NotFound
            );
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "Internal Server Error",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }
}