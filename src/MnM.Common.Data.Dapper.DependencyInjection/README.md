# MnM.Common.Data.Dapper.DependencyInjection

This README documents the **dependency injection helpers** for wiring up:

- A Dapper-based repository using `IRepository<TClient, TReturn>`
- A client factory for your `IDbClient` implementation
- Attribute-based column mapping via `ColumnAttributeTypeMapper<T>` with optional case sensitivity

These helpers are meant to be used together with the core `Repository<TClient, TReturn>` and `IDbClient` / Dapper utilities you already have in this project. fileciteturn2file0L1-L9

---

## Files

- `DapperRepositoryConfigurationOptions.cs`  
  Simple options class for configuring how the Dapper repository is registered (client factory and case sensitivity). fileciteturn2file0L1-L9
- `DependencyInjection.cs`  
  Extension methods on `IServiceCollection` for registering:
  - `IRepository<TClient, TReturn>` / `Repository<TClient, TReturn>`
  - A `Func<TClient>` client factory
  - A Dapper column mapper for `TReturn` via `ColumnAttributeTypeMapper<TReturn>` fileciteturn2file1L1-L12

All types live in the `MnM.Common.Data.Dapper.DependencyInjection` namespace. fileciteturn2file0L3-L7

---

## DapperRepositoryConfigurationOptions\<TClient>

```csharp
public class DapperRepositoryConfigurationOptions<TClient>
    where TClient : class, IDbClient, IDisposable
{
    public bool CaseSensitiveColumnMapping { get; set; }
    public Func<TClient> ClientFactory { get; set; } = () => throw new NotImplementedException();
}
```

- **`TClient`** – your `IDbClient` implementation type (must be `class`, implement `IDbClient`, and `IDisposable`).  
- **`CaseSensitiveColumnMapping`** – controls whether column mapping is case-sensitive when configuring Dapper’s type map.  
- **`ClientFactory`** – a `Func<TClient>` that creates instances of your client (e.g., your Dapper `DbClient` wrapper). This must be provided; the default throws `NotImplementedException`. fileciteturn2file0L3-L9

You typically provide this options instance when calling `AddDapperRepository`.

---

## DependencyInjection Extension Methods

All DI helpers are defined in the `DependencyInjection` static class and extend `IServiceCollection`. fileciteturn2file1L6-L13

### AddColumnMapper\<TReturn>

```csharp
public static void AddColumnMapper<TReturn>(bool caseSensitive = false)
    where TReturn : class
{
    D.SqlMapper.SetTypeMap(
        typeof(TReturn),
        new ColumnAttributeTypeMapper<TReturn>(caseSensitive));
}
```

Registers a Dapper `SqlMapper.ITypeMap` for the `TReturn` type using `ColumnAttributeTypeMapper<TReturn>`:

- `TReturn` – your DTO/POCO to be mapped from query results.
- `caseSensitive` – if `true`, column matching is case-sensitive; otherwise case-insensitive.  
- Uses Dapper’s `SqlMapper.SetTypeMap` under the hood, so all future queries that map to `TReturn` will respect the `[Column]` attributes and case-sensitivity rules. fileciteturn2file1L8-L18

> Note: `ColumnAttributeTypeMapper<TReturn>` is part of your existing Dapper utilities and understands attributes like `[Column]`, `[Ignore]`, `[IgnoreOnInsert]`, `[IgnoreOnUpdate]`.

---

### AddDapperRepository\<TClient, TReturn>(Func\<TClient> clientFactory)

```csharp
public static IServiceCollection AddDapperRepository<TClient, TReturn>(
    this IServiceCollection @this,
    Func<TClient> clientFactory)
    where TClient : class, IDbClient, IDisposable
    where TReturn : class
{
    return @this.AddDapperRepository<TClient, TReturn>(
        new DapperRepositoryConfigurationOptions<TClient>
        {
            ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory)),
            CaseSensitiveColumnMapping = false
        });
}
```

Convenience overload that:

- Uses the supplied `clientFactory` to create `TClient` instances.  
- Configures `CaseSensitiveColumnMapping = false` (case-insensitive) by default.  
- Delegates to the more general `AddDapperRepository` overload that takes `DapperRepositoryConfigurationOptions<TClient>`. fileciteturn2file1L20-L33

### AddDapperRepository\<TClient, TReturn>(DapperRepositoryConfigurationOptions\<TClient> options)

```csharp
public static IServiceCollection AddDapperRepository<TClient, TReturn>(
    this IServiceCollection @this,
    DapperRepositoryConfigurationOptions<TClient> options)
    where TClient : class, IDbClient, IDisposable
    where TReturn : class
{
    if (@this == null) throw new ArgumentNullException(nameof(@this));
    if (options == null) throw new ArgumentNullException(nameof(options));
    if (options.ClientFactory == null)
        throw new ArgumentException("ClientFactory cannot be null", nameof(options));

    @this.AddSingleton(options.ClientFactory
        ?? throw new ArgumentException("ClientFactory cannot be null", nameof(options)));
    @this.AddScoped<IRepository<TClient, TReturn>, Repository<TClient, TReturn>>();
    AddColumnMapper<TReturn>(options.CaseSensitiveColumnMapping);
    return @this;
}
```

