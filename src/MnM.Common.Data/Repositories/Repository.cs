using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MnM.Common.Data.Specifications;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Repositories
{
	public sealed class Repository<T, TReturn> : IRepository<T, TReturn>
		where T : class, IDisposable
		where TReturn : class
	{
		internal readonly Func<T> Factory;

		public Repository(Func<T> factory)
		{
			Factory = factory ?? throw new ArgumentNullException(nameof(factory));
		}

	#region Sync
		public void ExecuteDbAction(Action<T> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			using var client = Factory();
			action(client);
		}

		public IEnumerable<TReturn> ExecuteDbAction(Func<T, IEnumerable<TReturn>> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			using var client = Factory();
			return action(client);
		}

		public void ExecuteDbAction(INonQuerySpecification<T> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			ExecuteDbAction(specification, default);
		}

		public void ExecuteDbAction(INonQuerySpecification<T> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			ExecuteDbAction(specification.Execute(cancellationToken));
		}

		public IEnumerable<TReturn> ExecuteDbAction(IQuerySpecification<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbAction(specification, default);
		}

		public IEnumerable<TReturn> ExecuteDbAction(IQuerySpecification<T, TReturn> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbAction(specification.Execute(cancellationToken));
		}

		#endregion

		#region Async
		public Task ExecuteDbActionAsync(Func<T, Task> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			using var client = Factory();
			return action(client);
		}

		public Task<IEnumerable<TReturn>> ExecuteDbActionAsync(Func<T, Task<IEnumerable<TReturn>>> action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));
			using var client = Factory();
			return action(client);
		}

		public Task ExecuteDbActionAsync(INonQuerySpecificationAsync<T> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification, default);
		}

		public Task ExecuteDbActionAsync(INonQuerySpecificationAsync<T> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync(cancellationToken));
		}

		public Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQuerySpecificationAsync<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification, default);
		}

		public Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQuerySpecificationAsync<T, TReturn> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync(cancellationToken));
		}
		#endregion
	}
}