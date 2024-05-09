using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using MnM.Common.Data.Repositories;
using MnM.Common.Data.Specifications;
using FluentAssertions;
using Xunit.Categories;

#pragma warning disable IDE0062
namespace MnM.Common.Data.UnitTests
{
	public class DummyClient : IDisposable
	{
		public void Dispose()
		{
		}
	}

	[UnitTest]
	public class RepositoryTests
	{
		[Fact]
		public void ConstructorTest()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);

			// Assert
			repo.Should().NotBeNull();
		}

		[Fact]
		public void ConstructorWithNullFactoryTest()
		{
			// Arrange & Act
			var a = () => new Repository<DummyClient, object>(null);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("factory");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncActionNullTest()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);
			var a = () => repo.ExecuteDbActionAsync(null as Func<DummyClient, Task<IEnumerable<object>>>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncNullTest()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);
			var a = ()=> repo.ExecuteDbActionAsync((Func<DummyClient, Task>)null);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncTest()
		{
			// Arrange
			DummyClient Factory() => new();
			Task Task(DummyClient c) => System.Threading.Tasks.Task.FromResult(true);

			// Act
			var repo = new Repository<DummyClient, object>(Factory);
			var a = () => repo.ExecuteDbActionAsync(Task);
			

			// Assert
			await a.Should()
				.NotThrowAsync<Exception>();
		}

		[Fact]
		public async Task ExecuteDbActionAsyncActionWithReturnNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as Func<DummyClient, Task<IEnumerable<object>>>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithReturnTest()
		{
			// Arrange
			DummyClient Factory() => new();
			Task<IEnumerable<object>> DbAction(DummyClient c) =>
				Task.FromResult(new List<object>() as IEnumerable<object>);
			var repo = new Repository<DummyClient, object>(Factory);


			// Act
			var result = await repo.ExecuteDbActionAsync(DbAction);

			// Assert
			result.Should().NotBeNull();
		}


		[Fact]
		public void ExecuteDbActionActionNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction((Action<DummyClient>) null);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public void ExecuteDbActionTest()
		{
			// Arrange
			var repo = new Repository<DummyClient, object>(Factory);
			DummyClient Factory() => new();
			void Func(DummyClient c)
			{
			}

			// Act
			var a = () => repo.ExecuteDbAction(Func);

			// Assert
			a.Should().NotThrow<Exception>();
		}

		[Fact]
		public void ExecuteDbActionActionWithReturnNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as Action<DummyClient>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public void ExecuteDbActionWithReturnTest()
		{
			// Arrange
			DummyClient Factory() => new Mock<DummyClient>().Object;
			IEnumerable<object> DbAction(DummyClient c) => new List<object>();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var result = repo.ExecuteDbAction(DbAction);

			// Assert
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithNonQuerySpecificationNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as INonQuerySpecificationAsync<DummyClient>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithNonQuerySpecificationAndCancellationNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as INonQuerySpecificationAsync<DummyClient>, new CancellationToken());

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithNonQuerySpecificationTest()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecificationAsync<DummyClient>>();
			Task Task(DummyClient c) => System.Threading.Tasks.Task.FromResult(true);
			spec.Setup(s => s.ExecuteAsync()).Returns(Task);
			spec.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).Returns(Task);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			await repo.ExecuteDbActionAsync(spec.Object);

			// Assert
			spec.Verify(s => s.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithNonQuerySpecificationAndCancellationTokenTest()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecificationAsync<DummyClient>>();
			Task Task(DummyClient c) => System.Threading.Tasks.Task.FromResult(true);
			spec.Setup(s => s.ExecuteAsync()).Returns(Task);
			spec.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).Returns(Task);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);
			var cancelToken = new CancellationToken();

			// Act
			await repo.ExecuteDbActionAsync(spec.Object, cancelToken);

			// Assert
			spec.Verify(s => s.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithQuerySpecificationNullTest()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as IQuerySpecificationAsync<DummyClient, object>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithQuerySpecificationAndCancellationNullTests()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as IQuerySpecificationAsync<DummyClient, object>, new CancellationToken());

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithQuerySpecificationTest()
		{
			// Arrange
			var spec = new Mock<IQuerySpecificationAsync<DummyClient, object>>();

			Task<IEnumerable<object>> DbAction(DummyClient c) =>
				Task.FromResult(new List<object>() as IEnumerable<object>);
			spec.Setup(s => s.ExecuteAsync()).Returns(DbAction);
			spec.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).Returns(DbAction);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			await repo.ExecuteDbActionAsync(spec.Object);

			// Assert
			spec.Verify(s => s.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsyncWithQuerySpecificationAndCancellationTokenTest()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecificationAsync<DummyClient>>();
			Task<IEnumerable<object>> DbAction(DummyClient c) =>
				Task.FromResult(new List<object>() as IEnumerable<object>);
			spec.Setup(s => s.ExecuteAsync()).Returns(DbAction);
			spec.Setup(s => s.ExecuteAsync(It.IsAny<CancellationToken>())).Returns(DbAction);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);
			var cancelToken = new CancellationToken();

			// Act
			await repo.ExecuteDbActionAsync(spec.Object, cancelToken);

			// Assert
			spec.Verify(s => s.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
