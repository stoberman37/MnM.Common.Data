using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

#pragma warning disable IDE0062
namespace MnM.Common.Data.Dapper.SqlServer.UnitTests
{
	[UnitTest]
	public class SqlServerRetryStrategyTests
	{
		private static SqlException CreateSqlException(int number) => CreateSqlException(number, string.Empty);

		private static SqlException CreateSqlException(int number, string message)
		{
			Exception? innerEx = null;
			var c = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
			SqlErrorCollection errors = (c[0].Invoke(null) as SqlErrorCollection)!;
			var errorList =
				(errors.GetType().GetField("_errors", BindingFlags.Instance | BindingFlags.NonPublic)
					?.GetValue(errors) as List<object>)!;
			c = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
			var nineC = c.FirstOrDefault(f => f.GetParameters().Length == 9)!;
			SqlError sqlError = (nineC.Invoke(new object?[]
				{ number, (byte)0, (byte)0, "", "", "", (int)0, (uint)0, innerEx }) as SqlError)!;
			errorList.Add(sqlError);
			SqlException ex = (Activator.CreateInstance(typeof(SqlException),
				BindingFlags.NonPublic | BindingFlags.Instance, null, new object?[]
				{
					message, errors,
					innerEx, Guid.NewGuid()
				}, null) as SqlException)!;
			return ex;
		}

		[Fact]
		public void ConstructorTest()
		{
			// Arrange & Act
			var s = new SqlServerRetryStrategy();

			// Assert
			s.Should().NotBeNull();
			s.RetryCount.Should().Be(0);
			s.MaxRetryCount.Should().Be(0);
		}

		#region Sync Tests

		[Theory]
		[ClassData(typeof(RetryCountErrorNumberCrossProduct))]
		public void Retry_WithNoReturn_Fails_AfterCorrectRetries(int retryCount, int errorNumber)
		{
			// Arrange
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(retryCount);
			void Action() => throw CreateSqlException(errorNumber, $"This failed.  Finally: {s.RetryCount}");

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should().Throw<SqlException>().WithMessage($"This failed.  Finally: {retryCount}");
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Theory]
		[ClassData(typeof(ErrorNumbers))]
		public void Retry_WithReturn_Succeeds_AfterFailing(int number)
		{
			// Arrange
			var outer = 0;
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			int Action()
			{
				if (++outer <= 1)
				{
					throw CreateSqlException(number);
				}

				return 1000;
			}

			// Act
			var result = s.Retry(Action);

			// Assert
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
			result.Should().Be(1000);
		}

		[Theory]
		[ClassData(typeof(ErrorNumbers))]
		public void Retry_WithNoReturn_Succeeds_AfterFailing(int number)
		{
			// Arrange
			int outer = 0;
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			void Action()
			{
				if (++outer <= 1)
					throw CreateSqlException(number);
			}

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should().NotThrow<Exception>();
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
		}

		[Fact]
		public void Retry_WithNoReturn_Cancel_Success()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			void Action()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}

