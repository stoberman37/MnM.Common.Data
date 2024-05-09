using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.Dapper.UnitTests
{
	[UnitTest]
	public class DbClientExceptionTests
	{
		[Fact]
		public void DefaultConstructorTest()
		{
			// Arrange
			const string defaultMessage = "Exception of type 'MnM.Common.Data.Dapper.DbClientException' was thrown.";
			// Act
			var ex = new DbClientException();

			// Assert
			ex.Should().NotBeNull();
			ex.Message.Should().Be(defaultMessage);
			ex.InnerException.Should().BeNull();
		}

		[Fact]
		public void SimpleMessageConstructorTest()
		{
			// Arrange
			const string expectedMessage = "Expected message";
			// Act
			var ex = new DbClientException(expectedMessage);

			// Assert
			ex.Should().NotBeNull();
			ex.Message.Should().Be(expectedMessage);
			ex.InnerException.Should().BeNull();
		}

		[Fact]
		public void SimpleMessageWithExceptionConstructorTest()
		{
			// Arrange
			const string expectedMessage = "Expected message";
			var expectedInnerException = new Exception();

			// Act
			var ex = new DbClientException(expectedMessage, expectedInnerException);

			// Assert
			ex.Should().NotBeNull();
			ex.Message.Should().Be(expectedMessage);
			ex.InnerException.Should().NotBeNull()
				.And.Be(expectedInnerException);
		}
	}
}
