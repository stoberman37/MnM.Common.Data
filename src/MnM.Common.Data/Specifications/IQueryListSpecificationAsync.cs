using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data.Specifications
{
	/// <summary>
	/// Asynchronous query specification interface returning a collection of T
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IQueryListSpecificationAsync<in TClient, T>
	{
		Func<TClient, Task<IEnumerable<T>>> ExecuteAsync();
		Func<TClient, Task<IEnumerable<T>>> ExecuteAsync(CancellationToken cancellationToken);
	}
}