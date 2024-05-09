using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Dapper
{
	public class DbClient<TException> : IDbClient where TException : Exception
	{
		private const int DefaultCommandTimeout = 30;
		private readonly List<CommandData> _commands = new List<CommandData>();

		private readonly Func<DbConnection> _connectionFactory;

		internal ParameterManager ParameterManager { get; }  = new ParameterManager();

		public string CommandText { get; set; }
		public int CommandTimeout { get; set; } = DefaultCommandTimeout;
		public CommandType CommandType { get; set; } = CommandType.Text;

		public int MaxRetryCount
		{
			get => _retryStrategy.MaxRetryCount;
			set => _retryStrategy.SetMaxRetryCount(value);
		}

		private readonly IRetryStrategy _retryStrategy;

        public DbClient(Func<DbConnection> connectionFactory, IRetryStrategy retryStrategy)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _retryStrategy = retryStrategy ?? throw new ArgumentNullException(nameof(retryStrategy));
        }

		~DbClient()
		{
			Dispose(false);
		}

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

		protected virtual void Dispose(bool disposing)
		{
		}

		public IDbClient SetCommandText(string commandText)
		{
            CommandText = commandText;
            return this;
        }

        public IDbClient SetCommandTimeout(int timeout)
        {
            CommandTimeout = timeout;
            return this;
        }

        public IDbClient SetCommandType(CommandType commandType)
        {
            CommandType = commandType;
            return this;
        }

        public IDbClient SetMaxRetryCount(int retryCount)
        {
	        _retryStrategy.SetMaxRetryCount(retryCount);
            return this;
        }

        public IDbClient AddDbParameter(IDataParameter parameter)
        {
			if (parameter == null)
			{
				return this;
			}
            ParameterManager.AddDbParameter(parameter);
            return this;
        }

        public IDbClient AddDbParameters(IEnumerable<IDataParameter> parameters)
        {
            parameters?.ToList().ForEach(ParameterManager.AddDbParameter);
            return this;
        }

        public IDbClient AddNamedParameters(object namedParameters, CrudMethod crudMethodType)
        {
            ParameterManager.AddNamedParameters(namedParameters, crudMethodType);
            return this;
        }

        public IDbClient AddNamedParameters(object namedParameters)
        {
            return AddNamedParameters(namedParameters, CrudMethod.None);
        }

        public IDbClient PushCommand()
        {
            _commands.Add(new CommandData
            {
                CommandText = CommandText,
                CommandTimeout = CommandTimeout,
                CommandType = CommandType,
                Parameters = ParameterManager.BuildDapperParameters()
            });

            // Clear command text and parameters
            CommandText = "";
            ParameterManager.Clear();
            return this;
        }

        public IEnumerable<T> ExecuteQuery<T>(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(CommandText))
                PushCommand();
            if (_commands.Count > 1) throw new DbClientException("ExecuteQuery only supports a single command");

			try
            {
                return _retryStrategy.Retry(() =>
                {
					using var conn = _connectionFactory.Invoke();
					IEnumerable<T> output;
					conn.Open();
					var transaction = conn.BeginTransaction(isolationLevel);
					var cmd = new CommandDefinition(_commands[0].CommandText,
						_commands[0].Parameters,
						transaction,
						_commands[0].CommandTimeout,
						_commands[0].CommandType,
						cancellationToken: cancellationToken);
					try
					{
						output = conn.Query<T>(cmd);
						ParameterManager.ExtractOutputParameters(_commands[0].Parameters);
						transaction.Commit();
					}
					catch
					{
						transaction?.Rollback();
						throw;
					}

					return output;
				}, cancellationToken);
            }
            finally
            {
                Reset();
            }
        }

        public IEnumerable<TReturn> ExecuteQuery<TConcrete, TReturn>(IsolationLevel isolationLevel, CancellationToken cancellationToken)
            where TConcrete : TReturn
        {
            var result = ExecuteQuery<TConcrete>(isolationLevel, cancellationToken);
            return result.Select(s => s).Cast<TReturn>();
        }

        public int ExecuteNonQuery(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(CommandText))
                PushCommand();
            try
            {
                return _retryStrategy.Retry(() =>
                {
					using var conn = _connectionFactory.Invoke();
					var returnValue = 0;
					conn.Open();
					var transaction = conn.BeginTransaction(isolationLevel);
					try
					{
						foreach (var commandData in _commands)
							returnValue = _retryStrategy.Retry(() =>
							{
								var cmd = new CommandDefinition(commandData.CommandText,
									cancellationToken: cancellationToken,
									parameters: commandData.Parameters,
									commandTimeout: commandData.CommandTimeout,
									commandType: commandData.CommandType);
								var output = conn.Execute(cmd);
								ParameterManager.ExtractOutputParameters(commandData.Parameters);
								return output;
							}, cancellationToken);
						transaction.Commit();
					}
					catch
					{
						transaction?.Rollback();
						throw;
					}

					return returnValue;
				}, cancellationToken);
            }
            finally
            {
                Reset();
            }
        }


        public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(CommandText))
                PushCommand();
            if (_commands.Count > 1) throw new DbClientException("ExecuteQuery only supports a single command");

            try
            {
                var output = await _retryStrategy.RetryAsync(async () =>
                {
					await using var conn = _connectionFactory.Invoke();
					IEnumerable<T> results;
					await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
					var transaction = await conn.BeginTransactionAsync(isolationLevel, cancellationToken);
					var cmd = new CommandDefinition(_commands[0].CommandText,
						_commands[0].Parameters,
						transaction,
						_commands[0].CommandTimeout,
						_commands[0].CommandType);
					try
					{
						results = await conn.QueryAsync<T>(cmd).ConfigureAwait(false);
						ParameterManager.ExtractOutputParameters(_commands[0].Parameters);
						await transaction.CommitAsync(cancellationToken);
					}
					catch
					{
						await transaction.RollbackAsync(cancellationToken)!;
						throw;
					}
					return results;
				}, cancellationToken).ConfigureAwait(false);
                return output;
            }
            finally
            {
                Reset();
            }
        }

        public async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TConcrete, TReturn>(
            IsolationLevel isolationLevel, CancellationToken cancellationToken) where TConcrete : TReturn
        {
	        var result = await ExecuteQueryAsync<TConcrete>(isolationLevel, cancellationToken);
	        return result.Cast<TReturn>();
        }

        public async Task<int> ExecuteNonQueryAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            // support multiple commands
            if (!string.IsNullOrWhiteSpace(CommandText))
                PushCommand();
            try
            {
                return await _retryStrategy.RetryAsync(async () =>
                {
					await using var conn = _connectionFactory.Invoke();
					await conn.OpenAsync(cancellationToken).ConfigureAwait(false);
					var transaction = await conn.BeginTransactionAsync(isolationLevel, cancellationToken);
					var returnValue = 0;
					foreach (var commandData in _commands)
					{
						var cmd = new CommandDefinition(commandData.CommandText,
							commandData.Parameters,
							transaction,
							commandData.CommandTimeout,
							commandData.CommandType,
							cancellationToken: cancellationToken);
						try
						{
							returnValue = await conn.ExecuteAsync(cmd).ConfigureAwait(false);
							ParameterManager.ExtractOutputParameters(commandData.Parameters);
						}
						catch
						{
							await transaction.RollbackAsync(cancellationToken)!;
							throw;
						}
					}

					await transaction.CommitAsync(cancellationToken);
					return returnValue;
				}, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                Reset();
            }
        }

        internal void Reset()
        {
            CommandText = null;
            CommandTimeout = DefaultCommandTimeout;
            CommandType = CommandType.Text;
            ParameterManager.Clear();
            _commands.Clear();
        }

        internal class CommandData
        {
            public string CommandText { get; set; }
            public int CommandTimeout { get; set; }
            public CommandType CommandType { get; set; }
            public DynamicParameters Parameters { get; set; }
        }
    }
}