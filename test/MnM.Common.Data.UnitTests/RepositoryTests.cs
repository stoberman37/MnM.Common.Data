using System;
using System.Collections.Generic;
using System.Linq;
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
		public void DoNothing()
		{
		}

		public void Dispose()
		{
		}
	}

	[UnitTest]
	public class RepositoryTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);

			// Assert
			repo.Should().NotBeNull();
		}

		[Fact]
		public void Constructor_ThrowsException_WithNullFactory()
		{
			// Arrange & Act
			var a = () => new Repository<DummyClient, object>(null);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("factory");
		}

		[Fact]
		public void ExecuteDbAction_WithActionParam_ThrowsException_WithNullAction()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as Action<object>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public void ExecuteDbAction_WithActionParam_Succeeds()
		{
			// Arrange
			var repo = new Repository<DummyClient, object>(Factory);
			DummyClient Factory() => new();

			// Act
			var a = () => repo.ExecuteDbAction(_ => { });

			// Assert
			a.Should().NotThrow<Exception>();
		}

		[Fact]
		public void ExecuteDbAction_WithFuncParam_ThrowsException_WithNullFunc()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as Func<DummyClient, object>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("func");
		}

		[Fact]
		public void ExecuteDbAction_WithFuncParam_Succeeds()
		{
			// Arrange
			var expectedResult = new object();
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			object DbAction(DummyClient c) => expectedResult;
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var result = repo.ExecuteDbAction(DbAction);

			// Assert
			result.Should().NotBeNull().And.BeSameAs(expectedResult);
		}

		[Fact]
		public void ExecuteDbAction_WithFuncParamReturningCollection_ThrowsException_WithNullFunc()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as Func<DummyClient, IEnumerable<object>>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("func");
		}

		[Fact]
		public void ExecuteDbAction_WithFuncParamReturningCollection_Succeeds()
		{
			// Arrange
			var expectedResult = new List<object>();
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			IEnumerable<object> DbAction(DummyClient c) => expectedResult;
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var result = repo.ExecuteDbAction(DbAction);

			// Assert
			result.Should().NotBeNull().And.BeSameAs(expectedResult);
		}

		[Fact]
		public void ExecuteDbAction_WithINonQuerySpecification_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as INonQuerySpecification<DummyClient>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithINonQuerySpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecification<DummyClient>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute()).Returns(c => { });
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(), Times.Once);
		}

		[Fact]
		public void ExecuteDbAction_WithINonQuerySpecificationAndCancellationToken_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(null as INonQuerySpecification<DummyClient>, cancellationToken);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithINonQuerySpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecification<DummyClient>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute(It.IsAny<CancellationToken>())).Returns(c => { });
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object, cancellationToken);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public void ExecuteDbAction_WithIQuerySpecification_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as IQuerySpecification<DummyClient, object>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithIQuerySpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQuerySpecification<DummyClient, object>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute()).Returns(_ => new object());
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(), Times.Once);
		}

		[Fact]
		public void ExecuteDbAction_WithIQuerySpecificationAndCancellationToken_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(null as IQuerySpecification<DummyClient, object>, cancellationToken);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithIQuerySpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQuerySpecification<DummyClient, object>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute(It.IsAny<CancellationToken>()))
				.Returns(_ => new object());
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object, cancellationToken);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public void ExecuteDbAction_WithIQueryListSpecification_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(null as IQueryListSpecification<DummyClient, object>);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithIQueryListSpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQueryListSpecification<DummyClient, object>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute()).Returns(_ => new List<object> { new ()});
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(), Times.Once);
		}

		[Fact]
		public void ExecuteDbAction_WithIQueryListSpecificationAndCancellationToken_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => Mock.Of<DummyClient>(MockBehavior.Strict);
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(null as IQueryListSpecification<DummyClient, object>, cancellationToken);

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public void ExecuteDbAction_WithIQueryListSpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQueryListSpecification<DummyClient, object>>(MockBehavior.Strict);
			spec.Setup(s => s.Execute(It.IsAny<CancellationToken>()))
				.Returns(_ => new List<object> { new () });
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);
			var cancellationToken = new CancellationToken();

			// Act
			var a = () => repo.ExecuteDbAction(spec.Object, cancellationToken);

			// Assert
			a.Should().NotThrow<Exception>();
			spec.Verify(s => s.Execute(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithNoReturn_ThrowsException_withNullAction()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);
			var a = () => repo.ExecuteDbActionAsync(null as Func<DummyClient, Task>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithNoReturn_Succeeds()
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
		public async Task ExecuteDbActionAsync_WithReturn_ThrowsException_withNullAction()
		{
			// Arrange
			DummyClient Factory() => new();

			// Act
			var repo = new Repository<DummyClient, object>(Factory);
			var a = () => repo.ExecuteDbActionAsync(null as Func<DummyClient, Task<object>>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("action");
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithReturn_Succeeds()
		{
			// Arrange
			DummyClient Factory() => new();
			Task<object> DbAction(DummyClient c) => Task.FromResult(new object());
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var result = await repo.ExecuteDbActionAsync(DbAction);

			// Assert
			result.Should().NotBeNull();
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithCollectionReturn_ThrowsException_withNullAction()
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
		public async Task ExecuteDbActionAsync_WithCollectionReturn_Succeeds()
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
		public async Task ExecuteDbActionAsync_WithINonQuerySpecification_ThrowsException_WithNullSpecification()
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
		public async Task ExecuteDbActionAsync_WithINonQuerySpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecificationAsync<DummyClient>>(MockBehavior.Strict);
			Task Task(DummyClient c) => System.Threading.Tasks.Task.FromResult(true);
			spec.Setup(s => s.ExecuteAsync()).Returns(Task);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			await repo.ExecuteDbActionAsync(spec.Object);

			// Assert
			spec.Verify(s => s.ExecuteAsync(), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithINonQuerySpecificationAndCancellation_ThrowsException_WithNullSpecification()
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
		public async Task ExecuteDbActionAsync_WithINonQuerySpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<INonQuerySpecificationAsync<DummyClient>>(MockBehavior.Strict);
			Task Task(DummyClient c) => System.Threading.Tasks.Task.FromResult(true);
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
		public async Task ExecuteDbActionAsync_WithIQuerySpecification_ThrowsException_WithNullSpecification()
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
		public async Task ExecuteDbActionAsync_WithIQuerySpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQuerySpecificationAsync<DummyClient, object>>(MockBehavior.Strict);

			Task<object> DbAction(DummyClient c) => Task.FromResult(new object());
			spec.Setup(s => s.ExecuteAsync()).Returns(DbAction);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			await repo.ExecuteDbActionAsync(spec.Object);

			// Assert
			spec.Verify(s => s.ExecuteAsync(), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithIQuerySpecificationAndCancellation_ThrowsException_WithNullSpecification()
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
		public async Task ExecuteDbActionAsync_WithIQuerySpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQuerySpecificationAsync<DummyClient, object>>();
			Task<object> DbAction(DummyClient c) => Task.FromResult(new object());
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

		[Fact]
		public async Task ExecuteDbActionAsync_WithIQueryListSpecification_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as IQueryListSpecificationAsync<DummyClient, object>);

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithIQueryListSpecification_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQueryListSpecificationAsync<DummyClient, object>>(MockBehavior.Strict);

			Task<IEnumerable<object>> DbAction(DummyClient c) => Task.FromResult(new List<object>() as IEnumerable<object>);
			spec.Setup(s => s.ExecuteAsync()).Returns(DbAction);
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			await repo.ExecuteDbActionAsync(spec.Object);

			// Assert
			spec.Verify(s => s.ExecuteAsync(), Times.Once);
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithIQueryListSpecificationAndCancellation_ThrowsException_WithNullSpecification()
		{
			// Arrange
			DummyClient Factory() => new();
			var repo = new Repository<DummyClient, object>(Factory);

			// Act
			var a = () => repo.ExecuteDbActionAsync(null as IQueryListSpecificationAsync<DummyClient, object>, new CancellationToken());

			// Assert
			await a.Should()
				.ThrowAsync<ArgumentNullException>()
				.WithParameterName("specification");
		}

		[Fact]
		public async Task ExecuteDbActionAsync_WithIQueryListSpecificationAndCancellationToken_Succeeds()
		{
			// Arrange
			var spec = new Mock<IQueryListSpecificationAsync<DummyClient, object>>();
			Task<IEnumerable<object>> DbAction(DummyClient c) => Task.FromResult(new List<object>() as IEnumerable<object>);
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
