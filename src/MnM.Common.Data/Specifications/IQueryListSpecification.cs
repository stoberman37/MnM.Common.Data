using System;
using System.Collections.Generic;
using System.Threading;

namespace MnM.Common.Data.Specifications
{
	/// <summary>
	/// Synchronous query specification interface returning a collection of T
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IQueryListSpecification<in TClient, T>
	{
		Func<TClient, IEnumerable<T>> Execute();
		Func<TClient, IEnumerable<T>> Execute(CancellationToken cancellationToken);
	}
}