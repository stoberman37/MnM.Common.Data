using System;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data.Specifications
{
	/// <summary>
	/// Asynchronous query specification interface returning a single instance of T
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IQuerySpecificationAsync<in TClient, T>
	{
		Func<TClient, Task<T>> ExecuteAsync();
		Func<TClient, Task<T>> ExecuteAsync(CancellationToken cancellationToken);
	}
}