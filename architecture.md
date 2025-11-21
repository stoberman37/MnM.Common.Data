# Architecture Overview

This document provides a full architectural overview of the MnM.Common.Data ecosystem, including dedicated diagrams for each technology stack:

- SQL Server / Dapper
- DynamoDB
- Elasticsearch

All diagrams use Mermaid syntax and are written to be compatible with GitHub's Mermaid renderer (only `-->` arrows, simple labels).

---

## 1. Full System Architecture

```mermaid
flowchart TB
  subgraph Application
    AppServices[Application services]
  end

  subgraph RepositoryLayer[Repository layer]
    RepoIface[IRepository&lt;TClient, TReturn&gt;]
    RepoImpl[Repository&lt;TClient, TReturn&gt;]
    Specs[Specifications]
    RepoIface --> RepoImpl
    RepoImpl --> Specs
  end

  AppServices --> RepoIface

  subgraph Clients[Client abstractions]
    subgraph Sql[SQL / Dapper]
      IDb[IDbClient]
      DbClient[DbClient&lt;TException&gt;]
      SqlDb[SqlServerDbClient]
      IDb --> DbClient
      DbClient --> SqlDb
    end

    subgraph Dynamo[DynamoDB]
      IDyn[IDynamoDBClient&lt;T&gt;]
      DynClient[DynamoDBClient&lt;T&gt;]
      IDyn --> DynClient
    end

    subgraph Elastic[Elasticsearch]
      IEs[ICommonElasticsearchClient]
      EsClient[CommonElasticsearchClient]
      IEs --> EsClient
    end
  end

  RepoImpl --> IDb
  RepoImpl --> IDyn
  RepoImpl --> IEs

  subgraph Providers[Underlying providers]
    DbConn[DbConnection]
    Dapper[Dapper + type mapping]
    DynCtx[IDynamoDBContext]
    EsNet[ElasticsearchClient]
  end

  DbClient --> DbConn
  DbClient --> Dapper
  DynClient --> DynCtx
  EsClient --> EsNet

  subgraph Retry[Retry strategies]
    IRetry[IRetryStrategy]
    RetryBase[RetryStrategyBase&lt;TException&gt;]
    RetryCount[RetryStrategyByCount]
    SqlRetry[SqlServerRetryStrategy]
  end

  DbClient --> IRetry
  DynClient --> IRetry
  EsClient --> IRetry

  IRetry --> RetryBase
  RetryBase --> RetryCount
  RetryBase --> SqlRetry

  subgraph DI[Dependency injection]
    DapperDI[AddDapperRepository]
    DynamoDI[AddDynamoDBRepository]
    EsDI[AddElasticsearchRepository]
  end

  DapperDI --> RepoIface
  DynamoDI --> RepoIface
  EsDI --> RepoIface
```

---

## 2. SQL Server / Dapper Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository&lt;IDbClient, TReturn&gt;]
  Repo --> DbClient[DbClient&lt;TException&gt;]
  DbClient --> SqlDb[SqlServerDbClient]
  SqlDb --> Conn[DbConnection (SqlConnection)]

  DbClient --> Dapper[Dapper + mapping]
  Dapper --> ColumnMap[ColumnAttributeTypeMapper]
  Dapper --> FallbackMap[FallbackTypeMapper]
  Dapper --> Params[ParameterManager]
  Dapper --> Crud[CrudMethod + attributes]

  DbClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase&lt;TException&gt;]
  RetryBase --> SqlRetry[SqlServerRetryStrategy]
```

---

## 3. DynamoDB Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository&lt;IDynamoDBClient&lt;T&gt;, TReturn&gt;]
  Repo --> DynClient[DynamoDBClient&lt;T&gt;]

  DynClient --> DynInterface[IDynamoDBClient&lt;T&gt;]
  DynClient --> Ctx[IDynamoDBContext]

  DynClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase&lt;TException&gt;]
  RetryBase --> RetryCount[RetryStrategyByCount]

  Ctx --> DynamoService[AWS DynamoDB service]
```

---

## 4. Elasticsearch Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository&lt;ICommonElasticsearchClient, TReturn&gt;]
  Repo --> EsClient[CommonElasticsearchClient]

  EsClient --> EsInterface[ICommonElasticsearchClient]
  EsClient --> EsNet[ElasticsearchClient]

  EsClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase&lt;TException&gt;]
  RetryBase --> RetryCount[RetryStrategyByCount]

  EsNet --> Cluster[Elasticsearch cluster]
```

---

## Summary

This architecture demonstrates a modular, consistent, and testable data access system:

- All technologies share common retry logic, a common repository pattern, and DI conventions.
- Each backend (SQL Server, DynamoDB, Elasticsearch) has its own client abstraction.
- Low-level providers (Dapper, DynamoDBContext, ElasticsearchClient) are encapsulated behind these abstractions.
