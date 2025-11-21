# DynamoDB Client Abstraction for MnM.Common.Data

This project provides a lightweight, retry-aware wrapper around the AWS DynamoDB .NET Object Persistence Model (`IDynamoDBContext`).  
It delivers a consistent interface for CRUD and query operations while integrating seamlessly with your existing retry strategies and infrastructure.

---

## Contents

- [Overview](#overview)
- [IDynamoDBClient<T> Interface](#idynamodbclientt-interface)
- [DynamoDBClient<T> Implementation](#dynamodbclientt-implementation)
- [Supported Operations](#supported-operations)
- [Usage Examples](#usage-examples)
  - Basic registration
  - Creating and updating items
  - Batch updates
  - Querying with `QueryOperationConfig`
- [Disposal Behavior](#disposal-behavior)

---

## Overview

This library includes:

### ✔ `IDynamoDBClient<T>` — a high-level async CRUD/query interface  
### ✔ `DynamoDBClient<T>` — an implementation using `IDynamoDBContext` with retry handling  
### ✔ Full integration with your retry strategy infrastructure (`IRetryStrategy`)  

The client simplifies DynamoDB access and provides a consistent, easy-to-mock abstraction.

---

## IDynamoDBClient<T> Interface

`IDynamoDBClient<T>` defines all supported operations against DynamoDB for a given model `T`.

```csharp
public interface IDynamoDBClient<T> : IDisposable where T : class
```

Supported operations include:

- `CreateAsync`
- `UpdateAsync` (single or batch)
- `DeleteAsync`
- `ReadAsync` (by hash, hash+range, or `QueryOperationConfig`)
- `ListAsync` (query returning multiple items)

Source:  
fileciteturn4file0

---

## DynamoDBClient<T> Implementation

`DynamoDBClient<T>` is the concrete implementation backed by AWS `IDynamoDBContext`.

Key behaviors:

- Accepts a **connection factory** (`Func<IDynamoDBContext>`)
- Accepts an **IRetryStrategy** (default `RetryStrategyByCount`)
- Wraps **all** DynamoDB operations in retry logic
- Provides async-only operations (DynamoDB best practice)
- Supports batch writes

Excerpt:

```csharp
public class DynamoDBClient<T> : IDynamoDBClient<T> where T : class
{
    private readonly IDynamoDBContext _dynamoDbContext;
    private readonly IRetryStrategy _retryStrategy;

    public DynamoDBClient(Func<IDynamoDBContext> connectionFactory)
        : this(connectionFactory, new RetryStrategyByCount()) { }
}
```

Source:  
fileciteturn4file1

---

## Supported Operations

### Create  
- `CreateAsync(T toSave)`
- `CreateAsync(T toSave, CancellationToken)`

### Update  
- `UpdateAsync(T toSave)`
- `UpdateAsync(IEnumerable<T> toSave)` batch
- All with cancellation overloads

### Delete  
- `DeleteAsync(T toDelete)`

### Read  
- By hash key  
- By hash + range key  
- Using `QueryOperationConfig`  

### List  
- Similar to query but returns multiple items

Batch update uses:

```csharp
var batch = _dynamoDbContext.CreateBatchWrite<T>();
batch.AddPutItems(toSave);
```

---

## Usage Examples

### 1. Registering the Client

```csharp
services.AddSingleton<IDynamoDBClient<MyModel>>(sp =>
    new DynamoDBClient<MyModel>(() =>
        sp.GetRequiredService<IDynamoDBContext>()));
```

Or with a custom retry strategy:

```csharp
services.AddSingleton<IDynamoDBClient<MyModel>>(sp =>
    new DynamoDBClient<MyModel>(
        () => sp.GetRequiredService<IDynamoDBContext>(),
        new RetryStrategyByCount(maxRetryCount: 3)));
```

---

### 2. Creating an Item

```csharp
await dynamo.CreateAsync(new MyModel
{
    Id = "123",
    Name = "Example"
});
```

---

### 3. Updating an Item

```csharp
await dynamo.UpdateAsync(item);
```

---

### 4. Batch Updating

```csharp
var items = new List<MyModel> { a, b, c };
await dynamo.UpdateAsync(items);
```

---

### 5. Reading by Hash Key

```csharp
var item = await dynamo.ReadAsync("123");
```

---

### 6. Reading by Hash + Range Key

```csharp
var item = await dynamo.ReadAsync("User#123", "Order#456");
```

---

### 7. Querying with QueryOperationConfig

```csharp
var config = new QueryOperationConfig
{
    KeyExpression = new Expression
    {
        ExpressionStatement = "Id = :id",
        ExpressionAttributeValues = { [":id"] = "123" }
    }
};

var result = await dynamo.ReadAsync(config);
```

List version:

```csharp
var results = await dynamo.ListAsync(config);
```

---

## Disposal Behavior

`DynamoDBClient<T>` implements `IDisposable`.  
The current implementation suppresses finalization but does **not** dispose the underlying context (consistent with AWS guidance unless the caller owns the context).

---

## Summary

This project provides:

- A unified, retry-enabled DynamoDB client abstraction  
- Clean async API for CRUD + query operations  
- Batch support  
- Plug-and-play compatibility with your retry strategies  
- A simple interface ideal for mocking and unit testing  

The result is a robust, consistent, and testable DynamoDB data access layer.

