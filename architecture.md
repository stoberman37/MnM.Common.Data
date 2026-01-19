# Architecture Overview (GitHub‑Safe Mermaid)

This file contains **GitHub‑compatible Mermaid diagrams**.  
All labels avoid parentheses, angle brackets, HTML entities, or multiline text.

---

## 1. Full System Architecture

```mermaid
flowchart TB
  subgraph Application
    AppServices[Application Services]
  end

  subgraph RepositoryLayer
    RepoIface[IRepository]
    RepoImpl[Repository]
    Specs[Specifications]
    RepoIface --> RepoImpl
    RepoImpl --> Specs
  end

  AppServices --> RepoIface

  subgraph Clients
    subgraph Sql
      IDb[IDbClient]
      DbClient[DbClient]
      SqlDb[SqlServerDbClient]
      IDb --> DbClient
      DbClient --> SqlDb
    end

    subgraph Dynamo
      IDyn[IDynamoDBClient]
      DynClient[DynamoDBClient]
      IDyn --> DynClient
    end

    subgraph Elastic
      IEs[ICommonElasticsearchClient]
      EsClient[CommonElasticsearchClient]
      IEs --> EsClient
    end
  end

  RepoImpl --> IDb
  RepoImpl --> IDyn
  RepoImpl --> IEs

  subgraph Providers
    DbConn[DbConnection SqlConnection]
    Dapper[Dapper Mapping]
    DynCtx[IDynamoDBContext]
    EsNet[ElasticsearchClient]
  end

  DbClient --> DbConn
  DbClient --> Dapper
  DynClient --> DynCtx
  EsClient --> EsNet

  subgraph Retry
    IRetry[IRetryStrategy]
    RetryBase[RetryStrategyBase]
    RetryCount[RetryStrategyByCount]
    SqlRetry[SqlServerRetryStrategy]
  end

  DbClient --> IRetry
  DynClient --> IRetry
  EsClient --> IRetry

  IRetry --> RetryBase
  RetryBase --> RetryCount
  RetryBase --> SqlRetry
```

---

## 2. SQL Server and Dapper Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository]
  Repo --> DbClient[DbClient]
  DbClient --> SqlDb[SqlServerDbClient]
  SqlDb --> Conn[DbConnection SqlConnection]

  DbClient --> Dapper[Dapper Mapping]
  Dapper --> ColumnMap[ColumnAttributeTypeMapper]
  Dapper --> FallbackMap[FallbackTypeMapper]
  Dapper --> Params[ParameterManager]
  Dapper --> Crud[Crud Attributes]

  DbClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase]
  RetryBase --> SqlRetry[SqlServerRetryStrategy]
```

---

## 3. DynamoDB Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository]
  Repo --> DynClient[DynamoDBClient]

  DynClient --> DynInterface[IDynamoDBClient]
  DynClient --> Ctx[IDynamoDBContext]

  DynClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase]
  RetryBase --> RetryCount[RetryStrategyByCount]

  Ctx --> DynamoService[DynamoDB Service]
```

---

## 4. Elasticsearch Architecture

```mermaid
flowchart LR
  App[Application] --> Repo[IRepository]
  Repo --> EsClient[CommonElasticsearchClient]

  EsClient --> EsInterface[ICommonElasticsearchClient]
  EsClient --> EsNet[ElasticsearchClient]

  EsClient --> Retry[IRetryStrategy]
  Retry --> RetryBase[RetryStrategyBase]
  RetryBase --> RetryCount[RetryStrategyByCount]

  EsNet --> Cluster[Elasticsearch Cluster]
```

---

## Summary

These diagrams are simplified to comply with GitHub's Mermaid renderer:

- No parentheses in node labels  
- No special characters  
- Only `-->` arrows  
- Single‑line labels  
