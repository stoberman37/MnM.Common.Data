using System;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests
{
	[UnitTest]
	public class RepositoryConfigurationOptionsTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange & Act
			var options = new RepositoryConfigurationOptions();
			var retry = options.RetryStrategy();

			// Assert
			options.Should().NotBeNull();
			options.ElasticsearchClientSettings.Should().BeNull();
			options.RetryStrategy.Should().NotBeNull();
			retry.Should().BeOfType<RetryStrategyByCount>().Which.MaxRetryCount.Should().Be(0);
		}

		[Theory]
		[MemberData(nameof(GetterSetterData))]
		public void SettersAndGetters_Succeeds(ElasticsearchClientSettings settings, Func<IRetryStrategy> retryStrategy)
		{
			// Arrange & Act
			var options = new RepositoryConfigurationOptions
			{
				ElasticsearchClientSettings = settings,
				RetryStrategy = retryStrategy
			};

			// Assert
			options.Should().NotBeNull();
			options.ElasticsearchClientSettings.Should().Be(settings);
			if (retryStrategy == null)
			{
				options.RetryStrategy.Should().BeNull();
			}
			else
			{
				options.RetryStrategy.Should().NotThrow<Exception>();
			}
		}

		public static TheoryData<ElasticsearchClientSettings, Func<IRetryStrategy>> GetterSetterData => new()
		{
			{ new ElasticsearchClientSettings(), Mock.Of<IRetryStrategy> },
			{ null, Mock.Of<IRetryStrategy> },
			{ new ElasticsearchClientSettings(), null },
			{ null, null }
		};

	}
}