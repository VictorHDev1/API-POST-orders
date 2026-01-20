# ♻️ Issue  solution #3
#3 ♻️ Productionize codebase
Fat controller, no logging, low test coverage. Refactor Clean Arch + Serilog + 80% coverage. Document trade-offs in PR.

Clean Architecture · Serilog · High Test Coverage

Fat controller, no logging, low test coverage.
Refactor to Clean Architecture + Serilog + ~80% test coverage.
Document trade-offs.

## Overview

This solution addresses Issue #3 – Productionize codebase, which required refactoring an existing API with the following problems:

- Fat controllers containing business logic
- No centralized or structured logging
- Low automated test coverage
- Tight coupling between layers

The codebase was refactored following Clean Architecture principles, adding structured logging with Serilog and improving test coverage to approximately 80%, while preserving the original business behavior.

## Architecture

The solution is structured into five projects, enforcing a clear separation of concerns:

## 📁 Project Structure

```

├── OrderApi/
│   ├── Controllers/
│   │   └── OrderController.cs    # API endpoints
│   ├── Dockerfile
│   ├── docker-compose.yml
│   ├── Program.cs
├── OrderApi.Application/
│   |   ├── BusinessException/
│   │   |    └── BusinessException.cs  
│   |   ├── Dtos/
│   │   |    ├── CreateOrderItemRequest.cs
│   │   |    ├── CreateOrderRequest.cs
│   │   |    ├── OrderItemResponse.cs
│   │   |    ├── OrderResponse.cs
│   │   |    └── OrderSummaryResponse.cs       
│   |   ├── Interfaces/
│   │   |    ├── ICustomerRepository.cs
│   │   |    ├── IOrderRepository.cs
│   │   |    └── IOrderService.cs   
│   │   ├── Mappers/
│   │   |    ├── OrderMapper.cs   
│   │   |    ├── Services/
│   │   |    └── OrderService.cs   
├──OrderApi.Domain/
│   │   ├── Entities/
│   │   |    ├── Customer.cs   
│   │   |    ├── Order/
│   │   |    └── OrderItem.cs   
│   │   ├── Enums/
│   │   |    └── OrderStatus.cs
├──OrderApi.Infrastructure/
│   │   ├── Logging/
│   │   |    └── SerilogConfiguration.cs
│   │   ├── Persistence/
│   │   |    └── OrderContext.cs
│   │   ├── Repositories/
│   │   |    ├── OrderContext.cs
│   │   |    └── OrderRepository.cs
├──OrderAPITest/
│   │   |    └── Controllers.cs
└── README.md
```

---


## Layer Responsibilities

OrderApi (API)
- ASP.NET Core Web API
- Thin controllers
- HTTP request/response handling only
- Delegates business logic to the Application layer

OrderApi.Application
- Application services
- DTOs and mappings
- Business use cases
- Interfaces for persistence and infrastructure
- Custom business exceptions

OrderApi.Domain
- Core business entities
- Enums and domain rules
- No dependencies on other layers
- Pure domain logic

OrderApi.Infrastructure
- Database access and repositories
- Persistence configuration (EF Core)
- Logging implementation
- External system integrations

OrderApiTest
- Unit and integration tests
- Focused on business logic and controllers
- Validation of expected behavior and edge cases

## Logging

Serilog was introduced to provide centralized and structured logging.

Logging features:
- Console logging for local development
- File logging for traceability
- Structured logs with contextual information
- Easily extensible to external sinks (Seq, ELK, etc.)

Logging configuration is isolated in the Infrastructure layer, keeping the rest of the application decoupled from logging concerns.

## Testing and Coverage

- Automated tests added for application services and controllers
- Overall coverage increased to approximately 80%

Test focus:
- Business rules
- Error handling
- Happy paths and edge cases

Run tests:

dotnet test

All tests pass locally.

## Improvements Over Original Implementation

Before:
- Controllers contained business logic
- No logging
- Low test coverage
- Hard-to-maintain structure
- Tight coupling between layers

After:
- Thin controllers
- Clear separation of concerns
- Centralized structured logging
- High test coverage
- Scalable and maintainable architecture

## Trade-offs

- Increased number of projects and files
  Improves long-term maintainability and testability

- Slightly higher initial complexity
  Enables better scalability and clearer ownership of responsibilities

- Additional abstraction layers
  Allows easier changes to infrastructure and external dependencies

These trade-offs were considered acceptable for a production-ready codebase.

## Setup

Start SQL Server container:

docker compose up -d sqlserver

Wait approximately 30 seconds, then apply migrations:

cd src/OrderApi
dotnet ef database update

Run the API:

dotnet run

Swagger UI:
https://localhost:5001/swagger

## Alternative: Run Everything with Docker

docker compose up -d

Swagger UI:
http://localhost:5000/swagger

### Example: Create Order
```json
POST /api/order
{
  "customerId": 1,
  "items": [
    {
      "productName": "Widget",
      "productSku": "WGT-001",
      "quantity": 2,
      "unitPrice": 29.99
    }
  ]
}
```

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/order/{id}` | Get order by ID |
| POST | `/api/order` | Create new order |

## Database

The application uses SQL Server with Entity Framework Core.

Connection string configured in appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}

Tech Stack
.NET 8
Entity Framework Core 9
SQL Server 2022
Swagger/OpenAPI (Swashbuckle)
xUnit + FluentAssertions (testing)
Configuration
Environment-specific settings in:

appsettings.json (base)
appsettings.Development.json (dev overrides)

## Conclusion

This implementation fully addresses Issue #3 – Productionize codebase by delivering a clean, testable, observable, and production-ready API aligned with modern backend engineering best practices.
