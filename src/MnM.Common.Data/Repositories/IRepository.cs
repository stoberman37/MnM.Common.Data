using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MnM.Common.Data.Specifications;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Repositories
{
    public interface IRepository<out TClient, TReturn>
        where TClient : class, IDisposable
        where TReturn : class
    {
        // Synchronous calls
        void ExecuteDbAction(Action<TClient> action);
        IEnumerable<TReturn> ExecuteDbAction(Func<TClient, IEnumerable<TReturn>> action);
        void ExecuteDbAction(INonQuerySpecification<TClient> specification);
	    void ExecuteDbAction(INonQuerySpecification<TClient> specification, CancellationToken cancellationToken);
        IEnumerable<TReturn> ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification);
        IEnumerable<TReturn> ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification, CancellationToken cancellationToken);


		// Async calls
		Task ExecuteDbActionAsync(Func<TClient, Task> action);
        Task<IEnumerable<TReturn>> ExecuteDbActionAsync(Func<TClient, Task<IEnumerable<TReturn>>> action);
        Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification);
        Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification, CancellationToken cancellationToken);
        Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification);
        Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification, CancellationToken cancellationToken);
    }
}