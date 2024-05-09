using System;
using System.Data.SqlClient;

namespace MnM.Common.Data.Dapper.SqlServer
{

	public class SqlServerRetryStrategy : RetryStrategyBase<SqlException>
	{
		internal static  Func<RetryStrategyBase<SqlException>, SqlException, bool> RetryFunc = (s, ex) =>
			s.RetryCount <= s.MaxRetryCount &&
			(ex.Number == -2 || ex.Number == 11 || ex.Number == 1205 || ex.Number == 11001);

		public SqlServerRetryStrategy() : base(RetryFunc)
		{
		}
	}
}