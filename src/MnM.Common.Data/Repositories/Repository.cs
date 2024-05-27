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

		public TReturn ExecuteDbAction(Func<T, TReturn> func)
		{
			if (func == null) throw new ArgumentNullException(nameof(func));
			using var client = Factory();
			return func(client);
		}

		public IEnumerable<TReturn> ExecuteDbAction(Func<T, IEnumerable<TReturn>> func)
		{
			if (func == null) throw new ArgumentNullException(nameof(func));
			using var client = Factory();
			return func(client);
		}

		public void ExecuteDbAction(INonQuerySpecification<T> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			ExecuteDbAction(specification.Execute());
		}

		public void ExecuteDbAction(INonQuerySpecification<T> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			ExecuteDbAction(specification.Execute(cancellationToken));
		}

		public TReturn ExecuteDbAction(IQuerySpecification<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return (TReturn)ExecuteDbAction(specification.Execute());
		}

		public TReturn ExecuteDbAction(IQuerySpecification<T, TReturn> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return (TReturn)ExecuteDbAction(specification.Execute(cancellationToken));
		}

		public IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbAction(specification.Execute());
		}

		public IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<T, TReturn> specification, CancellationToken cancellationToken)
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

		public Task<TReturn> ExecuteDbActionAsync(Func<T, Task<TReturn>> action)
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
			return ExecuteDbActionAsync(specification.ExecuteAsync());
		}

		public Task ExecuteDbActionAsync(INonQuerySpecificationAsync<T> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync(cancellationToken));
		}

		public Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync());
		}

		public Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<T, TReturn> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync(cancellationToken));
		}

		public Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<T, TReturn> specification)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync());
		}

		public Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<T, TReturn> specification, CancellationToken cancellationToken)
		{
			if (specification == null) throw new ArgumentNullException(nameof(specification));
			return ExecuteDbActionAsync(specification.ExecuteAsync(cancellationToken));
		}
		#endregion
	}
}