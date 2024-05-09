using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace MnM.Common.Data.Dapper.SqlServer
{
    public class SqlServerDbClient : DbClient<SqlException>
    {
        public SqlServerDbClient(Func<DbConnection> connectionFactory) : base(connectionFactory, new SqlServerRetryStrategy())
        {
        }
    }
}