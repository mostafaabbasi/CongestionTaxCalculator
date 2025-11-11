# 🚗 Congestion Tax Calculator API

<div align="center">

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=for-the-badge&logo=csharp)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![Tests](https://img.shields.io/badge/Tests-59%2B-success?style=for-the-badge)

**A modern, production-ready .NET Web API for calculating congestion taxes with comprehensive business rules, clean architecture, and full test coverage.**

[Features](#-features) • [Quick Start](#-quick-start) • [API Documentation](#-api-documentation) • [Architecture](#-architecture) • [Testing](#-testing)

</div>

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Quick Start](#-quick-start)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [API Documentation](#-api-documentation)
- [Business Rules](#-business-rules)
- [Testing](#-testing)
- [Docker Deployment](#-docker-deployment)
- [Development Guide](#-development-guide)
- [Configuration](#-configuration)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🌟 Overview

The **Congestion Tax Calculator** is a sophisticated API designed to calculate congestion charges for vehicles passing through city toll zones. Built with modern .NET 9.0, it implements complex business rules including time-based pricing, vehicle exemptions, holiday handling, and single-charge interval logic.

### Why This Project?

- **Production-Ready**: Full Docker support, database migrations, comprehensive error handling
- **Well-Tested**: 59+ unit and integration tests with high code coverage
- **Clean Architecture**: DDD principles, CQRS pattern, separation of concerns
- **Developer-Friendly**: Swagger documentation, helper scripts, detailed guides
- **Scalable**: Containerized, cloud-ready, database-backed

---

## ✨ Features

### Core Functionality
- ✅ **RESTful API** for congestion tax calculation
- ✅ **Multi-City Support** with configurable rules per city
- ✅ **Vehicle Type Classification** (8 types with toll-free support)
- ✅ **Time-Based Pricing** with configurable fee schedules
- ✅ **Single Charge Interval** (60-minute window, highest fee)
- ✅ **Daily Maximum Cap** enforcement (60 SEK)
- ✅ **Smart Date Handling** (weekends, holidays, special months)

### Technical Features
- ✅ **Clean Architecture** with DDD principles
- ✅ **CQRS Pattern** using MediatR
- ✅ **Entity Framework Core** with SQL Server
- ✅ **FluentValidation** for input validation
- ✅ **API Versioning** (v1 with room for growth)
- ✅ **Swagger/OpenAPI** documentation
- ✅ **Docker Multi-Stage Builds** with test integration
- ✅ **Comprehensive Test Suite** (xUnit, FluentAssertions, Moq)
- ✅ **In-Memory Testing** for integration tests
- ✅ **Health Checks** for database connectivity

---

## 🚀 Quick Start

### Prerequisites

<table>
<tr><td><b>Option 1: Docker</b></td><td><b>Option 2: Local Development</b></td></tr>
<tr>
<td>

- Docker Desktop
- Docker Compose

</td>
<td>

- .NET 9.0 SDK
- SQL Server 2022
- (Optional) Visual Studio 2022 / Rider

</td>
</tr>
</table>

### Using Docker Compose (Recommended)

```bash
# Clone the repository
git clone <repository-url>
cd CongestionTaxCalculator

# Start all services (API + SQL Server)
docker compose up -d

# View logs
docker compose logs -f api

# API is now available at http://localhost:5032
# Swagger UI: http://localhost:5032/swagger
```

### Run Tests

```bash
# Run all tests using Docker
docker compose --profile test up tests

# Run tests with code coverage
docker compose -f compose.test.yaml run --rm test-coverage

# Run tests locally (requires .NET SDK)
dotnet test
```

### Local Development

```bash
# Restore dependencies
dotnet restore

# Update database
dotnet ef database update --project CongestionTaxCalculator

# Run the application
dotnet run --project CongestionTaxCalculator

# Run tests
dotnet test

# Watch mode with hot reload
dotnet watch run --project CongestionTaxCalculator
```

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles to create a decoupled, testable, and maintainable system.

- **Domain Layer**: Contains core business logic, entities, and value objects. It has no dependencies on other layers.
- **Application Layer**: Implements business logic using CQRS (MediatR). It depends only on the Domain layer.
- **Infrastructure Layer**: Handles external concerns like database access (EF Core), file systems, etc.
- **Presentation Layer**: The API endpoints (Minimal APIs) that expose the application's functionality.

<div align="center">
  <img src="https://i.imgur.com/A8W4G3g.png" alt="Clean Architecture Diagram" width="600">
</div>

---

## 📂 Project Structure

```
.
├── CongestionTaxCalculator/          # Main API project (Presentation)
│   ├── Domain/                       # Domain Layer
│   │   ├── Entities/                 # City, CongestionTaxCalculation
│   │   ├── Enums/                    # VehicleType
│   │   └── ValueObjects/             # Passage
│   ├── Features/                     # Application Layer (CQRS)
│   │   └── Tax/Commands/             # Calculate tax command
│   ├── Infrastructure/               # Infrastructure Layer
│   │   └── Persistence/              # DbContext and migrations
│   └── Endpoints/                    # API endpoints
├── CongestionTaxCalculator.Tests/    # Unit and integration tests
│   ├── Domain/                       # Domain tests
│   └── Features/                     # Feature tests
├── Dockerfile                        # Multi-stage Docker build with tests
├── compose.yaml                      # Main Docker Compose configuration
└── compose.test.yaml                 # Test-specific Docker Compose
```

---

## 📖 API Documentation

### Swagger UI

Once the application is running, interactive API documentation is available at:
- **http://localhost:5032/swagger**

### Example Request

**Calculate Congestion Tax:**

```bash
curl -X POST "http://localhost:5032/api/v1/tax/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "vehicleType": "Car",
    "city": "Gothenburg",
    "passageTimes": [
      "2025-11-13T07:00:00",
      "2025-11-13T12:00:00"
    ]
  }'
```

**Example Response:**
```json
{
  "totalTax": 26,
  "city": "Gothenburg",
  "vehicleType": "Car",
  "numberOfPassages": 2,
  "breakdowns": [
    {
      "passageTime": "2025-11-13T07:00:00",
      "fee": 18,
      "reason": "Fee: 18 SEK"
    },
    {
      "passageTime": "2025-11-13T12:00:00",
      "fee": 8,
      "reason": "Fee: 8 SEK"
    }
  ]
}
```

---

## 📊 Business Rules

### Congestion Tax Rules (Gothenburg)

1. **Toll-Free Vehicles:**
   - Motorcycles, Buses, Tractors
   - Emergency, Diplomat, Foreign, Military vehicles

2. **Toll-Free Days:**
   - Weekends (Saturday & Sunday)
   - July (entire month)
   - Public holidays

3. **Single Charge Interval:**
   - 60-minute window
   - Only the highest fee within the window is charged

4. **Maximum Daily Fee:**
   - 60 SEK per day

5. **Fee Schedule:**
   | Time          | Fee  |
   |---------------|------|
   | 06:00-06:29   | 8 SEK|
   | 06:30-06:59   | 13 SEK|
   | 07:00-07:59   | 18 SEK|
   | 08:00-08:29   | 13 SEK|
   | 08:30-14:59   | 8 SEK |
   | 15:00-15:29   | 13 SEK|
   | 15:30-16:59   | 18 SEK|
   | 17:00-17:59   | 13 SEK|
   | 18:00-18:29   | 8 SEK |

---

## 🧪 Testing

The project has a comprehensive test suite with **59+ tests**.

### Test Coverage
- ✅ Domain entity validation and business rules
- ✅ Tax calculation logic with various scenarios
- ✅ Toll-free vehicle types, weekends, holidays, and July exemptions
- ✅ Single charge interval and maximum daily fee enforcement
- ✅ Command handler integration with an in-memory database
- ✅ Input validation rules

### How to Run Tests

**Docker (Recommended):**
```bash
# Run all tests
docker compose --profile test up tests

# Run tests with code coverage
docker compose -f compose.test.yaml run --rm test-coverage

# Run specific test from compose
docker compose -f compose.test.yaml run --rm test-runner
```

**Local (.NET SDK Required):**
```bash
# Run all tests
dotnet test

# Run a specific test class
dotnet test --filter "FullyQualifiedName~CityTests"

# Run with detailed output
dotnet test --verbosity detailed
```

See [DOCKER_TEST_GUIDE.md](DOCKER_TEST_GUIDE.md) for more details.

---

## 🐳 Docker Deployment

### Build and Run

```bash
# Build and start all services
docker compose up --build

# Run in background
docker compose up -d

# View logs
docker compose logs -f api

# Stop services
docker compose down
```

### Multi-Stage Dockerfile

The Dockerfile is optimized with multiple stages:
- `build` - Builds the application and test projects
- `test` - Runs unit tests (can be used as a target)
- `publish` - Publishes the application
- `final` - Final lightweight runtime image

**Build a specific stage:**
```bash
docker build --target test -t congestion-tax-tests .
```

---

## 🛠️ Development Guide

### Prerequisites

- .NET 9.0 SDK
- Docker & Docker Compose
- SQL Server (can be run via Docker)

### Setup

1. **Clone the repository**
2. **Restore packages**: `dotnet restore`
3. **Run migrations**: `dotnet ef database update --project CongestionTaxCalculator`
4. **Run the app**: `dotnet run --project CongestionTaxCalculator`

### Add a Migration

```bash
dotnet ef migrations add <MigrationName> --project CongestionTaxCalculator
```

---

## ⚙️ Configuration

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: `Development` or `Production`
- `ASPNETCORE_URLS`: `http://+:5032`
- `ConnectionStrings__DefaultConnection`: Database connection string

### `appsettings.json`

The `appsettings.json` file contains default configurations for logging and connection strings. Use `appsettings.Development.json` or environment variables to override for local development.

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new feature branch (`git checkout -b feature/your-feature`).
3. Make your changes.
4. Add or update tests.
5. Run tests to ensure they pass (`docker compose --profile test up tests` or `dotnet test`).
6. Submit a pull request.

---

## 📝 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

<div align="center">
  <p>Developed with ❤️ by the Fintranet Team</p>
</div>

