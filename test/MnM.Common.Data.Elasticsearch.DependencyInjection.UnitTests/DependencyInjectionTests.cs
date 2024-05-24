using System;
using System.Diagnostics.CodeAnalysis;
using MnM.Common.Data.Repositories;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.Elasticsearch.DependencyInjection.UnitTests
{
	[UnitTest]
	[ExcludeFromCodeCoverage]
	public class DependencyInjectionTests
	{

		[Fact]
		public void AddElasticsearchRepository_Succeeds()
		{
			// Arrange
			// TODO: Figure out how to mock the retry strategy func 
			var services = new ServiceCollection();
			var settings = new ElasticsearchRepositoryConfigurationOptions
				{ ElasticsearchClientSettings = new ElasticsearchClientSettings(), RetryStrategy = Mock.Of<IRetryStrategy> };

			// Act
			services.AddElasticsearchRepository<object>(settings);
			var provider = services.BuildServiceProvider();
			var elasticsearchClientFunc = provider.GetService<Func<ElasticsearchClient>>();
			var commonClientFunc = provider.GetService<Func<ICommonElasticsearchClient>>();
			var repo = provider.GetService<IRepository<ICommonElasticsearchClient, object>>();

			// Assert
			elasticsearchClientFunc.Should().NotBeNull();
			commonClientFunc.Should().NotBeNull();
			repo.Should().NotBeNull();
		}

		[Fact]
		public void AddElasticsearchRepository_Fails_WithNullServiceCollection()
		{
			// Arrange & Act
			var a = () =>
				(null as ServiceCollection).AddElasticsearchRepository<object>(new ElasticsearchRepositoryConfigurationOptions());

			// Assert
			a.Should().NotBeNull();
			a.Should().Throw<ArgumentNullException>().WithParameterName("this");
		}

		[Fact]
		public void AddElasticsearchRepository_Fails_WithNullRepositoryConfigurationOptions()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act
			var a = () => services.AddElasticsearchRepository<object>(null);

			// Assert
			a.Should().Throw<ArgumentNullException>()
				.WithParameterName("options");
		}

		[Fact]
		public void AddElasticsearchRepository_Fails_WithNullElasticsearchClientSettings()
		{
			// Arrange
			var services = new ServiceCollection();
			var settings = new ElasticsearchRepositoryConfigurationOptions() { ElasticsearchClientSettings = null };

			// Act
			var a = () => services.AddElasticsearchRepository<object>(settings);

			// Assert
			a.Should().Throw<ArgumentException>()
				.WithParameterName("options")
				.WithMessage("ElasticsearchClientSettings cannot be null (Parameter 'options')");
		}

		[Fact]
		public void AddElasticsearchRepository_Fails_WithNullRetryStrategy()
		{
			// Arrange
			var services = new ServiceCollection();
			var settings = new ElasticsearchRepositoryConfigurationOptions()
			{
				ElasticsearchClientSettings = new ElasticsearchClientSettings(),
				RetryStrategy = null
			};

			// Act
			var a = () => services.AddElasticsearchRepository<object>(settings);

			// Assert
			a.Should().Throw<ArgumentException>()
				.WithParameterName("options")
				.WithMessage("RetryStrategy cannot be null (Parameter 'options')");
		}
	}
}