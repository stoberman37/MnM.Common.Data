using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace MnM.Common.Data.Elasticsearch
{
	public class CommonElasticsearchClient : ICommonElasticsearchClient
	{
		private ElasticsearchClient _client;
		public IRetryStrategy _retryStrategy;
		private bool _disposedValue;

		public CommonElasticsearchClient(Func<ElasticsearchClient> factory) : this(factory, new RetryStrategyByCount(0))
		{
		}

		public CommonElasticsearchClient(Func<ElasticsearchClient> factory, IRetryStrategy retryStrategy) 
		{
			if (factory == null) { throw new ArgumentNullException(nameof(factory)); }
			_client = factory();
			_retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
		}

		public Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search) where T : class =>
			SearchAsync(search, new CancellationToken());

		public async Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search, CancellationToken cancellationToken) where T: class
		{
			if (search == null) throw new ArgumentNullException(nameof(search));
			var response = await _retryStrategy.RetryAsync(() => _client.SearchAsync(search, cancellationToken), cancellationToken);
			return response.IsValidResponse ? response.Documents : null;
		}

		public Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search) where T : class =>
			SearchAsync(search, new CancellationToken());

		public async Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search, CancellationToken cancellationToken) where T : class
		{
			if (search == null) throw new ArgumentNullException(nameof(search));
			var response = await _retryStrategy.RetryAsync(() => _client.SearchAsync<T>(search, cancellationToken), cancellationToken);
			return response.IsValidResponse ? response.Documents : null;
		}

		public Task<T> GetAsync<T>(GetRequestDescriptor<T> getRequest) where T : class =>
			GetAsync<T>(getRequest, new CancellationToken());

		public async Task<T> GetAsync<T>(GetRequestDescriptor<T> getRequest, CancellationToken cancellationToken) where T : class
		{
			if (getRequest == null) throw new ArgumentNullException(nameof(getRequest));
			var response = await _retryStrategy.RetryAsync(() => _client.GetAsync(getRequest, cancellationToken),
				cancellationToken);
			return response.IsValidResponse ? response.Source : null;
		}
		public Task<T> GetAsync<T>(GetRequest getRequest) where T : class => GetAsync<T>(getRequest, new CancellationToken());

		public async Task<T> GetAsync<T>(GetRequest getRequest, CancellationToken cancellationToken) where T : class
		{
			if (getRequest == null) throw new ArgumentNullException(nameof(getRequest));
			var response = await _retryStrategy.RetryAsync(() => _client.GetAsync<T>(getRequest, cancellationToken),
				cancellationToken);
			return response.IsValidResponse ? response.Source : null;
		}

		// TODO: add index/update/delete

		protected virtual void Dispose(bool disposing)
		{
			if (_disposedValue) return;
			if (disposing)
			{
				_client = null;
			}
			_disposedValue = true;
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}

}
