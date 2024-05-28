using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data.DynamoDB
{
	// ReSharper disable once InconsistentNaming
	public class DynamoDBClient<T> : IDynamoDBClient<T>
		where T : class
	{
		private readonly IDynamoDBContext _dynamoDbContext;
		private readonly IRetryStrategy _retryStrategy;
		private bool _disposedValue;

		public DynamoDBClient(Func<IDynamoDBContext> connectionFactory) : this(connectionFactory, new RetryStrategyByCount())
		{
		}

		public DynamoDBClient(Func<IDynamoDBContext> connectionFactory, IRetryStrategy retryStrategy)
		{
			if (connectionFactory == null) throw new ArgumentNullException(nameof(connectionFactory));
			_retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
			_dynamoDbContext = connectionFactory();
		}

		public Task CreateAsync(T toSave)
		{
			if (toSave == default(T)) throw new ArgumentNullException(nameof(toSave));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.SaveAsync(toSave));
		}

		public Task CreateAsync(T toSave, CancellationToken cancellationToken)
		{
			if (toSave == default(T)) throw new ArgumentNullException(nameof(toSave));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.SaveAsync(toSave, cancellationToken), cancellationToken);
		}

		public Task UpdateAsync(T toSave)
		{
			if (toSave == default(T)) throw new ArgumentNullException(nameof(toSave));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.SaveAsync(toSave));
		}

		public Task UpdateAsync(T toSave, CancellationToken cancellationToken)
		{
			if (toSave == default(T)) throw new ArgumentNullException(nameof(toSave));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.SaveAsync(toSave, cancellationToken), cancellationToken);
		}

		public Task DeleteAsync(T toDelete)
		{
			if (toDelete == default(T)) throw new ArgumentNullException(nameof(toDelete));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.DeleteAsync(toDelete));
		}

		public Task DeleteAsync(T toDelete, CancellationToken cancellationToken)
		{
			if (toDelete == default(T)) throw new ArgumentNullException(nameof(toDelete));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.DeleteAsync(toDelete, cancellationToken), cancellationToken);
		}

		public Task<T> ReadAsync(object hashKey)
		{
			if (hashKey == null) throw new ArgumentNullException(nameof(hashKey));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.LoadAsync<T>(hashKey));
		}

		public Task<T> ReadAsync(object hashKey, CancellationToken cancellationToken)
		{
			if (hashKey == null) throw new ArgumentNullException(nameof(hashKey));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.LoadAsync<T>(hashKey, cancellationToken), cancellationToken);
		}

		public Task<T> ReadAsync(object hashKey, object rangeKey)
		{
			if (hashKey == null) throw new ArgumentNullException(nameof(hashKey));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.LoadAsync<T>(hashKey, rangeKey));
		}

		public Task<T> ReadAsync(object hashKey, object rangeKey, CancellationToken cancellationToken)
		{
			if (hashKey == null) throw new ArgumentNullException(nameof(hashKey));
			return _retryStrategy.RetryAsync(() => _dynamoDbContext.LoadAsync<T>(hashKey, rangeKey, cancellationToken),
				cancellationToken);
		}

		public Task<T> ReadAsync(QueryOperationConfig toRead)
		{
			if (toRead == null || toRead.Equals(default(QueryOperationConfig))) throw new ArgumentNullException(nameof(toRead));
			return _retryStrategy.RetryAsync(async () =>
				(await _dynamoDbContext.FromQueryAsync<T>(toRead).GetRemainingAsync()).FirstOrDefault());
		}

		public Task<T> ReadAsync(QueryOperationConfig toRead, CancellationToken cancellationToken)
		{
			if (toRead == null || toRead.Equals(default(QueryOperationConfig))) throw new ArgumentNullException(nameof(toRead));
			return _retryStrategy.RetryAsync(
				async () => (await _dynamoDbContext.FromQueryAsync<T>(toRead).GetRemainingAsync(cancellationToken)).FirstOrDefault(),
					 cancellationToken);
		}

		public Task UpdateAsync(IEnumerable<T> toSave)
		{
			if (toSave == null) throw new ArgumentNullException(nameof(toSave));
			var batch = _dynamoDbContext.CreateBatchWrite<T>();
			batch.AddPutItems(toSave);
			return _retryStrategy.RetryAsync(() => batch.ExecuteAsync());
		}

		public Task<IEnumerable<T>> ListAsync(QueryOperationConfig toRead)
		{
			if (toRead == null || toRead.Equals(default(QueryOperationConfig))) throw new ArgumentNullException(nameof(toRead));
			return _retryStrategy.RetryAsync(async () =>
				(await _dynamoDbContext.FromQueryAsync<T>(toRead).GetRemainingAsync()).AsEnumerable());
		}

		public Task<IEnumerable<T>> ListAsync(QueryOperationConfig toRead, CancellationToken cancellationToken)
		{
			if (toRead == null || toRead.Equals(default(QueryOperationConfig))) throw new ArgumentNullException(nameof(toRead));
			return _retryStrategy.RetryAsync(
				async () => (await _dynamoDbContext.FromQueryAsync<T>(toRead).GetRemainingAsync(cancellationToken)).AsEnumerable(),
				cancellationToken);
		}

		public Task UpdateAsync(IEnumerable<T> toSave, CancellationToken cancellationToken)
		{
			if (toSave == null) throw new ArgumentNullException(nameof(toSave));
			var batch = _dynamoDbContext.CreateBatchWrite<T>();
			batch.AddPutItems(toSave);
			return _retryStrategy.RetryAsync(() => batch.ExecuteAsync(cancellationToken), cancellationToken);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}
				_disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}