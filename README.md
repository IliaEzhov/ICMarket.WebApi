# ICMarket.WebApi

A blockchain data aggregator Web API built with ASP.NET Core 8 and Clean Architecture. The application fetches, stores, and serves historical blockchain network data from the [BlockCypher API](https://www.blockcypher.com/dev/) for ETH, DASH, BTC, and LTC networks.

## Table of Contents

- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Design Patterns](#design-patterns)
- [Testing](#testing)
- [Docker](#docker)
- [CI/CD Pipeline](#cicd-pipeline)
- [Security & Performance](#security--performance)
- [Implementation Details](#implementation-details)
- [Requirements Checklist](#requirements-checklist)

---

## Tech Stack

| Category | Technology |
|---|---|
| Framework | .NET 8 / ASP.NET Core 8 |
| Database | SQLite via Entity Framework Core |
| CQRS / Mediator | MediatR |
| Validation | FluentValidation |
| Testing | NUnit, Moq, Microsoft.AspNetCore.Mvc.Testing |
| Containerization | Docker (Linux) |
| API Documentation | Swagger / OpenAPI |
| Rate Limiting | .NET 8 Built-in RateLimiter |
| Caching | In-Memory Cache (IMemoryCache) |
| CI/CD | GitHub Actions |

## Architecture

The project follows **Clean Architecture** with strict dependency rules and separation of concerns across five layers:

```
┌──────────────────────────────────────────────────┐
│                    API Layer                      │
│         Controllers, Filters, Middleware          │
├──────────────────────────────────────────────────┤
│               Application Layer                   │
│    Commands, Queries, DTOs, Validators, Mappings  │
├──────────────────────────────────────────────────┤
│              Infrastructure Layer                  │
│     EF Core, Repositories, HTTP Services          │
├──────────────────────────────────────────────────┤
│                 Domain Layer                       │
│         Entities, Interfaces (zero deps)          │
├──────────────────────────────────────────────────┤
│                 Common Layer                       │
│            Shared Constants                        │
└──────────────────────────────────────────────────┘
```

**Data Flow:**

1. `POST /api/blockchain/fetch` → MediatR dispatches `FetchAndStoreBlockchainDataCommand`
2. The handler calls `IBlockchainService.FetchAllBlockchainDataAsync()` which fetches all 5 endpoints **in parallel** via `Task.WhenAll`
3. `BlockcypherService` deserializes JSON responses from `https://api.blockcypher.com/v1/{coin}/{network}`
4. Responses are mapped to `BlockchainData` entities with `CreatedAt = DateTime.UtcNow`
5. Entities are persisted via `IBlockchainDataRepository.AddRangeAsync()` + `IUnitOfWork.SaveChangesAsync()`
6. `GET` endpoints query the stored history ordered by `CreatedAt` descending

## Project Structure

```
ICMarket.WebApi/
├── src/
│   ├── API/                          # ASP.NET Core host
│   │   ├── Controllers/              # BlockchainController (3 endpoints)
│   │   ├── Filters/                  # ValidationExceptionFilter (400 responses)
│   │   ├── Middleware/               # GlobalExceptionHandlerMiddleware (500 responses)
│   │   ├── Program.cs                # App configuration & startup
│   │   ├── appsettings.json          # Connection strings, BlockCypher config
│   │   └── Dockerfile                # Multi-stage Docker build
│   │
│   ├── Application/                  # Use cases (depends on Domain)
│   │   ├── Behaviors/                # ValidationBehavior (MediatR pipeline)
│   │   ├── Commands/                 # FetchAndStoreBlockchainData (command + handler)
│   │   ├── Queries/                  # GetAllBlockchainData, GetBlockchainDataByName
│   │   ├── DTOs/                     # BlockchainDataDto
│   │   ├── Interfaces/               # IBlockchainService
│   │   ├── Mappings/                 # Manual ToDto/ToDtoList extension methods
│   │   └── DependencyInjection.cs    # AddApplication() registration
│   │
│   ├── Domain/                       # Core entities & interfaces (zero dependencies)
│   │   ├── Entities/                 # BaseEntity, BlockchainData
│   │   └── Interfaces/               # IBlockchainDataRepository, IUnitOfWork
│   │
│   ├── Infrastructure/               # External concerns (depends on Application, Domain)
│   │   ├── Configuration/            # BlockcypherSettings (options pattern)
│   │   ├── Models/                   # BlockcypherResponse (API deserialization)
│   │   ├── Persistence/              # AppDbContext, entity configurations
│   │   ├── Repositories/             # BlockchainDataRepository (EF Core)
│   │   ├── Services/                 # BlockcypherService (HTTP client)
│   │   └── DependencyInjection.cs    # AddInfrastructure() registration
│   │
│   └── Common/                       # Shared constants across all layers
│       └── Constants/                # BlockchainConstants, DatabaseConstants, ApiConstants
│
├── tests/
│   ├── ICMarket.UnitTests/           # 37 NUnit tests (handlers, validators, behaviors)
│   ├── ICMarket.IntegrationTests/    # 19 integration tests (health, swagger, endpoints)
│   └── ICMarket.FunctionalTests/     # 18 end-to-end workflow tests
│
├── docker-compose.yml                # Container orchestration
└── ICMarket.sln                      # Solution file
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (optional, for containerized deployment)

### Build

```bash
dotnet build ICMarket.sln
```

### Run

```bash
cd src/API
dotnet run
```

The API will be available at:
- **HTTP:** http://localhost:5182
- **HTTPS:** https://localhost:7041
- **Swagger UI:** http://localhost:5182/swagger

The SQLite database (`ICMarket.db`) is auto-created on first startup.

### Run Tests

```bash
# All tests
dotnet test ICMarket.sln

# Unit tests only
dotnet test tests/ICMarket.UnitTests

# Integration tests only
dotnet test tests/ICMarket.IntegrationTests

# Functional tests only
dotnet test tests/ICMarket.FunctionalTests
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/blockchain/fetch` | Fetch data from all 5 BlockCypher endpoints and store |
| `GET` | `/api/blockchain` | Get all stored blockchain data (descending by CreatedAt) |
| `GET` | `/api/blockchain/{name}` | Get history for a specific blockchain (e.g., `BTC.main`) |
| `GET` | `/health` | Health check (includes SQLite connectivity) |

### Supported Blockchain Names

| Name | BlockCypher Endpoint |
|---|---|
| `ETH.main` | https://api.blockcypher.com/v1/eth/main |
| `DASH.main` | https://api.blockcypher.com/v1/dash/main |
| `BTC.main` | https://api.blockcypher.com/v1/btc/main |
| `BTC.test3` | https://api.blockcypher.com/v1/btc/test3 |
| `LTC.main` | https://api.blockcypher.com/v1/ltc/main |

### Example Usage

```bash
# Fetch and store blockchain data from all endpoints
curl -X POST http://localhost:5182/api/blockchain/fetch

# Get all stored blockchain data history
curl http://localhost:5182/api/blockchain

# Get history for a specific blockchain (case-insensitive)
curl http://localhost:5182/api/blockchain/BTC.main

# Health check
curl http://localhost:5182/health
```

### Example Response

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "createdAt": "2024-01-15T10:30:00Z",
    "name": "BTC.main",
    "height": 826000,
    "hash": "0000000000000000000...",
    "time": "2024-01-15T10:25:00Z",
    "latestUrl": "https://api.blockcypher.com/v1/btc/main/blocks/...",
    "previousHash": "0000000000000000000...",
    "previousUrl": "https://api.blockcypher.com/v1/btc/main/blocks/...",
    "peerCount": 250,
    "unconfirmedCount": 1500,
    "lastForkHeight": 825999,
    "lastForkHash": "0000000000000000000...",
    "highFeePerKb": 50000,
    "mediumFeePerKb": 25000,
    "lowFeePerKb": 10000
  }
]
```

## Design Patterns

| Pattern | Implementation |
|---|---|
| **CQRS** | Commands (write) and Queries (read) separated via MediatR |
| **Repository** | `IBlockchainDataRepository` / `BlockchainDataRepository` |
| **Unit of Work** | `IUnitOfWork` implemented by `AppDbContext` |
| **Pipeline Behaviors** | `ValidationBehavior`, `LoggingBehavior`, `CachingBehavior` (MediatR pipeline) |
| **Options Pattern** | `BlockcypherSettings` bound from `appsettings.json` |
| **Dependency Injection** | Layer-specific extension methods (`AddApplication()`, `AddInfrastructure()`) |

## Testing

The project includes three test projects covering different testing levels:

### Unit Tests (`ICMarket.UnitTests`) — 37 tests
- **Handlers:** `FetchAndStoreBlockchainDataCommandHandler`, `GetAllBlockchainDataQueryHandler`, `GetBlockchainDataByNameQueryHandler`
- **Validators:** `GetBlockchainDataByNameQueryValidator` (valid names, case-insensitive, invalid names)
- **Mappings:** `BlockchainDataMappingExtensions` (ToDto, ToDtoList, all property mappings)
- **Behaviors:** `ValidationBehavior`, `LoggingBehavior`, `CachingBehavior` (cache hit, cache miss, non-cacheable requests, expiration)

### Integration Tests (`ICMarket.IntegrationTests`) — 19 tests
- **Health:** `/health` returns 200 + "Healthy"
- **Swagger:** JSON valid, contains all 3 blockchain endpoints, UI accessible
- **Endpoints:** GET empty, POST fetch, data persistence, GET by name, invalid name → 400, camelCase JSON

### Functional Tests (`ICMarket.FunctionalTests`) — 18 tests
- **Workflows:** Fetch → query flow, multiple fetches accumulate history, filter by name, case-insensitive
- **Ordering:** CreatedAt descending, timestamps populated
- **Validation:** Empty/invalid names → 400, valid names case-insensitive → 200

All test projects use `WebApplicationFactory<Program>` with in-memory SQLite and mocked `IBlockchainService` to avoid external HTTP calls.

## Security & Performance

### CORS (Configurable)

CORS is configured per environment via `appsettings.json`. Production restricts origins, methods, and headers:

```json
{
  "Cors": {
    "AllowedOrigins": ["https://yourdomain.com"],
    "AllowedMethods": ["GET", "POST"],
    "AllowedHeaders": ["Content-Type"]
  }
}
```

For development, use `["*"]` to allow all origins/methods/headers.

### Rate Limiting

Built-in .NET 8 rate limiting with two policies:

| Policy | Limit | Window | Applied To |
|--------|-------|--------|------------|
| **Default** | 100 requests | 60 seconds | All endpoints |
| **Strict** | 5 requests | 60 seconds | `POST /api/blockchain/fetch` |

Configurable via `appsettings.json`:

```json
{
  "RateLimiting": {
    "PermitLimit": 100,
    "WindowInSeconds": 60,
    "FetchPermitLimit": 5,
    "FetchWindowInSeconds": 60
  }
}
```

Exceeding the limit returns `HTTP 429 Too Many Requests`.

### In-Memory Caching

Query responses are cached using a MediatR pipeline behavior:

- **Cached Queries:** `GetAllBlockchainDataQuery`, `GetBlockchainDataByNameQuery`
- **Cache Duration:** 5 minutes (configurable)
- **Cache Invalidation:** Automatic when `POST /api/blockchain/fetch` completes successfully
- **Pattern:** `ICacheableQuery` marker interface + `CachingBehavior<TRequest, TResponse>`

The caching pipeline uses `CancellationChangeToken` for efficient bulk invalidation.

## Docker

### Build and Run with Docker

```bash
# Build the image
docker build -t icmarket-api -f src/API/Dockerfile .

# Run the container
docker run -p 8080:8080 icmarket-api
```

### Docker Compose

```bash
docker compose up --build
```

The API will be available at http://localhost:8080.

### Docker Features

| Feature | Implementation |
|---------|----------------|
| **Health Check** | `HEALTHCHECK` instruction using `curl` — auto-restarts unhealthy containers |
| **Volumes** | `icmarket-data` (SQLite DB), `icmarket-logs` (application logs) |
| **Networking** | Bridge network (`icmarket-network`) for container isolation |
| **Restart Policy** | `unless-stopped` — auto-restart on failure |
| **Non-root User** | Runs as `$APP_UID` for security |
| **Metadata** | `LABEL` instructions for maintainer, version, description |

### Volume Paths

```yaml
volumes:
  - icmarket-data:/app/data    # SQLite database
  - icmarket-logs:/app/logs    # Application logs
```

## CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/ci-cd.yml`) with 6 jobs:

| Job | Description |
|-----|-------------|
| **Build** | Restore, build, upload artifacts |
| **Test** | Run all 74 tests (Unit, Integration, Functional) with code coverage |
| **Code Quality** | `dotnet format --verify-no-changes` |
| **Security Scan** | `dotnet list package --vulnerable` — fails on vulnerable packages |
| **Docker Build** | Build image, run container, test endpoints |
| **Docker Push** | (Optional) Push to container registry on `main` branch |

### Test Reporting

Test results are published to GitHub Actions UI via `dorny/test-reporter` with:
- Per-test pass/fail status
- Duration metrics
- Failure details with stack traces

### Triggers

- Push to `main`, `master`, or `development` branches
- Pull requests to protected branches
- Manual trigger via `workflow_dispatch`

## Implementation Details

### Key Implementation Decisions

1. **Clean Architecture:** The project strictly follows Clean Architecture with the Domain layer having zero external dependencies. The dependency flow is: API → Application → Domain ← Infrastructure.

2. **CQRS with MediatR:** All operations are modeled as either Commands (write) or Queries (read), dispatched through MediatR. This provides a clean separation of concerns and enables pipeline behaviors like validation.

3. **Parallel Fetching:** `BlockcypherService` uses `Task.WhenAll` to fetch all 5 BlockCypher endpoints concurrently, improving performance significantly over sequential calls.

4. **Automatic Validation:** FluentValidation validators are auto-discovered from the assembly and executed as a MediatR pipeline behavior (`ValidationBehavior`). Invalid requests are rejected with structured 400 responses before reaching the handler.

5. **Global Error Handling:** Two layers of error handling:
   - `ValidationExceptionFilter` catches FluentValidation exceptions → structured 400 responses
   - `GlobalExceptionHandlerMiddleware` catches all unhandled exceptions → structured 500 responses (with stack traces only in Development)

6. **SQLite with EF Core:** The database is auto-created on startup via `EnsureCreatedAsync()`. Entity configuration uses Fluent API with proper column max lengths, indexes, and a composite index on `(Name, CreatedAt)` for optimized queries.

7. **Structured Logging:** All controllers and handlers use `ILogger<T>` with structured message templates (e.g., `"Fetched {Count} records for {Name}"`) for observability.

8. **Constants Library:** All magic strings and numbers are extracted into `ICMarket.Common.Constants` to ensure consistency across layers and tests.

9. **Manual Model Mapping:** Extension methods (`ToDto`, `ToDtoList`) are used instead of AutoMapper for explicit, type-safe mapping with zero reflection overhead.

10. **JSON Serialization:** API responses use camelCase property naming and ignore null values (`WhenWritingNull`), producing clean JSON payloads.

### BlockCypher Data Source

The application fetches blockchain network info from the following BlockCypher endpoints:

| # | Endpoint |
|---|---|
| 1 | https://api.blockcypher.com/v1/eth/main |
| 2 | https://api.blockcypher.com/v1/dash/main |
| 3 | https://api.blockcypher.com/v1/btc/main |
| 4 | https://api.blockcypher.com/v1/btc/test3 |
| 5 | https://api.blockcypher.com/v1/ltc/main |

Each response includes: block height, hash, timestamp, peer count, unconfirmed transaction count, fork info, and fee/gas metrics (varies by blockchain type).

### Database Schema

The `BlockchainData` table stores all fetched records with:
- `Id` (GUID, primary key)
- `CreatedAt` (UTC timestamp, added on fetch)
- `Name` (blockchain identifier, e.g., "BTC.main")
- Block info: `Height`, `Hash`, `Time`, `LatestUrl`, `PreviousHash`, `PreviousUrl`
- Network info: `PeerCount`, `UnconfirmedCount`, `LastForkHeight`, `LastForkHash`
- BTC/DASH/LTC fees: `HighFeePerKb`, `MediumFeePerKb`, `LowFeePerKb`
- ETH gas: `HighGasPrice`, `MediumGasPrice`, `LowGasPrice`, `HighPriorityFee`, `MediumPriorityFee`, `LowPriorityFee`, `BaseFee`

Indexes: composite `(Name, CreatedAt)` + `(CreatedAt)` for efficient querying.

## Requirements Checklist

### Minimum Functionality

| # | Requirement | Status |
|---|---|---|
| 1 | .NET Core with Clean Architecture and SOLID | ✅ 5-layer Clean Architecture |
| 2 | API endpoints in Swagger showing blockchain history | ✅ 3 endpoints with Swagger UI |
| 3 | CreatedAt timestamp, descending order | ✅ Entity + repository ordering |
| 4 | HealthChecks route and basic CORS policy | ✅ `/health` + configurable CORS |
| 5 | DI, logging, model mapping, serialization, validation | ✅ All implemented |
| 6 | Integration, Functional, and Unit Test projects | ✅ 3 test projects (74 tests) |
| 7 | Runtime profiles: .NET, Docker (Linux) | ✅ launchSettings.json + Dockerfile |

### Framework Requirements

| # | Requirement | Status |
|---|---|---|
| 1 | .NET Core >= .NET 6 | ✅ .NET 8 |
| 2 | Database: SQLite (EF) | ✅ SQLite via EF Core |
| 3 | Data stored as provided in API JSON responses | ✅ Full field mapping |
| 4 | Best practices: performance, inheritance, scalability | ✅ Async, parallel, caching, rate limiting |
| 5 | Async/parallel patterns (Tasks, PLinq) | ✅ Task.WhenAll for parallel fetch |
| 6 | At least two design patterns | ✅ CQRS + Repository + UoW (3 patterns) |
| 7 | Optional: API Gateway | ⏭️ Not implemented |
