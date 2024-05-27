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
        TReturn ExecuteDbAction(Func<TClient, TReturn> action);
        IEnumerable<TReturn> ExecuteDbAction(Func<TClient, IEnumerable<TReturn>> action);
        void ExecuteDbAction(INonQuerySpecification<TClient> specification);
	    void ExecuteDbAction(INonQuerySpecification<TClient> specification, CancellationToken cancellationToken);
	    TReturn ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification);
	    TReturn ExecuteDbAction(IQuerySpecification<TClient, TReturn> specification, CancellationToken cancellationToken);
	    IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<TClient, TReturn> specification);
	    IEnumerable<TReturn> ExecuteDbAction(IQueryListSpecification<TClient, TReturn> specification, CancellationToken cancellationToken);


		// Async calls
		Task ExecuteDbActionAsync(Func<TClient, Task> action);
        Task<IEnumerable<TReturn>> ExecuteDbActionAsync(Func<TClient, Task<IEnumerable<TReturn>>> action);
        Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification);
        Task ExecuteDbActionAsync(INonQuerySpecificationAsync<TClient> specification, CancellationToken cancellationToken);
        Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification);
        Task<TReturn> ExecuteDbActionAsync(IQuerySpecificationAsync<TClient, TReturn> specification, CancellationToken cancellationToken);
		Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<TClient, TReturn> specification);
		Task<IEnumerable<TReturn>> ExecuteDbActionAsync(IQueryListSpecificationAsync<TClient, TReturn> specification, CancellationToken cancellationToken);
    }
}