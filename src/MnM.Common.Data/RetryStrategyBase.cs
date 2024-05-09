using System;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data
{
	public abstract class RetryStrategyBase<TException> : IRetryStrategy where TException : Exception
	{
		private readonly Func<RetryStrategyBase<TException>, TException, bool> _continueFunc;

		protected RetryStrategyBase(Func<RetryStrategyBase<TException>, TException, bool> continueFunc) 
			: this(continueFunc, DefaultRetryCount)
		{
		}
		protected RetryStrategyBase(Func<RetryStrategyBase<TException>, TException, bool> continueFunc, int maxRetryCount)
		{
			_continueFunc = continueFunc ?? throw new ArgumentNullException(nameof(continueFunc));
			MaxRetryCount = maxRetryCount;
		}

		private const int DefaultRetryCount = 0;
		public int MaxRetryCount { get; private set; }
		public int RetryCount { get; private set; }

		public T Retry<T>(Func<T> func)
		{
			return Retry(func, default);
		}

		public T Retry<T>(Func<T> func, CancellationToken cancelToken)
		{
			while (true)
			{
				try
				{
					return func();
				}
				catch (TException ex)
				{
					RetryCount++;
					cancelToken.ThrowIfCancellationRequested();
					if (!_continueFunc(this, ex)) throw;
				}
			}
		}

		public void Retry(Action func)
		{
			Retry(func, default);
		}

		public void Retry(Action func, CancellationToken cancelToken)
		{
			while (true)
			{    try
				{
					func();
					return;
				}
				catch (TException ex)
				{
					RetryCount++;
					cancelToken.ThrowIfCancellationRequested();
					if (!_continueFunc(this, ex)) throw;
				}
			}
		}

		public Task<T> RetryAsync<T>(Func<Task<T>> func)
		{
			return RetryAsync(func, default);
		}

		public async Task<T> RetryAsync<T>(Func<Task<T>> func, CancellationToken cancelToken)
		{
			while (true)
			{    try
				{
					return await func().ConfigureAwait(false);
				}
				catch (TException ex)
				{
					RetryCount++;
					cancelToken.ThrowIfCancellationRequested();
					if (!_continueFunc(this, ex)) throw;
				}
			}
		}

		public Task RetryAsync(Func<Task> func)
		{
			return RetryAsync(func, default);
		}

		public async Task RetryAsync(Func<Task> func, CancellationToken cancelToken)
		{
			while (true)
			{    try
				{
					await func().ConfigureAwait(false);
					return;
				}
				catch (TException ex)
				{
					RetryCount++;
					cancelToken.ThrowIfCancellationRequested();
					if (!_continueFunc(this, ex)) throw;
				}
			}
		}

		public IRetryStrategy SetMaxRetryCount(int retryCount)
		{
			MaxRetryCount = retryCount;
			return this;
		}
	}
}