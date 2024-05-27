using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using MnM.Common.Data.Attributes;
using MnM.Common.Data.Dapper;
using Common.Data.Example;
using MnM.Common.Data.Repositories;
using MnM.Common.Data.Specifications;
using System.Threading.Tasks;
using MnM.Common.Data.Dapper.SqlServer;
using MnM.Common.Data.Elasticsearch;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;


Console.WriteLine("Hello World!");

var connectionString = @"Server=(localdb)\mydb;Database=master;Trusted_Connection=True;";
SqlConnection ConnectionFactory() => new(connectionString);
var clientRepo = new Repository<IDbClient, DbModel>(() => new SqlServerDbClient(ConnectionFactory));
Dapper.SqlMapper.SetTypeMap(typeof(DbModel),
	new ColumnAttributeTypeMapper<DbModel>(false));


var clientSpec = new GetDatabaseSpec("master");
var model = clientRepo.ExecuteDbAction(clientSpec).FirstOrDefault();
model = (await clientRepo.ExecuteDbActionAsync(clientSpec)).FirstOrDefault();

Console.WriteLine($"Id: {model.Id}, Name: {model.DbName}");

//var elasticRepo = new Repository<CommonElasticsearchClient, DbModel>(() => new CommonElasticsearchClient(() => new ElasticsearchClient()));
//var search = new SearchContractSpec();
//var result = await elasticRepo.ExecuteDbActionAsync(search);

namespace Common.Data.Example
{
	public class DbModel
	{
		[Column("name")]
		public string DbName { get; set; }
		[Column("database_id")]
		public int Id { get; set; }
	}

	public class SearchContractSpec : IQueryListSpecificationAsync<CommonElasticsearchClient, DbModel>
	{
		private SearchRequest<DbModel> _search;
		public SearchContractSpec(string index, int from, int size, string user, string value)
		{
			_search = new SearchRequest<DbModel>(index)
			{
				From = from,
				Size = size,
				Query = new TermQuery(user) { Value = value }
			};
		}

		public Func<CommonElasticsearchClient, Task<IEnumerable<DbModel>>> ExecuteAsync() =>
			ExecuteAsync(new CancellationToken());

		public Func<CommonElasticsearchClient, Task<IEnumerable<DbModel>>> ExecuteAsync(CancellationToken cancellationToken)
		{
			return wrapper => wrapper.SearchAsync(_search, cancellationToken);
		}
	}

	public class GetDatabaseSpec(string dbName) : IQueryListSpecification<IDbClient, DbModel>, IQueryListSpecificationAsync<IDbClient, DbModel>
	{
		private readonly string _dbName = dbName ?? throw new ArgumentNullException(nameof(dbName));

		public Func<IDbClient, IEnumerable<DbModel>> Execute() => Execute(new CancellationToken());

		public Func<IDbClient, IEnumerable<DbModel>> Execute(CancellationToken cancellationToken)
		{
			return db =>
				db.SetCommandText("SELECT database_id, Name from sys.databases WHERE name = @name")
					.SetCommandType(CommandType.Text)
					.AddNamedParameters(new { name = _dbName })
					.SetCommandTimeout(30)
					.ExecuteQuery<DbModel>(cancellationToken: cancellationToken);
		}

		public Func<IDbClient, Task<IEnumerable<DbModel>>> ExecuteAsync() => ExecuteAsync(new CancellationToken());

		public Func<IDbClient, Task<IEnumerable<DbModel>>> ExecuteAsync(CancellationToken cancellationToken)
		{
			return db =>
				db.SetCommandText("SELECT database_id, Name from sys.databases WHERE name = @name")
					.SetCommandType(CommandType.Text)
					.AddNamedParameters(new { name = _dbName })
					.SetCommandTimeout(30)
					.ExecuteQueryAsync<DbModel>(cancellationToken: cancellationToken);

		}
	}

	public class MySpec : INonQuerySpecification<IDbClient>
	{
		public Action<IDbClient> Execute()
		{
			return _ => { };
		}

		public Action<IDbClient> Execute(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}

	/*
	attribute-values "{\":uuid\":{\"S\":\"{ \\\"vin\\\": \\\"ABCDEF0123456GHIJK\\\" }\"},
	\":index\":{\"S\":\"{ \\\"ipaddress\\\": \\\"1.1.1.2\\\", \\\"created_date\\\": \\\"1639000000\\\" }\"}}"
	--expression-attribute-names "{\"#uuid\": \"uuid\", \"#index\": \"index\"}" --limit 1
	*/
	//	public class JerrysQuerySpec : IQuerySpecification<IDynamoDBClient, JerryModel>
	//	{
	//		private readonly string _uuid;
	//		private readonly string _index;
	//		public JerrysQuerySpec(string uuid, string index)
	//		{
	//			_uuid = uuid;
	//			_index = index;
	//		}

	//		public Func<IDynamoDBClient, Task<JerryModel>> ExecuteFunc()
	//		{
	//			var command = $"attribute-values "{\":uuid\":{\"S\":\"{ \\\"vin\\\": \\\"ABCDEF0123456GHIJK\\\" }\"},
	//\":index\":{\"S\":\"{ \\\"ipaddress\\\": \\\"1.1.1.2\\\", \\\"created_date\\\": \\\"1639000000\\\" }\"}}" 
	//--expression-attribute-names "{\"#uuid\": \"{_uuid}\", \"#index\": \"index\"}" --limit 1"
	//"
	//			return db => db.ExecuteLoadAsync(command);
	//		}
	//	}
}