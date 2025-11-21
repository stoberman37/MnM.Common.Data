# Elasticsearch Repository Dependency Injection  
Seamless DI Integration for Elasticsearch Repositories

This module provides **dependency injection (DI) extensions** and **configuration options** that enable the MnM.Common.Data repository pattern to work with Elasticsearch.  
It wires up:

- An `ElasticsearchClient` (from Elastic.Clients.Elasticsearch)
- A retry-enabled `CommonElasticsearchClient`
- A scoped generic repository: `IRepository<ICommonElasticsearchClient, TReturn>`

---

## 📦 Components Included

---

## 1. ElasticsearchRepositoryConfigurationOptions

Configuration object used during DI registration:

```csharp
public class ElasticsearchRepositoryConfigurationOptions
{
    public ElasticsearchClientSettings ElasticsearchClientSettings { get; set; }
    public Func<IRetryStrategy> RetryStrategy { get; set; } = () => new RetryStrategyByCount(0);
}
```

Properties:

| Property | Description |
|---------|-------------|
| **ElasticsearchClientSettings** | Required settings object for the underlying `ElasticsearchClient` |
| **RetryStrategy** | Factory for a retry policy injected into `CommonElasticsearchClient` |

Default retry strategy uses `RetryStrategyByCount(0)`.

---

## 2. DependencyInjection.AddElasticsearchRepository

Registers all Elasticsearch-related services into an `IServiceCollection`.

```csharp
services.AddElasticsearchRepository<TReturn>(options);
```

### What the DI method registers

| Service | Lifetime | Description |
|--------|----------|-------------|
| `ElasticsearchClient` | Singleton | Created from the provided `ElasticsearchClientSettings` |
| `Func<ElasticsearchClient>` | Singleton | Factory wrapper used by `CommonElasticsearchClient` |
| `Func<ICommonElasticsearchClient>` | Singleton | Creates retry-enabled client instances |
| `IRepository<ICommonElasticsearchClient, TReturn>` | Scoped | Generic repository for the return type |

### Required validation

The DI method ensures:

- `options` is not null  
- `ElasticsearchClientSettings` is provided  
- `RetryStrategy` is provided  

---

## 🚀 Usage Example

### 1. Configure DI

```csharp
services.AddElasticsearchRepository<MyDocument>(new ElasticsearchRepositoryConfigurationOptions
{
    ElasticsearchClientSettings = new ElasticsearchClientSettings(new Uri("http://localhost:9200")),
    RetryStrategy = () => new RetryStrategyByCount(maxRetryCount: 3)
});
```

This registers everything needed to query Elasticsearch through the repository pattern.

---

### 2. Using the Repository

```csharp
public class MyService
{
    private readonly IRepository<ICommonElasticsearchClient, MyDocument> _repo;

    public MyService(IRepository<ICommonElasticsearchClient, MyDocument> repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<MyDocument>> SearchAsync(string term)
    {
        return _repo.ExecuteDbActionAsync(client =>
            client.SearchAsync(new SearchRequestDescriptor<MyDocument>()
                .Index("my-index")
                .Query(q => q.Match(m => m.Field(f => f.Name).Query(term)))));
    }
}
```

---

## ✔ Benefits

- Clean integration into ASP.NET Core DI
- Provides a retry-enabled Elasticsearch client
- Zero manual wiring for client construction
- Works seamlessly with generic repository + spec pattern
- Fully testable via interface abstractions

---

## 📌 Summary

This module serves as a bridge between:

- The Elastic .NET Client  
- Your retry strategy framework  
- The MnM.Common.Data repository pattern  

With only a few lines of DI configuration, your application gains a robust, resilient Elasticsearch data access layer.

