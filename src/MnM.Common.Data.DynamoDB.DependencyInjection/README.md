# DynamoDB Repository Dependency Injection  
Seamless DI Integration for DynamoDB Repositories

This README documents how to integrate AWS DynamoDB operations into the generic repository pattern used across the `MnM.Common.Data` ecosystem.  
It covers:

- `DynamoDBRepositoryConfigurationOptions`
- `DependencyInjection.AddDynamoDBRepository`
- How these components register and produce a fully functional DynamoDB-backed repository.

---

## 📁 Components in This Module

### 1. DynamoDBRepositoryConfigurationOptions

Configuration options for wiring up the DynamoDB repository.

```csharp
public class DynamoDBRepositoryConfigurationOptions
{
    public Func<IDynamoDBContext> DynamoDBContext { get; set; }
    public Func<IRetryStrategy> RetryStrategy { get; set; }
}
```

Source:  
fileciteturn5file1

This defines two required delegates:

| Property | Purpose |
|----------|---------|
| **DynamoDBContext** | Factory producing an `IDynamoDBContext` instance |
| **RetryStrategy** | Factory for the retry strategy used by `DynamoDBClient<T>` |

Defaults exist, but both should usually be overridden.

---

### 2. DependencyInjection.AddDynamoDBRepository

Registers the repository, client factory, and DynamoDB context factory into `IServiceCollection`.

```csharp
public static IServiceCollection AddDynamoDBRepository<TReturn, TKey>(
    this IServiceCollection @this,
    DynamoDBRepositoryConfigurationOptions options)
```

Source:  
fileciteturn5file0

**What it registers:**

| Service | Lifetime | Description |
|---------|----------|-------------|
| `Func<IDynamoDBContext>` | Singleton | Creates AWS DynamoDB context |
| `Func<IDynamoDBClient<TReturn>>` | Singleton | Creates `DynamoDBClient<TReturn>` with retry strategies |
| `IRepository<IDynamoDBClient<TReturn>, TReturn>` | Scoped | Generic repository implementation |

**Validation:**

- Rejects null `options`
- Ensures both `DynamoDBContext` and `RetryStrategy` factories are provided

---

## ⚙ Example: Registering DynamoDB Repository

```csharp
services.AddDynamoDBRepository<MyItem, string>(new DynamoDBRepositoryConfigurationOptions
{
    DynamoDBContext = () => new DynamoDBContext(dynamoClient),
    RetryStrategy = () => new RetryStrategyByCount(maxRetryCount: 5)
});
```

---

## 💡 Using the Repository in Application Code

```csharp
public class MyService
{
    private readonly IRepository<IDynamoDBClient<MyItem>, MyItem> _repo;

    public MyService(IRepository<IDynamoDBClient<MyItem>, MyItem> repo)
    {
        _repo = repo;
    }

    public Task<MyItem> GetItemAsync(string id)
    {
        return _repo.ExecuteDbActionAsync(client => client.ReadAsync(id));
    }
}
```

This achieves:

- Clean DI injection
- Full retry protection
- Testable client abstractions
- Clear separation between domain logic and data access

---

## ✔ Key Benefits

- 🔧 Clean DI integration that avoids manual wiring  
- 🔁 Consistent retry behavior through `IRetryStrategy`  
- 🧪 Testable thanks to interface-based abstractions  
- 🔌 Works seamlessly with the repository & specification pattern  

---

## 📌 Summary

This module provides an elegant bridge between AWS DynamoDB and your generic repository pattern.  
With only a few lines of DI configuration, your application gains a robust, retry-enabled DynamoDB data access layer.

