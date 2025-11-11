# Congestion Tax Calculator - Unit Tests

This directory contains comprehensive unit tests for the Congestion Tax Calculator application.

## Test Structure

### Domain Tests
- **VehicleTypeExtensionsTests.cs**: Tests for vehicle type classification and toll-free vehicle identification
- **CityTests.cs**: Tests for City entity validation, toll fee schedules, and toll-free date management
- **CongestionTaxCalculationTests.cs**: Tests for the core congestion tax calculation logic including:
  - Single charge interval rules
  - Maximum daily fee caps
  - Toll-free vehicles, weekends, holidays, and July
  - Complex scenarios with multiple passages

### Feature Tests
- **CalculateTaxCommandHandlerTests.cs**: Integration tests for the MediatR command handler using in-memory database
- **CalculateTaxValidatorTests.cs**: Tests for FluentValidation rules on command inputs

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests with detailed output
```bash
dotnet test --verbosity detailed
```

### Run tests with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run specific test class
```bash
dotnet test --filter "FullyQualifiedName~CongestionTaxCalculationTests"
```

## Test Coverage

The test suite covers:
- ✅ Domain entity validation and business rules
- ✅ Tax calculation logic with various scenarios
- ✅ Toll-free vehicle types
- ✅ Weekends, holidays, and July exemptions
- ✅ Single charge interval (60 minutes) rules
- ✅ Maximum daily fee (60 SEK) enforcement
- ✅ Command handler integration with database
- ✅ Input validation

## Technologies Used

- **xUnit**: Test framework
- **FluentAssertions**: Fluent assertion library for readable tests
- **Moq**: Mocking framework (available if needed)
- **EntityFrameworkCore.InMemory**: In-memory database for integration tests

## Test Data

Tests use Gothenburg city configuration:
- Max daily fee: 60 SEK
- Single charge interval: 60 minutes
- July toll-free: Yes
- Various time-based fee schedules from 6:00 to 18:30

## Contributing

When adding new features, ensure:
1. Unit tests cover all business logic
2. Edge cases are tested
3. Test names clearly describe the scenario
4. Tests follow Arrange-Act-Assert pattern

