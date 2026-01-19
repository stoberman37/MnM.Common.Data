# Common Elasticsearch Client
A Lightweight, Retry‑Aware Wrapper Around the Official Elasticsearch .NET Client

This project provides a simple but powerful abstraction for executing Elasticsearch queries using a clean, testable interface and built‑in retry logic. It wraps the official **Elastic.Clients.Elasticsearch** SDK and exposes common operations such as searching and retrieving documents.

---

## ✨ Features

- Unified `ICommonElasticsearchClient` interface
- Retry support via your existing `IRetryStrategy` implementations
- Async‑only API (best practice for Elasticsearch)
- Supports both descriptor‑based and request‑based calls
- Safe null handling for invalid responses
- Fully disposable client wrapper
- Easily testable (interface‑based, pluggable factories)

---

## 📦 Included Components

### **1. ICommonElasticsearchClient**

Defines all supported operations:

```csharp
public interface ICommonElasticsearchClient : IDisposable
{
    Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search);
    Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search, CancellationToken token);

    Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search);
    Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search, CancellationToken token);

    Task<T> GetAsync<T>(GetRequestDescriptor<T> get);
    Task<T> GetAsync<T>(GetRequestDescriptor<T> get, CancellationToken token);

    Task<T> GetAsync<T>(GetRequest get);
    Task<T> GetAsync<T>(GetRequest get, CancellationToken token);
}
```

Supports:
- Search by descriptor
- Search using typed `SearchRequest<T>`
- Get by descriptor
- Get using typed `GetRequest`

All generic methods constrain `T` to `class`.

---

### **2. CommonElasticsearchClient**

The concrete implementation.  
Key behaviors:

- Accepts a factory: `Func<ElasticsearchClient>`
- Accepts (optional) custom retry strategy
- Wraps native Elasticsearch client calls in retry logic
- Validates inputs and returns `null` when responses are not valid
- Implements disposable pattern

Example constructor usage:

```csharp
var client = new CommonElasticsearchClient(
    () => new ElasticsearchClient(settings),
    new RetryStrategyByCount(maxRetries: 3));
```

Every search/get call is executed like:

```csharp
await _retryStrategy.RetryAsync(() => _client.SearchAsync(...));
```

---

## 🔍 Supported Operations

### 1. Search

Search using descriptor:

```csharp
var results = await client.SearchAsync(new SearchRequestDescriptor<MyDoc>(...)
    .Index("my-index")
    .Query(q => q.Match(m => m.Field(f => f.Name).Query("abc"))));
```

Search using request:

```csharp
var request = new SearchRequest<MyDoc>("my-index")
{
    Query = new Query(new MatchQuery("name") { Query = "abc" })
};

var results = await client.SearchAsync(request);
```

### 2. Get Document

Using descriptor:

```csharp
var doc = await client.GetAsync(new GetRequestDescriptor<MyDoc>("my-index", id));
```

Using request object:

```csharp
var doc = await client.GetAsync<MyDoc>(new GetRequest("my-index", id));
```

---

## 🧱 Retry Strategy Integration

You can plug in any retry strategy that implements your existing `IRetryStrategy` interface:

```csharp
new RetryStrategyByCount(maxRetryCount: 5)
```

Every Elasticsearch call is wrapped via:

```csharp
_retryStrategy.RetryAsync(() => _client.SearchAsync(...));
```

This ensures resilient querying against transient network or cluster issues.

---

## 🧪 Example: Registering in Dependency Injection

```csharp
services.AddSingleton<ICommonElasticsearchClient>(sp =>
    new CommonElasticsearchClient(
        () => new ElasticsearchClient(esSettings),
        new RetryStrategyByCount(3)));
```

---

## 🗑 Disposal Behavior

`CommonElasticsearchClient` implements the standard .NET disposable pattern.  
On dispose:

- Internal `_client` reference is cleared
- Finalizer suppression is used for performance

---

## ✅ Summary

`CommonElasticsearchClient` provides:

- A clean wrapper over Elasticsearch’s .NET client
- Retry‑protected search and get operations
- A testable and disposable abstraction
- Async‑only, high‑performance access patterns

This module fits naturally into the broader MnM.Common.Data ecosystem, providing consistency across data technologies (SQL Server, DynamoDB, Elasticsearch, etc.).

