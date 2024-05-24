using System;

namespace MnM.Common.Data.Dapper.DependencyInjection
{
	public class DapperRepositoryConfigurationOptions<TClient>
		where TClient : class, IDbClient, IDisposable
	{
		public bool CaseSensitiveColumnMapping { get; set; }
		public Func<TClient> ClientFactory { get; set; } = () => throw new NotImplementedException();
	}
}