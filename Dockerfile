FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5032
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CongestionTaxCalculator/CongestionTaxCalculator.csproj", "CongestionTaxCalculator/"]
COPY ["CongestionTaxCalculator.Tests/CongestionTaxCalculator.Tests.csproj", "CongestionTaxCalculator.Tests/"]
RUN dotnet restore "CongestionTaxCalculator/CongestionTaxCalculator.csproj"
RUN dotnet restore "CongestionTaxCalculator.Tests/CongestionTaxCalculator.Tests.csproj"
COPY . .
WORKDIR "/src/CongestionTaxCalculator"
RUN dotnet build "CongestionTaxCalculator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Test stage - runs unit tests
FROM build AS test
WORKDIR /src/CongestionTaxCalculator.Tests
RUN dotnet test --no-restore --verbosity normal --logger "trx;LogFileName=test_results.trx" --logger "console;verbosity=detailed"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG RUN_TESTS=true
# Optionally run tests before publishing
RUN if [ "$RUN_TESTS" = "true" ]; then \
        cd /src/CongestionTaxCalculator.Tests && \
        dotnet test --no-restore --verbosity normal; \
    fi
RUN dotnet publish "CongestionTaxCalculator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CongestionTaxCalculator.dll"]
