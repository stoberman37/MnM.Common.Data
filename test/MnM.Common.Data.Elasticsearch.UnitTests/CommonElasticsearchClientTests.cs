using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Moq;
using Xunit;
using FluentAssertions;
using Xunit.Categories;

#pragma warning disable IDE0062
namespace MnM.Common.Data.Elasticsearch.UnitTests
{
	[UnitTest]
	public class CommonElasticsearchClientTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange
			ElasticsearchClient Factory() => new();

			// Act
			var repo = new CommonElasticsearchClient(Factory);

			// Assert
			repo.Should().NotBeNull();
		}

		[Fact]
		public void Constructor_WithNullFactory_Fails()
		{
			// Arrange & Act
			var a = () => new CommonElasticsearchClient(null);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("factory");
		}

		[Fact]
		public void Constructor_WithNullRetryStrategy_Fails()
		{
			// Arrange & Act
			var a = () => new CommonElasticsearchClient(() => new ElasticsearchClient(), null);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("retryStrategy");
		}

		[Fact]
		public async Task SearchAsync_Fails_WithNullSearchRequest()
		{
			// Arrange
			ElasticsearchClient Factory() => new();
			var repo = new CommonElasticsearchClient(Factory);

			// Act
			var a = () => repo.SearchAsync(null as SearchRequest<object>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("search");
		}

		[Fact]
		public async Task SearchAsync_Fails_WithNullSearchRequestDescriptor()
		{
			// Arrange
			ElasticsearchClient Factory() => new();
			var repo = new CommonElasticsearchClient(Factory);

			// Act
			var a = () => repo.SearchAsync(null as SearchRequestDescriptor<object>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("search");
		}

		[Fact]
		public async Task GetAsync_Fails_WithNullSearchRequest()
		{
			// Arrange
			ElasticsearchClient Factory() => new();
			var repo = new CommonElasticsearchClient(Factory);

			// Act
			var a = () => repo.GetAsync<object>(null as GetRequest);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("getRequest");
		}

		[Fact]
		public async Task GetAsync_Fails_WithNullSearchRequestDescriptor()
		{
			// Arrange
			ElasticsearchClient Factory() => new();
			var repo = new CommonElasticsearchClient(Factory);

			// Act
			var a = () => repo.GetAsync(null as GetRequestDescriptor<object>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("getRequest");
		}
	}
}
