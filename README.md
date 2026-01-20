<<<<<<< HEAD
##### Issue Solution 1 #####

The original implementation generated order numbers based on a non-atomic read of the latest order, which caused race conditions under concurrency.
I fixed it by removing the check-then-insert logic, enforcing uniqueness at the database level, and handling conflicts explicitly.
This makes the endpoint safe for concurrent requests and scalable.

# API POST Orders

REST API developed in **.NET 8** for order creation and management.  
This project was implemented as part of a **technical take-home test**, with a focus on concurrency handling, data consistency, and clean API design.

---

## 🐛 Issue #1 – Fix race condition in `POST /orders`

### Problem
While running `dotnet test`, intermittent failures were observed when creating orders concurrently.  
The `POST /orders` endpoint needed to ensure correct behavior and data integrity under simultaneous requests.

---

###  Analysis
Under concurrent load, multiple requests may attempt to persist orders at the same time.  
The main risk identified was a **potential collision during order number generation**, which could lead to persistence errors in concurrent scenarios.

---

###  Implemented solution
The solution relies on database-level guarantees combined with proper API error handling:

- The order number is generated using a **GUID-based strategy**, ensuring uniqueness even under high concurrency.
- The database enforces **uniqueness on `OrderNumber`** via a unique constraint.
- Order persistence is handled atomically using `SaveChangesAsync`.
- In the event of a concurrent collision, the API catches `DbUpdateException` and returns an appropriate HTTP response (`409 Conflict`).

This approach avoids race conditions while keeping the implementation simple and efficient, without introducing unnecessary locks.

---

###  Tests
An **integration test** was added to cover the concurrent scenario:

- Multiple `POST /orders` requests are executed in parallel.
- The test verifies that orders are created successfully or that the API responds in a controlled manner when conflicts occur.
- The test fails with unsafe implementations and passes with the applied fix, preventing future regressions.

---

## 🚀 Technologies used
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- xUnit
- Docker & Docker Compose

---

##  Run locally

```bash
dotnet restore
dotnet build
dotnet test
dotnet run


### Run with Docker
docker-compose up --build
=======
<<<<<<< HEAD
# OrderAP-IPOST-orders
SOLUTION ISSUE 1 - dotnet test fails on concurrent order creation. Investigate + fix + add integration test
=======
# 🚨 C# BACKEND TAKE-HOME 🚨

**Mid–Senior C# Backend Engineer – DevOpsCollab**

**Your mission (4-6 hrs total):**
- Solve the 3 GitHub Issues below
- Submit clean PRs (one per issue)  
- Use Copilot/Cursor freely—but own the code
- Deadline: **Tuesday Jan 20 EOD**

## 🎯 Issues to Solve

**#1 🐛 Fix race condition in POST /orders**  
`dotnet test` fails on concurrent order creation. Investigate + fix + add integration test.

**#2 ⚡ Optimize /reports endpoint**  
Slow under load (check SQL profiler). Add indexes/queries. Show before/after benchmarks.

**#3 ♻️ Productionize codebase**  
Fat controller, no logging, low test coverage. Refactor Clean Arch + Serilog + 80% coverage. Document trade-offs in PR.

**Success =** Atomic commits, passing tests, clear PR descriptions.

Questions? Message me directly.

---

# Order Management API

A .NET 8 Web API for order management in an e-commerce backend. Built with Entity Framework Core and SQL Server.

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### Setup

1. **Start SQL Server container:**
   ```bash
   docker compose up -d sqlserver
   ```

2. **Wait for SQL Server to be ready** (about 30 seconds), then apply migrations:
   ```bash
   cd src/OrderApi
   dotnet ef database update
   ```

3. **Run the API:**
   ```bash
   dotnet run
   ```

4. **Open Swagger UI:**
   Navigate to [https://localhost:5001/swagger](https://localhost:5001/swagger)

### Alternative: Run Everything in Docker
```bash
docker compose up -d
```
API will be available at [http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 📁 Project Structure

```
OrderApi/
├── src/OrderApi/
│   ├── Controllers/
│   │   └── OrderController.cs    # API endpoints
│   ├── Data/
│   │   └── OrderContext.cs       # EF Core DbContext
│   ├── Models/
│   │   ├── Order.cs              # Order entity
│   │   ├── OrderItem.cs          # Order line item
│   │   ├── Customer.cs           # Customer entity
│   │   └── Dto/
│   │       └── OrderDtos.cs      # Request/Response DTOs
│   ├── Program.cs
│   ├── appsettings.json
│   └── OrderApi.csproj
├── tests/OrderApi.Tests/
│   ├── OrderControllerTests.cs
│   └── OrderApi.Tests.csproj
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## 🔌 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/order` | List orders (paginated) |
| GET | `/api/order/{id}` | Get order by ID |
| POST | `/api/order` | Create new order |
| PATCH | `/api/order/{id}/status` | Update order status |
| DELETE | `/api/order/{id}` | Delete order |
| GET | `/api/order/reports` | Get order reports |

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

---

## 🧪 Running Tests

```bash
cd tests/OrderApi.Tests
dotnet test
```

---

## 🗄️ Database

The application uses SQL Server with Entity Framework Core. 

### Connection String
Configured in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  }
}
```

### Seed Data
The database is seeded with:
- 10 sample customers
- 100 sample orders with items

### Migrations
```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

---

## 🛠️ Development Notes

### Tech Stack
- .NET 8
- Entity Framework Core 8
- SQL Server 2022
- Swagger/OpenAPI (Swashbuckle)
- xUnit + FluentAssertions (testing)

### Configuration
Environment-specific settings in:
- `appsettings.json` (base)
- `appsettings.Development.json` (dev overrides)

---

## 📄 License

MIT
>>>>>>> e674d89 (Resolved issue 1)
>>>>>>> 60250ab (First commit)
