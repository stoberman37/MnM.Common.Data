using System;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Dapper
{
    [Serializable]
    public class DbClientException : Exception
    {
        public DbClientException()
        {
        }

        public DbClientException(string message)
            : base(message)
        {
        }

        public DbClientException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}