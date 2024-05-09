using System;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

#pragma warning disable IDE0062
namespace MnM.Common.Data.UnitTests
{
	[UnitTest]
	public class RetryStrategyByCountTests
	{
		[Fact]
		public void ConstructorTest()
		{
			// Arrange & Act
			var s = new RetryStrategyByCount();

			// Assert
			s.Should().NotBeNull();
			s.RetryCount.Should().Be(0);
			s.MaxRetryCount.Should().Be(0);
		}

		[Fact]
		public void MaxRetryCountTest()
		{
			// Arrange
			var s = new RetryStrategyByCount();

			// Act
			s.SetMaxRetryCount(10);

			// Assert
			s.MaxRetryCount.Should().Be(10);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(5)]
		[InlineData(10)]
		public void RetryWithNoReturnFailsAfterCorrectRetriesTest(int retryCount)
		{
			// Arrange
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(retryCount);
			void Action() => throw new Exception("This failed.  Finally");

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should().Throw<Exception>().WithMessage("This failed.  Finally");
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Fact]
		public void RetryWithNoReturnSucceedsAfterFailingTest()
		{
			// Arrange
			int outer = 0;
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			void Action()
			{
				if (++outer <= 1)
					throw new Exception("This failed the first time");
			}

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should().NotThrow<Exception>();
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
		}

		[Fact]
		public void RetryWithNoReturnCancellationTest()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			void Action()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}
				throw new Exception("This failed the first time");
			}

			// Act
			var a = () => s.Retry(Action, source.Token);

			// Assert
			a.Should().Throw<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(5)]
		[InlineData(10)]
		public async Task RetryAsyncWithNoReturnFailsAfterCorrectRetriesTest(int retryCount)
		{
			// Arrange
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(retryCount);
			Task ActionAsync() => throw new Exception("This failed.  Finally");

			// Act
			var a = () => s.RetryAsync(ActionAsync);

			// Assert
			await a.Should().ThrowAsync<Exception>();
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Fact]
		public async Task RetryAsyncWithNoReturnSucceedsAfterFailingTest()
		{
			// Arrange
			var outer = 0;
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			Task ActionAsync()
			{
				if (++outer <= 1)
				{
					throw new Exception("This failed the first time");
				}
				return Task.FromResult(0);
			}

			// Act
			var a = () => s.RetryAsync(ActionAsync);

			// Assert
			await a.Should().NotThrowAsync<Exception>();
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
		}

		[Fact]
		public async Task RetryAsyncWithNoReturnCancellationTest()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			Task ActionAsync()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}
				throw new Exception("This failed the first time");
			}

			// Act
			var a= () => s.RetryAsync(ActionAsync, source.Token);

			// Assert
			await a.Should()
				.ThrowAsync<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(5)]
		[InlineData(10)]
		public void RetryWithReturnFailsAfterCorrectRetriesTest(int retryCount)
		{
			// Arrange
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(retryCount);
			int Action() => throw new Exception("This failed.  Finally");

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should()
				.Throw<Exception>()
				.WithMessage("This failed.  Finally");
			s.RetryCount.Should().Be(retryCount+1);
		}

		[Fact]
		public void RetryWithReturnSucceedsAfterFailingTest()
		{
			// Arrange
			int outer = 0;
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			int Action()
			{
				if (++outer <= 1)
					throw new Exception("This failed the first time");
				return 1000;
			}

			// Act
			var result = s.Retry(Action);

			// Assert
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
			result.Should().Be(1000);
		}

		[Fact]
		public void RetryWithReturnCancellationTest()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			int Action()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}
				throw new Exception("This failed the first time");
			}

			// Act
			var a = () => s.Retry(Action, source.Token);

			// Assert
			a.Should().Throw<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(5)]
		[InlineData(10)]
		public async Task RetryAsyncWithReturnFailsAfterCorrectRetriesTest(int retryCount)
		{
			// Arrange
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(retryCount);
			Task<int> ActionAsync() => throw new Exception("This failed.  Finally");

			// Act
			var a = () => s.RetryAsync(ActionAsync);

			// Assert
			await a.Should().ThrowAsync<Exception>();
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Fact]
		public async Task RetryAsyncWithReturnSucceedsAfterFailingTest()
		{
			// Arrange
			var outer = 0;
			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			Task<int> ActionAsync()
			{
				if (++outer <= 1)
					throw new Exception("This failed the first time");
				return Task.FromResult(1000);
			}

			// Act
			var result = await s.RetryAsync(ActionAsync);

			// Assert
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
			result.Should().Be(1000);
		}

		[Fact]
		public async Task RetryAsyncWithReturnCancellationTest()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new RetryStrategyByCount();
			s.SetMaxRetryCount(2);
			Task<int> ActionAsync()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}
				throw new Exception("This failed the first time");
			}

			// Act
			var a = () => s.RetryAsync(ActionAsync, source.Token);

			// Assert
			await a.Should().ThrowAsync<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}
	}
}

