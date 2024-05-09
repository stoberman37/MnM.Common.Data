using System;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data
{
	public interface IRetryStrategy
	{
		int MaxRetryCount { get; }
		T Retry<T>(Func<T> func);
		T Retry<T>(Func<T> func, CancellationToken cancelToken);
		void Retry(Action func);
		void Retry(Action func, CancellationToken cancelToken);
		Task<T> RetryAsync<T>(Func<Task<T>> func);
		Task<T> RetryAsync<T>(Func<Task<T>> func, CancellationToken cancelToken);
		Task RetryAsync(Func<Task> func);
		Task RetryAsync(Func<Task> func, CancellationToken cancelToken);
		IRetryStrategy SetMaxRetryCount(int retryCount);
	}
}