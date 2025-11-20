# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\MnM.Common.Data\MnM.Common.Data.csproj
4. Upgrade src\MnM.Common.Data.Dapper\MnM.Common.Data.Dapper.csproj
5. Upgrade src\MnM.Common.Data.Dapper.SqlServer\MnM.Common.Data.Dapper.SqlServer.csproj
6. Upgrade src\MnM.Common.Data.Dapper.DependencyInjection\MnM.Common.Data.Dapper.DependencyInjection.csproj
7. Upgrade src\MnM.Common.Data.Elasticsearch\MnM.Common.Data.Elasticsearch.csproj
8. Upgrade src\MnM.Common.Data.Elasticsearch.DependencyInjection\MnM.Common.Data.Elasticsearch.DependencyInjection.csproj
9. Upgrade src\MnM.Common.Data.DynamoDB\MnM.Common.Data.DynamoDB.csproj
10. Upgrade src\MnM.Common.Data.DynamoDB.DependencyInjection\MnM.Common.Data.DynamoDB.DependencyInjection.csproj
11. Upgrade test\MnM.Common.Data.UnitTests\MnM.Common.Data.UnitTests.csproj
12. Upgrade test\MnM.Common.Data.Dapper.UnitTests\MnM.Common.Data.Dapper.UnitTests.csproj
13. Upgrade test\MnM.Common.Data.Dapper.SqlServer.UnitTests\MnM.Common.Data.Dapper.SqlServer.UnitTests.csproj
14. Upgrade test\MnM.Common.Data.Dapper.DependencyInjection.UnitTests\MnM.Common.Data.Dapper.DependencyInjection.UnitTests.csproj
15. Upgrade test\MnM.Common.Data.Elasticsearch.UnitTests\MnM.Common.Data.Elasticsearch.UnitTests.csproj
16. Upgrade test\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests.csproj
17. Upgrade test\MnM.Common.Data.DynamoDB.UnitTests\MnM.Common.Data.DynamoDB.UnitTests.csproj
18. Upgrade test\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests.csproj
19. Upgrade examples\MnM.Common.Data.Example\MnM.Common.Data.Example.csproj
20. Run unit tests to validate upgrade in the projects listed below:
   - test\MnM.Common.Data.UnitTests\MnM.Common.Data.UnitTests.csproj
   - test\MnM.Common.Data.Dapper.UnitTests\MnM.Common.Data.Dapper.UnitTests.csproj
   - test\MnM.Common.Data.Dapper.SqlServer.UnitTests\MnM.Common.Data.Dapper.SqlServer.UnitTests.csproj
   - test\MnM.Common.Data.Dapper.DependencyInjection.UnitTests\MnM.Common.Data.Dapper.DependencyInjection.UnitTests.csproj
   - test\MnM.Common.Data.Elasticsearch.UnitTests\MnM.Common.Data.Elasticsearch.UnitTests.csproj
   - test\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests.csproj
   - test\MnM.Common.Data.DynamoDB.UnitTests\MnM.Common.Data.DynamoDB.UnitTests.csproj
   - test\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

| Project name | Description |
|:-------------------------------|:---------------------------:|

### Aggregate NuGet packages modifications across all projects

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.Extensions.DependencyInjection | 8.0.0 | 10.0.0 | Recommended for .NET 10.0 |
| System.Data.Odbc | 8.0.0 | 10.0.0 | Recommended for .NET 10.0 |
| System.Data.OleDb | 8.0.0 | 10.0.0 | Recommended for .NET 10.0 |

### Project upgrade details

#### src\MnM.Common.Data\MnM.Common.Data.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`

#### src\MnM.Common.Data.Dapper\MnM.Common.Data.Dapper.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`

#### src\MnM.Common.Data.Dapper.SqlServer\MnM.Common.Data.Dapper.SqlServer.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`

#### src\MnM.Common.Data.Dapper.DependencyInjection\MnM.Common.Data.Dapper.DependencyInjection.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### src\MnM.Common.Data.Elasticsearch\MnM.Common.Data.Elasticsearch.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`

#### src\MnM.Common.Data.Elasticsearch.DependencyInjection\MnM.Common.Data.Elasticsearch.DependencyInjection.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### src\MnM.Common.Data.DynamoDB\MnM.Common.Data.DynamoDB.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`

#### src\MnM.Common.Data.DynamoDB.DependencyInjection\MnM.Common.Data.DynamoDB.DependencyInjection.csproj modifications
Project properties changes:
  - Target frameworks should be changed from `netstandard2.1;net8.0` to `netstandard2.1;net8.0;net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### test\MnM.Common.Data.UnitTests\MnM.Common.Data.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### test\MnM.Common.Data.Dapper.UnitTests\MnM.Common.Data.Dapper.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - System.Data.Odbc should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)
  - System.Data.OleDb should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### test\MnM.Common.Data.Dapper.SqlServer.UnitTests\MnM.Common.Data.Dapper.SqlServer.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### test\MnM.Common.Data.Dapper.DependencyInjection.UnitTests\MnM.Common.Data.Dapper.DependencyInjection.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### test\MnM.Common.Data.Elasticsearch.UnitTests\MnM.Common.Data.Elasticsearch.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### test\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests\MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### test\MnM.Common.Data.DynamoDB.UnitTests\MnM.Common.Data.DynamoDB.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### test\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests\MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)

#### examples\MnM.Common.Data.Example\MnM.Common.Data.Example.csproj modifications
Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`
NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection should be updated from `8.0.0` to `10.0.0` (recommended for .NET 10.0)
