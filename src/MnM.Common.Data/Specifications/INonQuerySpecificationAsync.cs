using System;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data.Specifications
{
	public interface INonQuerySpecificationAsync<in TClient>
	{
		Func<TClient, Task> ExecuteAsync();
		Func<TClient, Task> ExecuteAsync(CancellationToken cancellationToken);
	}
}