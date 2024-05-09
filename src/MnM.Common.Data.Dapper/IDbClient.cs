using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Dapper
{
	/// <summary>
	/// DbClient interface
	/// </summary>
	public interface IDbClient : IDisposable
	{
		/// <summary>
		/// Add an IDataParameter for the query
		/// </summary>
		/// <param name="parameter">parameter to add</param>
		/// <returns>the IDbClient instance</returns>
		IDbClient AddDbParameter(IDataParameter parameter);

		/// <summary>
		/// Add a collection of IDataParameters for the query
		/// </summary>
		/// <param name="parameters">collection of parameters to add</param>
		/// <returns>the IDbClient instance</returns>
		IDbClient AddDbParameters(IEnumerable<IDataParameter> parameters);

		/// <summary>
		/// Add the properties of the specified object to the query 
		/// </summary>
		/// <param name="namedParameters">object who's properties are added to the query call</param>
		/// <param name="crudMethodType">Crud operation being performed</param>
		/// <returns>the IDbClient instance</returns>
		IDbClient AddNamedParameters(object namedParameters, CrudMethod crudMethodType);

		/// <summary>
		/// Add the properties of the specified object to the query 
		/// </summary>
		/// <param name="namedParameters">object who's properties are added to the query call</param>
		/// <returns>the IDbClient instance</returns>
		IDbClient AddNamedParameters(object namedParameters);

		/// <summary>
		/// Set the command text for the query
		/// </summary>
		/// <param name="commandText"></param>
		/// <returns>the IDbClient instance</returns>
		IDbClient SetCommandText(string commandText);

		/// <summary>
		///  Set the command type for the query
		/// </summary>
		/// <param name="commandType"></param>
		/// <returns>the IDbClient instance</returns>
		IDbClient SetCommandType(CommandType commandType);

		/// <summary>
		///     Set the retry count for the query
		/// </summary>
		/// <param name="retryCount"></param>
		/// <returns>the IDbClient instance</returns>
		IDbClient SetMaxRetryCount(int retryCount);

		/// <summary>
		/// Set the timeout value for the comment
		/// </summary>
		/// <param name="timeout"></param>
		/// <returns>the IDbClient instance</returns>
		IDbClient SetCommandTimeout(int timeout);

		/// <summary>
		///     pushes the current command settings onto the command stack. This allows for setting up multiple commands.
		/// </summary>
		/// <returns></returns>
		IDbClient PushCommand();

		/// <summary>
		/// Execute the query, returning a collection of type T
		/// </summary>
		/// <param name="cancellationToken">task cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <typeparam name="T">Type of the object be returns</typeparam>
		/// <returns>query results</returns>
		IEnumerable<T> ExecuteQuery<T>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

		/// <summary>
		///     Execute the query, returning a collection of type TReturn
		/// </summary>
		/// <typeparam name="TConcrete">concrete type to instantiate for return</typeparam>
		/// <typeparam name="TReturn">type to cast return to</typeparam>
		/// <param name="cancellationToken">cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <returns></returns>
		IEnumerable<TReturn> ExecuteQuery<TConcrete, TReturn>(
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
			CancellationToken cancellationToken = default) where TConcrete : TReturn;

		/// <summary>
		///     Execute a non-result query
		/// </summary>
		/// <param name="cancellationToken">cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <returns>return value from the query</returns>
		int ExecuteNonQuery(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

		/// <summary>
		///     Execute the query asynchronously
		/// </summary>
		/// <param name="cancellationToken">task cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <typeparam name="T">Type of return object</typeparam>
		/// <returns>awaitable query results</returns>
		Task<IEnumerable<T>> ExecuteQueryAsync<T>(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

		/// <summary>
		///     Execute the query asynchronously, returning a collection of type TReturn
		/// </summary>
		/// <typeparam name="TConcrete">concrete type to instantiate for return</typeparam>
		/// <typeparam name="TReturn">type to cast return to</typeparam>
		/// <param name="cancellationToken">cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <returns></returns>
		Task<IEnumerable<TReturn>> ExecuteQueryAsync<TConcrete, TReturn>(
			IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
			where TConcrete : TReturn;

		/// <summary>
		///     Execute a non-result query asynchronously
		/// </summary>
		/// <param name="cancellationToken">task cancellation token</param>
		/// <param name="isolationLevel">transaction isolation level</param>
		/// <returns>awaitable return value from the query</returns>
		Task<int> ExecuteNonQueryAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
	}
}