This overload gives you full control:

- Validates `IServiceCollection`, `options`, and `options.ClientFactory`.  
- Registers the client factory (`Func<TClient>`) as a **singleton** in the container.  
- Registers `IRepository<TClient, TReturn>` with a **scoped** lifetime, implemented by `Repository<TClient, TReturn>`.  
- Calls `AddColumnMapper<TReturn>` using `options.CaseSensitiveColumnMapping` to configure Dapper’s type map. fileciteturn2file1L35-L50

---

## Usage Examples

### Example 1 – Registering a Dapper Repository in ASP.NET Core

Assume you have:

- A Dapper client class `AppDbClient` that implements `IDbClient` and `IDisposable`.
- A DTO `UserDto` that is mapped from queries.

```csharp
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MnM.Common.Data.Dapper;
using MnM.Common.Data.Dapper.DependencyInjection;
using MnM.Common.Data.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserRepository(
        this IServiceCollection services,
        string connectionString,
        IRetryStrategy retryStrategy)
    {
        // Client factory creates an AppDbClient (IDbClient wrapper)
        Func<AppDbClient> clientFactory = () =>
        {
            DbConnection connection = new SqlConnection(connectionString);
            return new AppDbClient(connection, retryStrategy);
        };

        // Registers:
        // - Func<AppDbClient> as singleton
        // - IRepository<AppDbClient, UserDto> as scoped
        // - ColumnAttributeTypeMapper<UserDto> with case-insensitive mapping
        services.AddDapperRepository<AppDbClient, UserDto>(clientFactory);

        return services;
    }
}
```

Now you can inject the repository wherever you need it:

```csharp
public class UserService
{
    private readonly IRepository<AppDbClient, UserDto> _repository;

    public UserService(IRepository<AppDbClient, UserDto> repository)
    {
        _repository = repository;
    }

    public UserDto GetUserById(int id)
    {
        // Use your existing repository + specifications infrastructure
        var spec = new GetUserByIdSpecification(id);
        return _repository.ExecuteDbAction(spec);
    }
}
```

This takes advantage of your existing `Repository<TClient, TReturn>` implementation and specification pattern.

---

### Example 2 – Using DapperRepositoryConfigurationOptions for Case-Sensitive Mapping

If your database column names must match case exactly, you can enable case-sensitive column mapping:

```csharp
using Microsoft.Extensions.DependencyInjection;
using MnM.Common.Data.Dapper.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaseSensitiveUserRepository(
        this IServiceCollection services,
        Func<AppDbClient> clientFactory)
    {
        var options = new DapperRepositoryConfigurationOptions<AppDbClient>
        {
            ClientFactory = clientFactory,
            CaseSensitiveColumnMapping = true // enable case-sensitive mapping
        };

        services.AddDapperRepository<AppDbClient, UserDto>(options);
        return services;
    }
}
```

This will:

- Register `Func<AppDbClient>` as singleton.
- Register `IRepository<AppDbClient, UserDto>` as scoped.
- Register `ColumnAttributeTypeMapper<UserDto>` with `caseSensitive = true`. fileciteturn2file0L5-L9turn2file1L35-L50

---

### Example 3 – Registering a Column Mapper Without the Repository

If you only want to configure Dapper’s column mapping for a type and don’t need the repository registration, you can call `AddColumnMapper` directly at startup:

```csharp
using Microsoft.Extensions.DependencyInjection;
using MnM.Common.Data.Dapper.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUserColumnMapping(
        this IServiceCollection services)
    {
        // This configures Dapper to map UserDto using [Column] attributes
        // and case-insensitive column matching.
        DependencyInjection.AddColumnMapper<UserDto>(caseSensitive: false);

        return services;
    }
}
```

Even though `AddColumnMapper` doesn’t directly extend `IServiceCollection`, you can call it as part of your composition root (Program.cs, Startup.cs, or a registration extension) to ensure Dapper is configured once at application startup. fileciteturn2file1L8-L18

---

## Summary

- Use **`DapperRepositoryConfigurationOptions<TClient>`** when you need advanced control over the client factory and column case sensitivity.
- Call **`AddDapperRepository<TClient, TReturn>`** to register:
  - `Func<TClient>` (client factory) as a singleton
  - `IRepository<TClient, TReturn>` (backed by `Repository<TClient, TReturn>`) as scoped
  - Dapper’s type map for `TReturn` via `ColumnAttributeTypeMapper<TReturn>`
- Use **`AddColumnMapper<TReturn>`** to configure Dapper’s attribute-based mapping even without the repository.

These helpers give you a clean, centralized way to compose your data access stack with Dapper, `IDbClient`, and the repository/specification patterns you already have in your solution.

