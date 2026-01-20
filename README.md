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