				throw CreateSqlException(-12, "This failed the first time");
			}

			// Act
			var a = () => s.Retry(Action, source.Token);

			// Assert
			a.Should().Throw<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		[Theory]
		[ClassData(typeof(RetryCountErrorNumberCrossProduct))]
		public void Retry_WithReturn_Fails_AfterCorrectRetries(int retryCount, int errorNumber)
		{
			// Arrange
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(retryCount);
			int Action() => throw CreateSqlException(errorNumber, $"This failed.  Finally, {s.RetryCount}");

			// Act
			var a = () => s.Retry(Action);

			// Assert
			a.Should()
				.Throw<SqlException>()
				.WithMessage($"This failed.  Finally, {retryCount}");
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Fact]
		public void Retry_WithReturn_Cancel_Success()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			int Action()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}

				throw CreateSqlException(-12, "This failed the first time");
			}

			// Act
			var a = () => s.Retry(Action, source.Token);

			// Assert
			a.Should().Throw<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		#endregion

		#region RetryAsync Tests

		[Theory]
		[ClassData(typeof(RetryCounts))]
		public async Task RetryAsync_WithNoReturn_Fails_AfterCorrectRetries(int retryCount)
		{
			// Arrange
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(retryCount);
			Task ActionAsync() => throw CreateSqlException(-2, "This failed.  Finally");

			// Act
			var a = () => s.RetryAsync(ActionAsync);

			// Assert
			await a.Should().ThrowAsync<Exception>();
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Theory]
		[ClassData(typeof(ErrorNumbers))]
		public async Task RetryAsync_WithNoReturn_Succeeds_AfterFailing(int errorNumber)
		{
			// Arrange
			var outer = 0;
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			Task ActionAsync()
			{
				if (++outer <= 1)
				{
					throw CreateSqlException(errorNumber, "This failed the first time");
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
		public async Task RetryAsync_WithNoReturn_Cancel_Success()
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);

			Task ActionAsync()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}

				throw CreateSqlException(0, "This failed the first time");
			}

			// Act
			var a = () => s.RetryAsync(ActionAsync, source.Token);

			// Assert
			await a.Should()
				.ThrowAsync<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}

		[Theory]
		[ClassData(typeof(RetryCountErrorNumberCrossProduct))]
		public async Task RetryAsync_WithReturn_Fails_AfterCorrectRetries(int retryCount, int errorNumber)
		{
			// Arrange
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(retryCount);
			Task<int> ActionAsync() => throw CreateSqlException(errorNumber, $"This failed.  Finally: {s.RetryCount}");

			// Act
			var a = () => s.RetryAsync(ActionAsync);

			// Assert
			await a.Should().ThrowAsync<SqlException>().WithMessage($"This failed.  Finally: {retryCount}");
			s.RetryCount.Should().Be(retryCount + 1);
		}

		[Theory]
		[ClassData(typeof(ErrorNumbers))]
		public async Task RetryAsync_WithReturn_Succeeds_AfterFailing(int errorNumber)
		{
			// Arrange
			var outer = 0;
			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);
			Task<int> ActionAsync()
			{
				if (++outer <= 1)
					throw CreateSqlException(errorNumber);
				return Task.FromResult(1000);
			}

			// Act
			var result = await s.RetryAsync(ActionAsync);

			// Assert
			s.RetryCount.Should().Be(1);
			outer.Should().Be(2);
			result.Should().Be(1000);
		}

		[Theory]
		[ClassData(typeof(ErrorNumbers))]
		public async Task RetryAsync_WithReturnCancellationTest(int errorNumber)
		{
			// Arrange
			var outer = 0;
			var source = new CancellationTokenSource();

			var s = new SqlServerRetryStrategy();
			s.SetMaxRetryCount(2);
			Task<int> ActionAsync()
			{
				if (++outer <= 1)
				{
					source.Cancel();
				}
				throw CreateSqlException(errorNumber, "This failed the first time");
			}

			// Act
			var a = () => s.RetryAsync(ActionAsync, source.Token);

			// Assert
			await a.Should().ThrowAsync<OperationCanceledException>();
			s.RetryCount.Should().Be(1);
		}
		#endregion

		#region TestData

		private static readonly List<int> RetryCountList = new() { 0, 1, 5, 10 };
		private static readonly List<int> ErrorNumberList = new() { -2, 11, 1205, 11001 };

		private class RetryCounts : TheoryData<int>
		{
			public RetryCounts()
			{
				RetryCountList.ForEach(Add);
			}
		}

		private class ErrorNumbers : TheoryData<int>
		{
			public ErrorNumbers()
			{
				ErrorNumberList.ForEach(Add);
			}
		}

		private class RetryCountErrorNumberCrossProduct : TheoryData<int, int>
		{
			public RetryCountErrorNumberCrossProduct()
			{
				ErrorNumberList.ForEach(errNo => RetryCountList.ForEach(r => Add(r, errNo)));
			}
		}
		#endregion
	}
}

