# MnM.Common.Data Core Patterns

This README documents the core data patterns and helper types contained in the attached files:

- **CRUD method enum**
- **Retry strategy abstraction & implementations**
- **Attribute-based column & ignore metadata**
- **Repository wrapper for executing DB actions and specifications**

These types are designed to be used together with the Dapper/DbClient utilities documented in your other README, but they can also be used independently.

---

## Contents

- [CRUD Method Enum](#crud-method-enum)
- [Retry Strategy Abstractions](#retry-strategy-abstractions)
  - `IRetryStrategy`
  - `RetryStrategyBase<TException>`
  - `RetryStrategyByCount`
- [Attribute-Based Mapping Helpers](#attribute-based-mapping-helpers)
  - `ColumnAttribute`
  - `IgnoreAttribute`
  - `IgnoreOnInsertAttribute`
  - `IgnoreOnUpdateAttribute`
- [Repository Abstraction](#repository-abstraction)
  - `IRepository<TClient, TReturn>`
  - `Repository<T, TReturn>`
- [Usage Examples](#usage-examples)
  - Custom retry strategy
  - Using `RetryStrategyByCount`
  - Using CRUD attributes with parameters
  - Using the repository with specifications
- [Notes & Extensibility](#notes--extensibility)

---

## CRUD Method Enum

```csharp
namespace MnM.Common.Data
{
    public enum CrudMethod
    {
        None,
        Insert,
        Update
    }
}
```

The `CrudMethod` enum is used primarily in parameter-building helpers to indicate what kind of operation is being performed. For example, a parameter builder can **ignore certain properties** when performing an `Insert` vs an `Update`. fileciteturn1file0

---

## Retry Strategy Abstractions

### IRetryStrategy

```csharp
public interface IRetryStrategy
{
    int MaxRetryCount { get; }

    T Retry<T>(Func<T> func);
    T Retry<T>(Func<T> func, CancellationToken cancelToken);
    void Retry(Action func);
    void Retry(Action func, CancellationToken cancelToken);

    Task<T> RetryAsync<T>(Func<Task<T>> func);
    Task<T> RetryAsync<T>(Func<Task<T>> func, CancellationToken cancelToken);
    Task RetryAsync(Func<Task> func);
    Task RetryAsync(Func<Task> func, CancellationToken cancelToken);

    IRetryStrategy SetMaxRetryCount(int retryCount);
}
``` 

`IRetryStrategy` defines a **synchronous and asynchronous** retry API around delegates (`Func<T>`, `Action`, and their `Task` equivalents). Implementations encapsulate the logic for:

- How many times to retry (`MaxRetryCount`)
- Which exceptions should trigger a retry
- How (or whether) to delay between retries fileciteturn1file1

This interface is used by higher-level components (like `DbClient`) to apply retries consistently without knowing the details of the backoff behavior.

---

### RetryStrategyBase\<TException\>

```csharp
public abstract class RetryStrategyBase<TException> : IRetryStrategy 
    where TException : Exception
{
    private readonly Func<RetryStrategyBase<TException>, TException, bool> _continueFunc;

    protected RetryStrategyBase(
        Func<RetryStrategyBase<TException>, TException, bool> continueFunc,
        int maxRetryCount = DefaultRetryCount)
    {
        _continueFunc = continueFunc ?? throw new ArgumentNullException(nameof(continueFunc));
        MaxRetryCount = maxRetryCount;
    }

    private const int DefaultRetryCount = 0;
    public int MaxRetryCount { get; private set; }
    public int RetryCount { get; private set; }

    // ... Retry / RetryAsync implementations ...
}
```

`RetryStrategyBase<TException>` provides a reusable implementation of the `IRetryStrategy` interface, where:

- **`TException`** specifies the base exception type that should be caught and retried (e.g., `SqlException`, `TimeoutException`, etc.).
- A `Func<RetryStrategyBase<TException>, TException, bool>` called `_continueFunc` determines **whether to continue retrying** after each failure.
- `RetryCount` is incremented for each retry attempt.
- Both sync and async overloads are implemented in a loop that:
  1. Invokes the function,
  2. Catches `TException`,
  3. Increments `RetryCount`,
  4. Checks cancellation,
  5. Calls `_continueFunc` to decide whether to re-throw or try again. fileciteturn1file2

You typically subclass this type and supply the `_continueFunc` predicate to encode your retry policy.

---

### RetryStrategyByCount

```csharp
public class RetryStrategyByCount : RetryStrategyBase<Exception>
{
    public Func<RetryStrategyBase<Exception>, Exception, bool> RetryByCount 
        = (s, e) => s.RetryCount <= s.MaxRetryCount;

    public RetryStrategyByCount()
        : base((s, ex) => s.RetryCount <= s.MaxRetryCount)
    {
    }

    public RetryStrategyByCount(int maxRetryCount)
        : base((s, ex) => s.RetryCount <= s.MaxRetryCount, maxRetryCount)
    {
    }
}
```

`RetryStrategyByCount` is a concrete `RetryStrategyBase<Exception>` that retries **up to a maximum number of times**, regardless of the exception type. The default behavior is:

```csharp
s => s.RetryCount <= s.MaxRetryCount
```

You can control the max attempts via the constructor or with `SetMaxRetryCount`. fileciteturn1file3

---

## Attribute-Based Mapping Helpers

These attributes live under `MnM.Common.Data.Attributes` and are intended to be used on POCO properties.

### ColumnAttribute

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class ColumnAttribute : Attribute
{
    public ColumnAttribute() { }

    public ColumnAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
```

`ColumnAttribute` is used to:

- Explicitly mark a property as a column
- Optionally give the column a name (`Name`) that may differ from the property name fileciteturn1file6

Your parameter builders or type mappers (e.g., Dapper custom mappers) can read this attribute to determine what column name to use in SQL.

---

### IgnoreAttribute

```csharp
// For explicit pocos, marks property as a column and optionally supplies column name
// For non-explicit pocos, causes a property to be ignored
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreAttribute : Attribute
{
}
```

`IgnoreAttribute` is used to signal that a property should be **ignored by mapping logic** for certain scenarios:

- For **explicit POCOs**, this can be used to mark non-column properties.
- For **non-explicit POCOs**, the comment indicates that this causes a property to be ignored entirely. fileciteturn1file7

The actual behavior depends on the consuming mapper/parameter layer, but the intention is to prevent these properties from being mapped to SQL.

---

### IgnoreOnInsertAttribute

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreOnInsertAttribute : Attribute
{
}
```

Marks a property that should be **ignored when performing an `Insert`**. It is typically checked in conjunction with `CrudMethod.Insert`. fileciteturn1file4

---

### IgnoreOnUpdateAttribute

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnoreOnUpdateAttribute : Attribute
{
}
```

Marks a property that should be **ignored when performing an `Update`**, e.g., audit fields or immutable keys that should not be modified. fileciteturn1file5

---

## Repository Abstraction

### IRepository\<TClient, TReturn\>

```csharp
public interface IRepository<out TClient, TReturn>
    where TClient : class, IDisposable
    where TReturn : class
{
    // Synchronous calls
    void ExecuteDbAction(Action<TClient> action);
    TReturn ExecuteDbAction(Func<TClient, TReturn> action);
    IEnumerable<TReturn> ExecuteDbAction(Func<TClient, IEnumerable<TReturn>> action);

    void ExecuteDbAction(INonQuerySpecification<TClient> specification);
    void ExecuteDbAction(INonQuerySpecification<TClient> specification, CancellationToken cancellationToken);

    TReturn ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification);
    TReturn ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification, CancellationToken cancellationToken);

    IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<TClient, TReturn> specification);
    IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<TClient, TReturn> specification, CancellationToken cancellationToken);

    // Async calls
    Task ExecuteDbActionAsync(Func<TClient, Task> action);
    Task<IEnumerable<TReturn>> ExecuteDbActionAsync(Func<TClient, Task<IEnumerable<TReturn>>> action);

    Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification);
    Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification, CancellationToken cancellationToken);

    Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification);
    Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification, CancellationToken cancellationToken);

    Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<TClient, TReturn> specification);
    Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<TClient, TReturn> specification, CancellationToken cancellationToken);
}
```

`IRepository<TClient, TReturn>` wraps a **client type** (e.g., `IDbClient`, `DbConnection`, etc.) and exposes methods for:

- Executing arbitrary actions / functions that accept the client
- Executing well-defined **specification objects**:
  - `INonQuerySpecification<TClient>`
  - `IQuerySpecification<TClient, TReturn>`
  - `IQueryListSpecification<TClient, TReturn>`
  - Async variants for all of the above fileciteturn1file8

This separates:

- Creation / lifecycle of the client
- Definition of the query (specifications)
- Execution of the query (repository)

---

### Repository\<T, TReturn\>

```csharp
public sealed class Repository<T, TReturn> : IRepository<T, TReturn>
    where T : class, IDisposable
    where TReturn : class
{
    internal readonly Func<T> Factory;

    public Repository(Func<T> factory)
    {
        Factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    // Sync ExecuteDbAction overloads
    // Async ExecuteDbActionAsync overloads
}
```

`Repository<T, TReturn>` is the concrete implementation that:

- Accepts a **factory** delegate (`Func<T>`) that creates the client.
- For each operation:
  - Creates a client via `Factory()`
  - Wraps it in a `using` block to ensure disposal
  - Invokes the delegate or specification against the client fileciteturn1file9

#### Synchronous execution (simplified)

```csharp
public void ExecuteDbAction(Action<T> action)
{
    if (action == null) throw new ArgumentNullException(nameof(action));
    using var client = Factory();
    action(client);
}

public TReturn ExecuteDbAction(Func<T, TReturn> func)
{
    if (func == null) throw new ArgumentNullException(nameof(func));
    using var client = Factory();
    return func(client);
}
```

#### Specification-based execution (simplified)

```csharp
public void ExecuteDbAction(INonQuerySpecification<T> specification)
{
    if (specification == null) throw new ArgumentNullException(nameof(specification));
    ExecuteDbAction(specification.Execute());
}

public TReturn ExecuteDbAction(IQuerySpecification<T, TReturn> specification)
{
    if (specification == null) throw new ArgumentNullException(nameof(specification));
    return ExecuteDbAction(specification.Execute());
}
```

Async counterparts call the async delegates/specifications and return their `Task`s.

---

## Usage Examples

### Example 1 – Custom Retry Strategy

A custom retry strategy that retries **only on specific exceptions** and applies a simple count-based limit:

```csharp
using System;
using System.Data.SqlClient;
using MnM.Common.Data;

public sealed class SqlRetryStrategy : RetryStrategyBase<SqlException>
{
    public SqlRetryStrategy(int maxRetries)
        : base(ShouldRetry, maxRetries)
    {
    }

    private static bool ShouldRetry(RetryStrategyBase<SqlException> state, SqlException ex)
    {
        // Only retry on transient error numbers, and while under the max count
        var isTransient = ex.Errors.Count > 0 && ex.Errors[0].Number is 4060 or 40197 or 40501;
        return isTransient && state.RetryCount <= state.MaxRetryCount;
    }
}
```

Usage:

```csharp
var retry = new SqlRetryStrategy(maxRetries: 3);

var result = retry.Retry(() =>
{
    // some SQL operation that might throw SqlException
    return DoDatabaseWork();
});
```

---

### Example 2 – RetryStrategyByCount with an IDbClient

```csharp
using MnM.Common.Data;
using MnM.Common.Data.Dapper; // assuming your DbClient<TException> lives here

IRetryStrategy retryStrategy = new RetryStrategyByCount(maxRetryCount: 3);

Func<DbConnection> connectionFactory = () => new SqlConnection(connectionString);

var dbClient = new DbClient<DbClientException>(connectionFactory, retryStrategy);

var users = dbClient
    .SetCommandText("SELECT * FROM dbo.Users")
    .ExecuteQuery<UserDto>();
```

Here, any `Exception` thrown from the underlying DB operation will be retried up to `MaxRetryCount` times before bubbling up.

---

### Example 3 – Combining CRUD Attributes with Parameter Builders

Given a POCO:

```csharp
using MnM.Common.Data;
using MnM.Common.Data.Attributes;

public class UserModel
{
    [Column("user_id")]
    [IgnoreOnInsert] // key is generated by the DB on insert
    public int Id { get; set; }

    [Column("user_name")]
    public string Name { get; set; }

    [Ignore] // never send to DB
    public string DebugInfo { get; set; }
}
```

You can use the `CrudMethod` together with your parameter builder to **only include appropriate properties** for each operation:

```csharp
// Insert (Id ignored because of IgnoreOnInsert)
dbClient
    .SetCommandText("INSERT INTO dbo.Users (user_name) VALUES (@user_name);")
    .AddNamedParameters(model, CrudMethod.Insert)
    .ExecuteNonQuery();

// Update (Id included, used in WHERE clause)
dbClient
    .SetCommandText("UPDATE dbo.Users SET user_name = @user_name WHERE user_id = @user_id;")
    .AddNamedParameters(model, CrudMethod.Update)
    .ExecuteNonQuery();
```

The builder would inspect:

- `ColumnAttribute` to derive parameter names.
- `IgnoreAttribute` to exclude `DebugInfo`.
- `IgnoreOnInsertAttribute` / `IgnoreOnUpdateAttribute` to conditionally ignore `Id`. fileciteturn1file0turn1file4turn1file5turn1file6turn1file7

---

### Example 4 – Using the Repository with Specifications

Assume you have an `IDbClient` (from your Dapper wrapper) and specification interfaces in `MnM.Common.Data.Specifications`.

```csharp
using System;
using System.Collections.Generic;
using MnM.Common.Data.Repositories;

public class GetUserByIdSpecification : IQuerySpecification<IDbClient, UserDto>
{
    private readonly int _id;

    public GetUserByIdSpecification(int id)
    {
        _id = id;
    }

    public Func<IDbClient, UserDto> Execute()
    {
        return client =>
        {
            var users = client
                .SetCommandText("SELECT user_id, user_name FROM dbo.Users WHERE user_id = @Id")
                .AddNamedParameters(new { Id = _id })
                .ExecuteQuery<UserDto>();

            return users.SingleOrDefault();
        };
    }
}
```

Now you can wire up a repository:

```csharp
Func<IDbClient> clientFactory = () => new DbClient<DbClientException>(connectionFactory, retryStrategy);

var repo = new Repository<IDbClient, UserDto>(clientFactory);

UserDto user = repo.ExecuteDbAction(new GetUserByIdSpecification(123));
```

For async usage, implement `IQuerySpecificationAsync<IDbClient, UserDto>` and call the corresponding `ExecuteDbActionAsync` overload.

---

## Notes & Extensibility

- **Retry strategies**  
  - Use `RetryStrategyBase<TException>` when you want to express a policy based on exception types and retry counts.
  - Use `RetryStrategyByCount` as a quick, generic retry-by-count implementation.
  - Compose retries with higher-level abstractions such as `IDbClient`.

- **Attributes & CRUD**  
  - `ColumnAttribute`, `IgnoreAttribute`, `IgnoreOnInsertAttribute`, and `IgnoreOnUpdateAttribute` are metadata; their behavior is fully realized by your parameter/type-mapping layers.
  - They are especially useful in combination with a `CrudMethod`-aware parameter builder.

- **Repository & Specifications**  
  - `Repository<T, TReturn>` centralizes client creation and disposal via `Factory`.
  - Specifications encapsulate **what** to query, while the repository encapsulates **how** and **when** to execute against a client.

These components together provide a clean, composable infrastructure layer for data access, retry behavior, and metadata-driven mapping.

