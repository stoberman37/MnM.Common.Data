# MnM.Common.Data.Dapper Utilities

This folder contains small helper types that sit on top of **Dapper** to provide:

- A fluent, retry-aware `IDbClient` wrapper around `DbConnection`
- Centralized parameter management (including output parameters)
- Attribute-based column mapping using `[Column]`
- A fallback type mapper that can chain multiple `ITypeMap` implementations

The code is intended to be lightweight and unobtrusive while offering a consistent pattern for executing commands and queries in a transactional, retryable way.

---

## Contents

- `IDbClient` – public interface for building and executing commands
- `DbClient<TException>` – implementation of `IDbClient` with retry support
- `DbClientException` – simple custom exception type used by `DbClient`
- `ParameterManager` – manages named and `IDataParameter`-based parameters
- `FallbackTypeMapper` – chains multiple Dapper `ITypeMap` implementations
- `ColumnAttributeTypeMapper` – maps columns using `[Column(Name = ...)]` attributes

---

## Dependencies

These helpers assume the following packages and types are available:

- [`Dapper`](https://github.com/DapperLib/Dapper)
- A retry strategy type implementing `IRetryStrategy` (with `Retry` / `RetryAsync` methods)
- Attribute types from `MnM.Common.Data.Attributes`:
  - `ColumnAttribute`
  - `IgnoreAttribute`
  - `IgnoreOnInsertAttribute`
  - `IgnoreOnUpdateAttribute`
- An enum `CrudMethod` with at least: `None`, `Insert`, `Update`

You will also need your own ADO.NET provider (`SqlConnection`, `NpgsqlConnection`, etc.).

---

## Quick Start

### 1. Configure Dapper column mapping (optional but recommended)

If you use `[Column]` attributes to map CLR properties to database column names, you can register the `ColumnAttributeTypeMapper<T>` at application startup:

```csharp
using Dapper;
using MnM.Common.Data.Dapper;

public static void ConfigureDapper()
{
    SqlMapper.SetTypeMap(typeof(UserDto), new ColumnAttributeTypeMapper<UserDto>());
}
```

```csharp
using MnM.Common.Data.Attributes;

public class UserDto
{
    [Column("user_id")]
    public int Id { get; set; }

    [Column("user_name")]
    public string Name { get; set; }
}
```

`ColumnAttributeTypeMapper<T>` will first try to resolve members using the `ColumnAttribute.Name` value, and fall back to Dapper's default mapping if no such attribute is present.

---

### 2. Create a `DbClient` instance

Typically, you create a `DbClient<TException>` using a connection factory and your retry strategy implementation:

```csharp
using System.Data.Common;
using System.Data.SqlClient;
using MnM.Common.Data.Dapper;

Func<DbConnection> connectionFactory = () =>
    new SqlConnection(connectionString);

// Example retry strategy; implement IRetryStrategy in your own code.
IRetryStrategy retryStrategy = new ExponentialBackoffRetryStrategy(
    maxRetryCount: 3,
    initialDelay: TimeSpan.FromSeconds(1));

var dbClient = new DbClient<DbClientException>(connectionFactory, retryStrategy);
```

- `connectionFactory` is invoked whenever a command is executed.
- `retryStrategy` is used for all calls to `ExecuteQuery`, `ExecuteNonQuery`, and their async equivalents.

---

## Fluent API Overview

All configuration methods return `IDbClient` for fluent chaining:

```csharp
dbClient
    .SetCommandText("SELECT * FROM Users WHERE Id = @Id")
    .SetCommandType(CommandType.Text)
    .SetCommandTimeout(30)
    .SetMaxRetryCount(5)
    .AddNamedParameters(new { Id = 42 })
    .ExecuteQuery<UserDto>();
```

### Common methods

- `SetCommandText(string)` – sets the SQL text or stored procedure name.
- `SetCommandType(CommandType)` – `Text`, `StoredProcedure`, etc.
- `SetCommandTimeout(int)` – command timeout in seconds.
- `SetMaxRetryCount(int)` – sets the max retry count via the injected `IRetryStrategy`.
- `AddNamedParameters(object)` – adds properties of the object as parameters.
- `AddNamedParameters(object, CrudMethod)` – same as above but respects ignore attributes per CRUD operation.
- `AddDbParameter(IDataParameter)` / `AddDbParameters(IEnumerable<IDataParameter>)` – adds explicit ADO.NET parameters.
- `PushCommand()` – pushes the current command into an internal list to build multi-command batches.
- `ExecuteQuery<T>()` / `ExecuteQueryAsync<T>()`
- `ExecuteQuery<TConcrete, TReturn>()` / async variant
- `ExecuteNonQuery()` / `ExecuteNonQueryAsync()`

---

## Examples

### Example 1 – Simple SELECT

```csharp
using System.Data;
using MnM.Common.Data.Dapper;

public async Task<IReadOnlyList<UserDto>> GetUsersAsync(
    IDbClient dbClient,
    CancellationToken cancellationToken)
{
    var users = await dbClient
        .SetCommandText("SELECT user_id, user_name FROM dbo.Users")
        .SetCommandType(CommandType.Text)
        .ExecuteQueryAsync<UserDto>(
            isolationLevel: IsolationLevel.ReadCommitted,
            cancellationToken: cancellationToken);

    return users.ToList();
}
```

---

### Example 2 – Parameterized SELECT with named parameters

Using `AddNamedParameters` allows you to supply an anonymous or POCO object whose properties become parameter values:

```csharp
public UserDto GetUserById(IDbClient dbClient, int id)
{
    var users = dbClient
        .SetCommandText("SELECT user_id, user_name FROM dbo.Users WHERE user_id = @Id")
        .AddNamedParameters(new { Id = id })
        .ExecuteQuery<UserDto>();

    return users.SingleOrDefault();
}
```

If your parameter object uses `[Column]` attributes, the parameter names will use the attribute `Name` rather than the property name, unless ignored by `Ignore*` attributes for a particular `CrudMethod`.

---

### Example 3 – INSERT / UPDATE with CRUD-specific ignore attributes

```csharp
using MnM.Common.Data.Attributes;

public class UserUpdateModel
{
    [Column("user_id")]
    [IgnoreOnInsert]
    public int Id { get; set; }

    [Column("user_name")]
    public string Name { get; set; }

    [Ignore] // never send this value as a parameter
    public string SomeTransientProperty { get; set; }
}

public int UpdateUser(IDbClient dbClient, UserUpdateModel model)
{
    const string sql = @"
UPDATE dbo.Users
SET user_name = @user_name
WHERE user_id = @user_id;";

    return dbClient
        .SetCommandText(sql)
        .AddNamedParameters(model, CrudMethod.Update)
        .ExecuteNonQuery();
}
```

In this example:

- `IgnoreOnInsert` prevents `Id` from being sent during inserts but allows it for updates.
- `Ignore` prevents a property from ever being turned into a parameter.

---

### Example 4 – Multiple commands in a single transaction

`PushCommand()` lets you queue up multiple commands which will all be executed in one transaction via `ExecuteNonQuery` / `ExecuteNonQueryAsync`:

```csharp
public int TransferFunds(IDbClient dbClient, int fromAccountId, int toAccountId, decimal amount)
{
    const string withdrawSql = @"
UPDATE Accounts
SET Balance = Balance - @Amount
WHERE AccountId = @FromAccountId;";

    const string depositSql = @"
UPDATE Accounts
SET Balance = Balance + @Amount
WHERE AccountId = @ToAccountId;";

    return dbClient
        .SetCommandText(withdrawSql)
        .AddNamedParameters(new { Amount = amount, FromAccountId = fromAccountId })
        .PushCommand() // queue first command
        .SetCommandText(depositSql)
        .AddNamedParameters(new { Amount = amount, ToAccountId = toAccountId })
        .PushCommand() // queue second command
        .ExecuteNonQuery(); // runs both commands inside a transaction
}
```

All queued commands share:

- The same connection
- The same transaction (`IsolationLevel` is configurable via the method call)
- The configured retry strategy

---

### Example 5 – Working with output parameters

`ParameterManager` and `DbClient` support input/output and output parameters through `AddDbParameter` and `ExtractOutputParameters`.

```csharp
using System.Data;
using System.Data.SqlClient;

public int InsertUserWithIdentity(IDbClient dbClient, string name)
{
    const string sql = @"
INSERT INTO dbo.Users (user_name)
VALUES (@Name);

SET @NewId = SCOPE_IDENTITY();";

    var idParam = new SqlParameter("@NewId", SqlDbType.Int)
    {
        Direction = ParameterDirection.Output
    };

    dbClient
        .SetCommandText(sql)
        .AddNamedParameters(new { Name = name })
        .AddDbParameter(idParam)
        .ExecuteNonQuery();

    // After execution, ParameterManager extracts the output value back into idParam.Value
    return (int)idParam.Value;
}
```

Only supported `DbType` values are extracted; unsupported types will result in `NotImplementedException` if used as output parameters.

---

### Example 6 – Async queries with cancellation and isolation level

```csharp
using System.Threading;
using System.Threading.Tasks;
using System.Data;

public async Task<IReadOnlyList<UserDto>> GetUsersAsync(
    IDbClient dbClient,
    CancellationToken cancellationToken)
{
    var results = await dbClient
        .SetCommandText("SELECT user_id, user_name FROM dbo.Users")
        .ExecuteQueryAsync<UserDto>(
            isolationLevel: IsolationLevel.ReadCommitted,
            cancellationToken: cancellationToken);

    return results.ToList();
}
```

Async commands use `IRetryStrategy.RetryAsync`, open the connection asynchronously, and wrap each operation in a transaction.

---

## Error Handling

`DbClient<TException>` expects a generic exception type that you own (e.g., `DbClientException`), but it will also allow database-specific exceptions (e.g., `SqlException`) to bubble up as appropriate.

Typical failure scenarios:

- Invalid SQL / parameters → provider-specific exception
- Transaction issues → provider-specific exception
- Missing mapping for output parameter type → `NotImplementedException`

You can wrap calls to `IDbClient` in your own higher-level error handling or logging as needed.

---

## Fallback Mapping Behavior

`FallbackTypeMapper` implements `SqlMapper.ITypeMap` and simply tries each inner mapper in order until one returns a non-null result for:

- `FindConstructor`
- `GetConstructorParameter`
- `GetMember`
- `FindExplicitConstructor`

`ColumnAttributeTypeMapper` composes a `CustomPropertyTypeMap` (which uses `[Column]` attributes) with a `DefaultTypeMap` inside a `FallbackTypeMapper`. This gives you attribute-based mapping while still benefiting from Dapper's default behavior when no attribute is present.

---

## Extensibility Tips

- Implement your own `IRetryStrategy` to customize backoff, max retry count, and which exceptions are retried.
- Subclass or wrap `DbClient<TException>` if you need to:
  - Enforce specific command timeouts
  - Add logging around each execution
  - Inject additional behavior around transactions
- Add more helper methods (e.g., `ExecuteScalar<T>`) using the same patterns if needed.

---

## License

This README does not include a license file. Make sure to add an appropriate license for your project as needed.
