using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Specifications
{
	public interface INonQuerySpecification<in TClient>
	{
		Action<TClient> Execute();
		Action<TClient> Execute(CancellationToken cancellationToken);
	}
}
