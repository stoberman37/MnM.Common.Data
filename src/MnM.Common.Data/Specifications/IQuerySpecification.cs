using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Specifications
{
	/// <summary>
	///  Synchronous query specification interface returning a single instance of T
	/// </summary>
	/// <typeparam name="TClient"></typeparam>
	/// <typeparam name="T"></typeparam>
	public interface IQuerySpecification<in TClient, T>
	{
		Func<TClient, T> Execute();
		Func<TClient, T> Execute(CancellationToken cancellationToken);
	}
}