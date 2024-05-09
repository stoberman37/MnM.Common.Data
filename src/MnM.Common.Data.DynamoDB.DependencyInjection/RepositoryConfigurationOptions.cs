using Amazon.DynamoDBv2.DataModel;
using System;

namespace MnM.Common.Data.DynamoDB.DependencyInjection
{

	public class RepositoryConfigurationOptions
	{
		public Func<IDynamoDBContext> DynamoDBContext { get; set; } = () => throw new NotImplementedException();

		public Func<IRetryStrategy> RetryStrategy { get; set; } = () => new RetryStrategyByCount(0);
	}
}