using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model.Internal.MarshallTransformations;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.DynamoDB.UnitTests
{
	[UnitTest]
	// ReSharper disable once InconsistentNaming
	public class DynamoDBClientTests
	{
		[Fact]
		public void ConstructorTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;

			// Act
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Assert
			client.Should().NotBeNull();
		}

		[Fact]
		public void ConstructorWithStrategyTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var retryStrategy = new Mock<IRetryStrategy>();

			// Act
			var client = new DynamoDBClient<object, object>(DbContextFactory, retryStrategy.Object);

			// Assert
			client.Should().NotBeNull();
		}

		[Theory]
		[ClassData(typeof(ConstructorParametersTestData))]
		public void ConstructorParametersTest(string missingParameter, Func<IDynamoDBContext> factory, IRetryStrategy retryStrategy)
		{
			// Arrange & Act
			var a = () => new DynamoDBClient<object, object>(factory, retryStrategy);

			// Assert
			a.Should().Throw<ArgumentNullException>()
				.Which
				.ParamName.Should().Be(missingParameter);
		}

		[Fact]
		public async Task CreateAsync_NullParameterTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			var a = () => client.CreateAsync(null);

			// Assert
			await a.Should().ThrowAsync<ArgumentNullException>()
				.WithParameterName("toSave");
		}

		[Fact]
		public async Task CreateAsync_SuccessTest()
		{
			// Arrange
			var context = new Mock<IDynamoDBContext>();
			var o = new object();
			context.Setup(c => c.SaveAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
			IDynamoDBContext DbContextFactory() => context.Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			await client.CreateAsync(new object());

			// Assert
			context.Verify(c => c.SaveAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_NullParameterTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			var a = () => client.UpdateAsync(null as object);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("toSave");
		}

		[Fact]
		public async Task UpdateAsync_SuccessTest()
		{
			// Arrange
			var context = new Mock<IDynamoDBContext>();
			var o = new object();
			context.Setup(c => c.SaveAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
			IDynamoDBContext DbContextFactory() => context.Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			await client.UpdateAsync(new object());

			// Assert
			context.Verify(c => c.SaveAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task UpdateAsync_List_NullParameterTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			var a = () => client.UpdateAsync(null);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("toSave");
		}

		// @TODO: figure out how to mock a BatchWrite condition
		//[Fact]
		//public async Task UpdateAsync_List_SuccessTest()
		//{
		//	// Arrange
		//	var context = new Mock<IDynamoDBContext>();
		//	context.Setup(c => c.CreateBatchWrite(It.IsAny<DynamoDBOperationConfig>())).Returns(() =>
		//	{
		//		return new BatchWrite<object>(context, null);
		//	});
		//	var o = new List<object> { 1, 2, 3 };
		//	IDynamoDBContext DbContextFactory() => context.Object;
		//	var client = new DynamoDBClient<object, object>(DbContextFactory);

		//	// Act
		//	await client.UpdateAsync(o);

		//	// Assert
		//	context.Verify(c => c.SaveAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
		//}

		[Fact]
		public async Task DeleteAsync_NullParameterTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			var a = () => client.DeleteAsync(null);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("toDelete");
		}

		[Fact]
		public async Task DeleteAsync_SuccessTest()
		{
			// Arrange
			var context = new Mock<IDynamoDBContext>();
			var o = new object();
			context.Setup(c => c.DeleteAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()));
			IDynamoDBContext DbContextFactory() => context.Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			await client.DeleteAsync(new object());

			// Assert
			context.Verify(c => c.DeleteAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ReadAsync_NullParameterTest()
		{
			// Arrange
			static IDynamoDBContext DbContextFactory() => new Mock<IDynamoDBContext>().Object;
			var client = new DynamoDBClient<object, object>(DbContextFactory);

			// Act
			var a = () => client.ReadAsync(null);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("toRead");
		}

		[Fact]
		public async Task ReadAsync_SuccessTest()
		{
			// Arrange
			var context = new Mock<IDynamoDBContext>();
			var o = new object();
			context.Setup(c => c.LoadAsync<object>(It.IsAny<string>(), new CancellationToken())).ReturnsAsync(o);
			IDynamoDBContext DbContextFactory() => context.Object;
			var client = new DynamoDBClient<object, string>(DbContextFactory);

			// Act
			var result = await client.ReadAsync("test");

			// Assert
			context.Verify(c => c.LoadAsync<object>(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
		}

		private class ConstructorParametersTestData : TheoryData<string, Func<IDynamoDBContext>, IRetryStrategy>
		{

			public ConstructorParametersTestData()
			{
				Add("connectionFactory", null, new Mock<IRetryStrategy>().Object);
				Add("retryStrategy", () => new Mock<IDynamoDBContext>().Object, null);
				Add("connectionFactory", null, null);
			}
		}

		[Fact]
		public void DisposeTest()
		{
			// Arrange
			var dbContextFactory = () => Mock.Of<IDynamoDBContext>();
			var retryStrategy = Mock.Of<IRetryStrategy>();

			// Act
			var a = () =>
			{
				using var client = new DynamoDBClient<object, object>(dbContextFactory, retryStrategy);
			};

			// Assert
			a.Should().NotThrow<Exception>();
		}

	}
}
