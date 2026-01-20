## ⚡ Issue #2 – Optimize `/reports` Endpoint Performance

###  Problem
The `/reports` endpoint was performing poorly under load.  
Using SQL Server Profiler, high execution times were observed despite a small dataset, indicating inefficient query patterns.

---

###  Root Cause Analysis
The performance bottleneck was caused by inefficient data access patterns:

- Loading full datasets into memory using `ToListAsync()`
- Executing additional database queries inside loops (**N+1 query pattern**)
- Performing aggregations in application code instead of the database

This resulted in multiple round-trips to SQL Server and unnecessary in-memory processing.

docs/benchmarks/1.jpg
---

###  Solution Implemented
The endpoint was refactored to optimize query execution and reduce database overhead:

- Moved aggregations (`COUNT`, `SUM`, `GROUP BY`) to SQL using optimized EF Core queries
- Removed per-entity database calls inside loops
- Reduced database round-trips to a minimal number of queries
- Kept the response contract and business logic unchanged

The generated report remains functionally identical to the previous implementation.

docs/benchmarks/3.jpg

docs/benchmarks/2.jpg
---

###  Performance Benchmark (SQL Server Profiler)

Metrics were calculated by aggregating all EF Core queries executed during a single request.

#### Before optimization
- Total Duration: ~8,956 ms
- Logical Reads: ~24
- 
docs/benchmarks/4_Trace_Before.JPG

#### After optimization
- Total Duration: ~10 ms
- Logical Reads: ~24
- 
docs/benchmarks/4_Trace_After.JPG

Although logical reads remained constant due to the small dataset fitting in memory, execution time was drastically reduced by eliminating N+1 queries, reducing round-trips, and moving aggregations to the database.

docs/benchmarks/4_Api_Reports.jpg
---

###  Validation
- Endpoint tested locally and via Swagger
- Report output verified before and after optimization to ensure identical results
- SQL Server Profiler used to capture and compare execution metrics

---

###  Result
- Endpoint is now scalable under load
- Query execution time reduced by approximately **99%**
- Data-access logic follows best practices for performance and maintainability
