using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Specifications
{
	public interface IQuerySpecification<in TClient, T>
	{
		Func<TClient, IEnumerable<T>> Execute();
		Func<TClient, IEnumerable<T>> Execute(CancellationToken cancellationToken);
	}

	public interface IQuerySpecificationAsync<in TClient, T>
	{
		Func<TClient, Task<IEnumerable<T>>> ExecuteAsync();
		Func<TClient, Task<IEnumerable<T>>> ExecuteAsync(CancellationToken cancellationToken);
	}
}