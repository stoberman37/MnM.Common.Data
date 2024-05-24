using System;
using Microsoft.Extensions.DependencyInjection;
using MnM.Common.Data.Repositories;
using Elastic.Clients.Elasticsearch;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Elasticsearch.DependencyInjection
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddElasticsearchRepository<TReturn>(this IServiceCollection @this, ElasticsearchRepositoryConfigurationOptions options)
			where TReturn : class
		{
			if (@this == null) throw new ArgumentNullException(nameof(@this));
			if (options == null) throw new ArgumentNullException(nameof(options));
			if (options.ElasticsearchClientSettings == null) throw new ArgumentException("ElasticsearchClientSettings cannot be null", nameof(options));
			if (options.RetryStrategy == null) throw new ArgumentException("RetryStrategy cannot be null", nameof(options));

			var c = new ElasticsearchClient(options.ElasticsearchClientSettings);
			@this.AddSingleton(c);
			@this.AddSingleton(p => new Func<ElasticsearchClient>(p.GetService<ElasticsearchClient>));
			@this.AddSingleton<Func<ICommonElasticsearchClient>>(p =>
				() => new CommonElasticsearchClient(p.GetService<Func<ElasticsearchClient>>(), options.RetryStrategy()));
			@this.AddScoped<IRepository<ICommonElasticsearchClient, TReturn>, Repository<ICommonElasticsearchClient, TReturn>>();
			return @this;
		}
	}
}
