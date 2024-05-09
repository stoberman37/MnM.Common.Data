//using System;
//using System.Data;
//using System.Diagnostics.CodeAnalysis;
//using Moq;
//using Xunit;
//using MnM.Common.Data;

//namespace MnM.Common.Data.Dapper.UnitTests
//{
//	[ExcludeFromCodeCoverage]
//	[Trait("Category", "CI")]
//	// ReSharper disable once InconsistentNaming
//	public class IDbConnectionExensionsTests
//	{
//		[Fact]
//		public void EnsureOpenWhenClosedTest()
//		{
//			// Arrange
//			var conn = new Mock<IDbConnection>();
//			conn.SetupGet(c => c.State).Returns(ConnectionState.Closed);

//			// Act
//			conn.Object.EnsureOpen();

//			// Assert
//			conn.Verify(c => c.Open(), Times.Once);
//		}

//		[Fact]
//		public void EnsureOpenWhenOpenTest()
//		{
//			// Arrange
//			var conn = new Mock<IDbConnection>();
//			conn.SetupGet(c => c.State).Returns(ConnectionState.Open);

//			// Act
//			conn.Object.EnsureOpen();

//			// Assert
//			conn.Verify(c => c.Open(), Times.Never);
//		}

//		[Fact]
//		public void EnsureClosedWhenClosedTest()
//		{
//			// Arrange
//			var conn = new Mock<IDbConnection>();
//			conn.SetupGet(c => c.State).Returns(ConnectionState.Closed);

//			// Act
//			conn.Object.EnsureClosed();

//			// Assert
//			conn.Verify(c => c.Close(), Times.Never);
//		}

//		[Fact]
//		public void EnsureClosedWhenOpenTest()
//		{
//			// Arrange
//			var conn = new Mock<IDbConnection>();
//			conn.SetupGet(c => c.State).Returns(ConnectionState.Open);

//			// Act
//			conn.Object.EnsureClosed();

//			// Assert
//			conn.Verify(c => c.Close(), Times.Once);
//		}


//		[Fact]
//		public void EnsureOpenNullTest()
//		{
//			// Arrange & Act
//			var ex = Record.Exception(() => (null as IDbConnection).EnsureOpen());

//			// Assert
//			Assert.NotNull(ex);
//			Assert.IsType<ArgumentNullException>(ex);
//			Assert.Equal("connection", (ex as ArgumentNullException)?.ParamName);
//		}

//		[Fact]

//		public void EnsureClosedNullTest()
//		{
//			// Arrange & Act
//			var ex = Record.Exception(() => (null as IDbConnection).EnsureClosed());

//			// Assert
//			Assert.NotNull(ex);
//			Assert.IsType<ArgumentNullException>(ex);
//			Assert.Equal("connection", (ex as ArgumentNullException)?.ParamName);
//		}
//	}
//}