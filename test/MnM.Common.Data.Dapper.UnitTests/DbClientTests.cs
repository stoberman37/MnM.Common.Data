using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Moq;
using Xunit;
using FluentAssertions;
using Xunit.Categories;

namespace MnM.Common.Data.Dapper.UnitTests
{
	[UnitTest]

	public class DbClientTests
	{

		[Fact]
		public void ConstructorWithConnectionFactoryTest()
		{
			// Arrange
			var conn = Mock.Of<DbConnection>();
			var factory = new Mock<Func<DbConnection>>();
			var retryStrategy = Mock.Of<IRetryStrategy>();

			factory.Setup(f => f()).Returns(conn);

			// Act
			var db = new DbClient<Exception>(factory.Object, retryStrategy);

			// Assert
			db.Should().NotBeNull();
			factory.Verify(f => f(), Times.Exactly(0));
		}

		public static TheoryData<Func<DbConnection>, IRetryStrategy, string> NullTestData => new()
		{
			{ null, Mock.Of<IRetryStrategy>(), "connectionFactory" },
			{ Mock.Of<Func<DbConnection>>(), null, "retryStrategy" },
			{ null, null, "connectionFactory" },
		};

		[Theory]
		[MemberData(nameof(NullTestData))]
		public void ConstructorWithIDbConnectionFactoryFailsOnNullTest(Func<DbConnection> func, IRetryStrategy retryStrategy, string missingMember)
		{
			// Arrange & Act
			var a = () => new DbClient<Exception>(func, retryStrategy);

			// Assert
			a.Should().Throw<ArgumentNullException>()
				.WithParameterName(missingMember);
		}

		[Fact]
		public void DisposeTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var factory = new Mock<Func<DbConnection>>();
			var retryStrategy = new Mock<IRetryStrategy>();
			factory.Setup(f => f()).Returns(conn.Object);

			// Act
			var a = () =>
			{
				using var c = new DbClient<Exception>(factory.Object, retryStrategy.Object);
			};

			// Assert
			a.Should().NotThrow<Exception>();
		}

		[Fact]
		public void SetCommandTextTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.SetCommandText("command text");

			// Assert
			c2.Should().BeSameAs(client);
			client.CommandText.Should().Be("command text");
		}

		[Fact]
		public void SetCommandTimeoutTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.SetCommandTimeout(10);

			// Assert
			c2.Should().BeSameAs(client);
			client.CommandTimeout.Should().Be(10);
		}

		[Fact]
		public void SetCommandTypeTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.SetCommandType(CommandType.Text);

			// Assert
			c2.Should().BeSameAs(client);
			client.CommandType.Should().Be(CommandType.Text);
		}

		[Fact]
		public void AddDbParameterTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddDbParameter(new Mock<IDataParameter>().Object);

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.DbParameters.Count.Should().Be(1);
		}

		[Fact]
		public void AddDbParameterNullTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddDbParameter(null);

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.DbParameters.Count.Should().Be(0);
		}

		[Fact]
		public void AddDbParametersTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddDbParameters(new[]{ Mock.Of<IDataParameter>(), Mock.Of<IDataParameter>() });

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.DbParameters.Count.Should().Be(2);
		}

		[Fact]
		public void AddDbParametersNullTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddDbParameters(null);

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.DbParameters.Count.Should().Be(0);
		}

		[Fact]
		public void AddNamedParametersTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddNamedParameters(new { param = "" });

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.NamedParameters.Count.Should().Be(1);
		}

		[Fact]
		public void AddNamedParametersNullTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			var c2 = client.AddNamedParameters(null);

			// Assert
			c2.Should().BeSameAs(client);
			client.ParameterManager.NamedParameters.Count.Should().Be(0);
		}

		[Fact]
		public void PropertySettersAndGettersTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var retryStrategyForClient = new Mock<IRetryStrategy>();
			var retryStrategyForFluentClient = new Mock<IRetryStrategy>();
			var fluentClient = new DbClient<Exception>(() => conn.Object, retryStrategyForFluentClient.Object);
			var client = new DbClient<Exception>(() => conn.Object, retryStrategyForClient.Object);

			// Act
			fluentClient.SetCommandText("test")
				.SetCommandTimeout(1000)
				.SetCommandType(CommandType.TableDirect)
				.SetMaxRetryCount(5);
			client.CommandText = "test";
			client.CommandTimeout = 1000;
			client.CommandType = CommandType.TableDirect;
			client.MaxRetryCount = 5;

			// Assert
			client.CommandText.Should().Be("test");
			client.CommandTimeout.Should().Be(1000);
			client.CommandType.Should().Be(CommandType.TableDirect);
			retryStrategyForClient.Verify(s => s.SetMaxRetryCount(It.IsAny<int>()), Times.Exactly(1));
			fluentClient.CommandText.Should().Be("test");
			fluentClient.CommandTimeout.Should().Be(1000);
			fluentClient.CommandType.Should().Be(CommandType.TableDirect);
			retryStrategyForFluentClient.Verify(s => s.SetMaxRetryCount(It.IsAny<int>()), Times.Exactly(1));
		}

		[Fact]
		public void ResetTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var clientWithDefaults = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);
			client.SetCommandText("test")
				.SetCommandTimeout(1000)
				.SetCommandType(CommandType.TableDirect)
				.SetMaxRetryCount(5);
			client.AddDbParameter(new Mock<IDataParameter>().Object);
			client.AddNamedParameters(new { Test = "test" });

			// Act
			client.Reset();

			// Assert
			client.CommandText.Should().Be(clientWithDefaults.CommandText);
			client.CommandTimeout.Should().Be(clientWithDefaults.CommandTimeout);
			client.CommandType.Should().Be(clientWithDefaults.CommandType);
			client.MaxRetryCount.Should().Be(clientWithDefaults.MaxRetryCount);
			client.ParameterManager.DbParameters.Count.Should().Be(0);
			client.ParameterManager.NamedParameters.Count.Should().Be(0);
		}

		[Fact]
		public void PushCommandTest()
		{
			// Arrange
			var conn = new Mock<DbConnection>();
			var client = new DbClient<Exception>(() => conn.Object, new Mock<IRetryStrategy>().Object);

			// Act
			client.SetCommandText("cmd1").PushCommand();

			// Assert
			client.CommandText.Should().Be(string.Empty);
			client.ParameterManager.DbParameters.Count.Should().Be(0);
			client.ParameterManager.NamedParameters.Count.Should().Be(0);
		}
	}
}
