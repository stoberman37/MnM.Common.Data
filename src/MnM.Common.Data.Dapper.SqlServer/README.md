# SQL Server Client & Retry Strategy

This README documents the SQL Server–specific implementations included in this package:

- `SqlServerDbClient` – a preconfigured `DbClient<SqlException>` wrapper for SQL Server  
- `SqlServerRetryStrategy` – a retry policy tailored for SQL Server transient faults  

These provide a plug‑and‑play way to use your existing `DbClient` + Dapper infrastructure with SQL Server by automatically applying a retry strategy for known transient SQL errors.

---

## Contents

- [Overview](#overview)
- [SqlServerDbClient](#sqlserverdbclient)
- [SqlServerRetryStrategy](#sqlserverretrystrategy)
- [Usage Examples](#usage-examples)
  - Basic usage
  - DI registration example
  - Using with repository/specification pattern
- [Transient SQL Errors Handled](#transient-sql-errors-handled)

---

## Overview

These two classes provide SQL Server–specific defaults:

### ✔ Automatic retry strategy for SQL Server  
### ✔ Safe defaults without requiring extra configuration  
### ✔ Still fully compatible with your broader `IDbClient`/Dapper/data-repository architecture  

---

## SqlServerDbClient

```csharp
public class SqlServerDbClient : DbClient<SqlException>
{
    public SqlServerDbClient(Func<DbConnection> connectionFactory)
        : base(connectionFactory, new SqlServerRetryStrategy())
    {
    }
}
```

This class simply wraps `DbClient<SqlException>` and injects a **SQL Server‑aware retry strategy** by default.  
You only need to supply a `Func<DbConnection>` factory (such as a `SqlConnection`).  

fileciteturn3file0

---

## SqlServerRetryStrategy

```csharp
public class SqlServerRetryStrategy : RetryStrategyBase<SqlException>
{
    internal static Func<RetryStrategyBase<SqlException>, SqlException, bool> RetryFunc =
        (s, ex) =>
            s.RetryCount <= s.MaxRetryCount &&
            (ex.Number == -2 || ex.Number == 11 || ex.Number == 1205 || ex.Number == 11001);

    public SqlServerRetryStrategy() : base(RetryFunc)
    {
    }
}
```

This strategy retries when:

- The retry count is still within `MaxRetryCount`
- The SQL Server exception number is one of several recognized transient faults  
  (timeouts, deadlocks, etc.)

fileciteturn3file1

It inherits from your core `RetryStrategyBase<TException>` and plugs in logic specific to SQL Server fault codes.

---

## Usage Examples

### Example 1 – Basic Usage

```csharp
using System.Data.SqlClient;
using MnM.Common.Data.Dapper.SqlServer;

var client = new SqlServerDbClient(
    () => new SqlConnection(connectionString));

// Example query
var users = client
    .SetCommandText("SELECT * FROM dbo.Users")
    .ExecuteQuery<UserDto>();
```

### Example 2 – Registering in ASP.NET Core DI

```csharp
using System.Data.SqlClient;
using MnM.Common.Data.Dapper.SqlServer;
using Microsoft.Extensions.DependencyInjection;

services.AddSingleton<Func<SqlServerDbClient>>(() =>
    new SqlServerDbClient(() => new SqlConnection(connectionString)));

services.AddScoped<IRepository<SqlServerDbClient, UserDto>, Repository<SqlServerDbClient, UserDto>>();
```

This works seamlessly with your existing Dapper repository + specifications.

### Example 3 – With Specifications

```csharp
public class GetUserByIdSpec : IQuerySpecification<SqlServerDbClient, UserDto>
{
    public int Id { get; }
    public GetUserByIdSpec(int id) => Id = id;

    public Func<SqlServerDbClient, UserDto> Execute() =>
        client =>
        {
            var result = client
                .SetCommandText("SELECT * FROM dbo.Users WHERE UserId = @Id")
                .AddNamedParameters(new { Id })
                .ExecuteQuery<UserDto>();

            return result.SingleOrDefault();
        };
}
```

Used via repository:

```csharp
var user = repo.ExecuteDbAction(new GetUserByIdSpec(42));
```

---

## Transient SQL Errors Handled

The following SQL error numbers trigger retries:

| SQL Error Number | Meaning |
|------------------|---------|
| **-2**           | Timeout expired |
| **11**           | General network error |
| **1205**         | Deadlock victim |
| **11001**        | DNS/connection failure |

These represent the most common transient SQL Server failures in production systems.

---

## Summary

`SqlServerDbClient` + `SqlServerRetryStrategy` provide:

- A ready‑to‑use SQL Server Dapper client  
- Built‑in intelligent retry logic for transient faults  
- Compatibility with your full data‑access stack (mappers, repository, specifications)

Drop them in, wire up a connection factory, and you're ready to go.

