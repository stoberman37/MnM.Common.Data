using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Specifications
{
	public interface INonQuerySpecification<in TClient>
	{
		Action<TClient> Execute();
		Action<TClient> Execute(CancellationToken cancellationToken);
	}

	public interface INonQuerySpecificationAsync<in TClient>
	{
		Func<TClient, Task> ExecuteAsync();
		Func<TClient, Task> ExecuteAsync(CancellationToken cancellationToken);
	}
}
