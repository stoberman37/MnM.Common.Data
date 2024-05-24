using System;
using Amazon.DynamoDBv2.DataModel;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.DynamoDB.DependencyInjection.UnitTests
{
	[UnitTest]
	public class DynamoDbRepositoryConfigurationOptionsTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange & Act
			var options = new DynamoDBRepositoryConfigurationOptions();
			var a = () => options.DynamoDBContext();
			var r = () => options.RetryStrategy();

			// Assert
			options.Should().NotBeNull();
			options.DynamoDBContext.Should().NotBeNull();
			a.Should().Throw<NotImplementedException>();
			options.RetryStrategy.Should().NotBeNull();
			r.Should().NotThrow();
			r().Should().BeOfType<RetryStrategyByCount>().Which.MaxRetryCount.Should().Be(0);
		}

		[Fact]
		public void SettersAndGetters_Succeed()
		{
			// Arrange & Act
			var options = new DynamoDBRepositoryConfigurationOptions
			{
				DynamoDBContext = Mock.Of<IDynamoDBContext>,
				RetryStrategy = Mock.Of<IRetryStrategy>
			};

			// Assert
			options.Should().NotBeNull();
			options.DynamoDBContext.Should().NotBeNull();
			options.RetryStrategy.Should().NotBeNull();
		}
	}
}