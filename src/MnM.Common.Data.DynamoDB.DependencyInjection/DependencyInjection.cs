using System;
using Amazon.DynamoDBv2.DataModel;
using MnM.Common.Data.DynamoDB;
using Microsoft.Extensions.DependencyInjection;
using MnM.Common.Data.DynamoDB.DependencyInjection;
using MnM.Common.Data.Repositories;

namespace MnM.Common.Data.DynamoDB.DependencyInjection
{
	public static class DependencyInjection
	{
		// ReSharper disable once InconsistentNaming
		public static IServiceCollection AddDynamoDBRepository<TReturn, TKey>(this IServiceCollection @this, RepositoryConfigurationOptions options)
			where TReturn : class
			where TKey: notnull
		{
			if (@this == null) throw new ArgumentNullException(nameof(@this));
			if (options == null) throw new ArgumentNullException(nameof(options));
			if (options.DynamoDBContext == null) throw new ArgumentException("DynamoDBContext cannot be null", nameof(options));
			if (options.RetryStrategy == null) throw new ArgumentException("RetryStrategy cannot be null", nameof(options));

			@this.AddSingleton(p => new Func<IDynamoDBContext>(options.DynamoDBContext));
			@this.AddSingleton<Func<DynamoDBClient<TReturn, TKey>>>(p => 
				 () => new DynamoDBClient<TReturn, TKey>(p.GetService<Func<IDynamoDBContext>>(), options.RetryStrategy()));
			@this.AddScoped<IRepository<DynamoDBClient<TReturn, TKey>, TReturn>, Repository<DynamoDBClient<TReturn, TKey>, TReturn>>();
			return @this;
		}
	}
}
