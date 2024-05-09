using Dapper;
using System;
using System.Data;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.Dapper.UnitTests
{
	[UnitTest]
	public class CommandDataTests
	{
		[Fact]
		public void GettersAndSettersTest()
		{
			// Arrange
			const string expectedCommandText = "Command Text";
			const int expectedCommandTimeout = 100;
			const CommandType expectedCommandType = CommandType.Text;
			var expectedParams = new DynamicParameters();

			// Act
			var cmdData = new DbClient<Exception>.CommandData
			{
				CommandText = "Command Text",
				CommandTimeout = 100,
				CommandType = CommandType.Text,
				Parameters = expectedParams
			};
			var cmdText = cmdData.CommandText;
			var cmdTimeout = cmdData.CommandTimeout;
			var cmdType = cmdData.CommandType;
			var cmdParams = cmdData.Parameters;

			// Assert
			cmdText.Should().Be(expectedCommandText);
			cmdTimeout.Should().Be(expectedCommandTimeout);
			cmdType.Should().Be(expectedCommandType);
			cmdParams.Should().BeSameAs(expectedParams);
		}
	}
}
