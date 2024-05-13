using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MnM.Common.Data.DynamoDB
{
	public interface IDynamoDBClient<T, TKey> : IDisposable
		where T : class 
		where TKey : notnull
	{
		Task CreateAsync(T toSave);
		Task CreateAsync(T toSave, CancellationToken cancellationToken);
		Task UpdateAsync(T toSave);
		Task UpdateAsync(T toSave, CancellationToken cancellationToken);
		Task UpdateAsync(IEnumerable<T> toSave);
		Task UpdateAsync(IEnumerable<T> toSave, CancellationToken cancellationToken);
		Task DeleteAsync(T toDelete);
		Task DeleteAsync(T toDelete, CancellationToken cancellationToken);
		Task<T> ReadAsync(TKey toRead);
		Task<T> ReadAsync(TKey toRead, CancellationToken cancellationToken);
	}
}