using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Categories;
using MnM.Common.Data.Repositories;
using FluentAssertions;
using Moq;

namespace MnM.Common.Data.Dapper.DependencyInjection.UnitTests
{
	[UnitTest]
	[ExcludeFromCodeCoverage]
	public class DependencyInjectionTests
	{
		[Fact]
		public void AddDapperRepository_WithConnectionFactory_Fails_WithNullServiceCollection()
		{
			// Arrange & Act
			var a = () => (null as ServiceCollection).AddDapperRepository<IDbClient, object>(Mock.Of<IDbClient>);

			// Assert
			a.Should().NotBeNull();
			a.Should().Throw<ArgumentNullException>().WithParameterName("this");
		}

		[Fact]
		public void AddDapperRepository_WithClientFactory_Succeeds()
		{
			// Arrange
			var services = new ServiceCollection();

			Func<DbClient<Exception>> clientFactory = () => new DbClient<Exception>(Mock.Of<DbConnection>, Mock.Of<IRetryStrategy>());

			// Act
			services.AddDapperRepository<DbClient<Exception>, object>(clientFactory);
			var provider = services.BuildServiceProvider();
			var repo = provider.GetService<IRepository<DbClient<Exception>, object>>();

			// Assert
			repo.Should().NotBeNull();
		}

		[Fact]
		public void AddDapperRepository_WithClientFactory_Fails_WithNullClientFactory()
		{
			// Arrange
			var services = new ServiceCollection();

			// Act
			var a = () => services.AddDapperRepository<DbClient<Exception>, object>((Func < DbClient < Exception >> )null);

			// Assert
			a.Should().Throw<ArgumentNullException>()
				.WithParameterName("clientFactory");
		}

		[Fact]
		public void AddDapperRepository_WithOptions_Succeeds()
		{
			// Arrange
			var services = new ServiceCollection();
			var options = new RepositoryConfigurationOptions<DbClient<Exception>>()
			{
				CaseSensitiveColumnMapping = true,
				ClientFactory = () => new DbClient<Exception>(Mock.Of<DbConnection>, Mock.Of<IRetryStrategy>())
			};

			// Act
			services.AddDapperRepository<DbClient<Exception>, object>(options);
			var provider = services.BuildServiceProvider();
			var clientFactory = provider.GetService<Func<DbClient<Exception>>>();
			var repo = provider.GetService<IRepository<DbClient<Exception>, object>>();

			// Assert
			repo.Should().NotBeNull();
			clientFactory.Should().NotBeNull();
		}

		[Fact]
		public void AddDapperRepository_WithOptions_Fails_WithNllClientFactory()
		{
			// Arrange
			var services = new ServiceCollection();
			var options = new RepositoryConfigurationOptions<DbClient<Exception>>
			{
				CaseSensitiveColumnMapping = true,
				ClientFactory = null
			};

			// Act
			var a = () => services.AddDapperRepository<DbClient<Exception>, object>(options);

			// Assert
			a.Should().Throw<ArgumentException>()
				.WithParameterName("options");
		}

		[Fact]
		public void AddDapperRepository_WithOptions_Fails_WithNullOptions()
		{
			// Arrange
			var services = new ServiceCollection();
			Func<DbClient<Exception>> clientFactory = () => new DbClient<Exception>(Mock.Of<DbConnection>, Mock.Of<IRetryStrategy>());

			// Act
			var a = () =>
				services.AddDapperRepository<DbClient<Exception>, object>(
					null as RepositoryConfigurationOptions<DbClient<Exception>>);

			// Assert
			a.Should().Throw<ArgumentNullException>()
				.WithParameterName("options");
		}
	}
}