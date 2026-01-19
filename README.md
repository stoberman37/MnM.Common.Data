# MnM.Common.Data – Unified Data Access Infrastructure

MnM.Common.Data is a **cross-technology, extensible, retry-aware data access framework** designed to provide a consistent and testable architecture across multiple storage systems:

- SQL Server / Dapper
- AWS DynamoDB
- Elasticsearch
- Generic Repository Pattern
- Retry Strategy Framework
- Attribute-Based Mapping Utilities
- Fully Supported Dependency Injection Modules

This root README provides a high-level overview of the library and links to individual subsystem documentation.

---

# ✨ Key Features

### 🔁 Unified Retry Infrastructure
Consistent retry behavior across:
- SQL Server (`SqlServerRetryStrategy`)
- DynamoDB (`RetryStrategyByCount`)
- Elasticsearch (`RetryStrategyByCount`)
- Custom retry strategies via `IRetryStrategy` and `RetryStrategyBase<TException>`

### 📦 Client Abstractions
Each technology exposes a high-level client:
- `IDbClient` → SQL Server (via Dapper)
- `IDynamoDBClient<T>` → DynamoDB
- `ICommonElasticsearchClient` → Elasticsearch

### 🧱 Repository + Specification Pattern
Framework-wide support for:
- `IRepository<TClient, TReturn>`
- Query/NonQuery specifications (sync + async)

This ensures the application layer is independent of any specific backend.

### 🧩 Dependency Injection Modules
First-class DI modules for each technology:
- `AddDapperRepository`
- `AddDynamoDBRepository`
- `AddElasticsearchRepository`

### 🗺 Attribute-Based Mapping (SQL)
Includes:
- `[Column]`, `[Ignore]`, `[IgnoreOnInsert]`, `[IgnoreOnUpdate]`
- `ColumnAttributeTypeMapper`, `FallbackTypeMapper`
- Automatic parameter and property mapping

---

# 📂 Repository Structure

Below is a summary of each subsystem and links to detailed documentation.

---

## 1. **SQL Server / Dapper Layer**

Includes:
- `DbClient<TException>`
- `SqlServerDbClient`
- Parameter and CRUD mapping utilities
- Dapper type-mapping enhancements
- DI extensions

📘 Documentation:
- [src/MnM.Common.Data.Dapper/README.md](src/MnM.Common.Data.Dapper/README.md)
- [src/MnM.Common.Data.Dapper.SqlServer/README.md](src/MnM.Common.Data.Dapper.SqlServer/README.md)
- [src/MnM.Common.Data.Dapper.DependencyInjection/README.md](src/MnM.Common.Data.Dapper.DependencyInjection/README.md)

---

## 2. **DynamoDB Integration**

Includes:
- `IDynamoDBClient<T>`
- `DynamoDBClient<T>`
- Async CRUD + Query
- Batch operations
- DI configuration

📘 Documentation:
- [src/MnM.Common.Data.DynamoDB/README.md](src/MnM.Common.Data.DynamoDB/README.md)
- [src/MnM.Common.Data.DynamoDB.DependencyInjection/README.md](src/MnM.Common.Data.DynamoDB.DependencyInjection/README.md)

---

## 3. **Elasticsearch Integration**

Includes:
- `ICommonElasticsearchClient`
- `CommonElasticsearchClient`
- Search + Get operations
- DI configuration

📘 Documentation:
- [src/MnM.Common.Data.Elasticsearch/README.md](src/MnM.Common.Data.Elasticsearch/README.md)
- [src/MnM.Common.Data.Elasticsearch.DependencyInjection/README.md](src/MnM.Common.Data.Elasticsearch.DependencyInjection/README.md)

---

## 4. **Retry Strategy Framework**

Shared across all technologies:

- `IRetryStrategy`
- `RetryStrategyBase<TException>`
- `RetryStrategyByCount`
- `SqlServerRetryStrategy`

---

## 5. **Repository & Specification Pattern**

Provides:
- Generic `IRepository<TClient, TReturn>`
- Core `Repository<TClient, TReturn>`
- Query / List / NonQuery specifications (sync + async)
- Clean separation between business logic & data layer

---

## 6. **Architecture Diagrams**

Visual documentation of the entire data architecture including:
- Full-system overview
- SQL Server subsystem
- DynamoDB subsystem
- Elasticsearch subsystem

📘 Documentation:
- [architecture.md](architecture.md)

---

# 🚀 Getting Started

### 1. Choose a backend
Examples:
- SQL Server → Dapper client + `AddDapperRepository`
- DynamoDB → `DynamoDBClient<T>` + `AddDynamoDBRepository`
- Elasticsearch → `CommonElasticsearchClient` + `AddElasticsearchRepository`

### 2. Register repository in DI
```csharp
services.AddDapperRepository<AppDbClient, UserDto>(factory);
services.AddDynamoDBRepository<User, string>(options);
services.AddElasticsearchRepository<SearchDocument>(options);
```

### 3. Inject repository
```csharp
public class UserService
{
    private readonly IRepository<IDbClient, UserDto> _repo;

    public UserService(IRepository<IDbClient, UserDto> repo)
    {
        _repo = repo;
    }
}
```

### 4. Execute with specifications or delegates
```csharp
var result = await _repo.ExecuteDbActionAsync(client =>
    client.SetCommandText("SELECT * FROM Users")
          .ExecuteQueryAsync<UserDto>());
```

---

# 🧪 Testing Support

The entire library is built on interfaces, making it easy to mock:
- `IDbClient`
- `IDynamoDBClient<T>`
- `ICommonElasticsearchClient`
- `IRepository<TClient, TReturn>`

---

# 📄 Additional Documentation

| Component | Documentation |
|----------|---------------|
| SQL Server / Dapper | [src/MnM.Common.Data.Dapper/README.md](src/MnM.Common.Data.Dapper/README.md), [src/MnM.Common.Data.Dapper.SqlServer/README.md](src/MnM.Common.Data.Dapper.SqlServer/README.md), [src/MnM.Common.Data.Dapper.DependencyInjection/README.md](src/MnM.Common.Data.Dapper.DependencyInjection/README.md) |
| DynamoDB | [src/MnM.Common.Data.DynamoDB/README.md](src/MnM.Common.Data.DynamoDB/README.md), [src/MnM.Common.Data.DynamoDB.DependencyInjection/README.md](src/MnM.Common.Data.DynamoDB.DependencyInjection/README.md) |
| Elasticsearch | [src/MnM.Common.Data.Elasticsearch/README.md](src/MnM.Common.Data.Elasticsearch/README.md), [src/MnM.Common.Data.Elasticsearch.DependencyInjection/README.md](src/MnM.Common.Data.Elasticsearch.DependencyInjection/README.md) |
| Architecture | [architecture.md](architecture.md) |

---

# 🎯 Summary

MnM.Common.Data is a **fully modular, extensible, and technology-agnostic data access framework** designed for clean architecture and enterprise-grade reliability.

Use this root README to navigate into each subsystem's documentation and explore detailed functionality.

