using System;
using MnM.Common.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using D = Dapper;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Dapper.DependencyInjection
{
	public static class DependencyInjection
	{
		public static void AddColumnMapper<TReturn>(bool caseSensitive = false)
			where TReturn : class

		{
			D.SqlMapper.SetTypeMap(
				typeof(TReturn),
				new ColumnAttributeTypeMapper<TReturn>(caseSensitive));
		}

		public static IServiceCollection AddDapperRepository<TClient, TReturn>(this IServiceCollection @this, Func<TClient> clientFactory)
			where TClient : class, IDbClient, IDisposable
			where TReturn : class
		{
			return @this.AddDapperRepository<TClient, TReturn>(new DapperRepositoryConfigurationOptions<TClient>
			{
				ClientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory)),
				CaseSensitiveColumnMapping = false
			});
		}

		public static IServiceCollection AddDapperRepository<TClient, TReturn>(this IServiceCollection @this, DapperRepositoryConfigurationOptions<TClient> options)
			where TClient : class, IDbClient, IDisposable
			where TReturn : class
		{
			if (@this == null) throw new ArgumentNullException(nameof(@this));
			if (options == null) throw new ArgumentNullException(nameof(options));
			if (options.ClientFactory == null) throw new ArgumentException("ClientFactory cannot be null", nameof(options));

			@this.AddSingleton(options.ClientFactory ?? throw new ArgumentException("ClientFactory cannot be null", nameof(options)));
			@this.AddScoped<IRepository<TClient, TReturn>, Repository<TClient, TReturn>>();
			AddColumnMapper<TReturn>(options.CaseSensitiveColumnMapping);
			return @this;
		}
	}
}
