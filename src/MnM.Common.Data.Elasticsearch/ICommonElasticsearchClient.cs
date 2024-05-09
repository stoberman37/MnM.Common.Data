using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;

namespace MnM.Common.Data.Elasticsearch
{
	public interface ICommonElasticsearchClient : IDisposable
	{
		Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search) where T : class;
		Task<IEnumerable<T>> SearchAsync<T>(SearchRequestDescriptor<T> search, CancellationToken cancellationToken) where T: class;
		Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search) where T : class;
		Task<IEnumerable<T>> SearchAsync<T>(SearchRequest<T> search, CancellationToken cancellationToken) where T : class;
		Task<T> GetAsync<T>(GetRequestDescriptor<T> getRequest) where T : class;
		Task<T> GetAsync<T>(GetRequestDescriptor<T> getRequest, CancellationToken cancellationToken) where T : class;
		Task<T> GetAsync<T>(GetRequest getRequest) where T : class;
		Task<T> GetAsync<T>(GetRequest getRequest, CancellationToken cancellationToken) where T : class;
	}
}