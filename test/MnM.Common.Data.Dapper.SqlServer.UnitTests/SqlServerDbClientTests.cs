using System;
using System.Data.Common;
using Xunit;
using Xunit.Categories;
using FluentAssertions;
using Moq;

namespace MnM.Common.Data.Dapper.SqlServer.UnitTests
{
	[UnitTest]
	public class SqlServerDbClientTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange & Act
			var client = new SqlServerDbClient(Mock.Of<DbConnection>);

			// Assert
			client.Should().NotBeNull();
		}
	}
}
