using System;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.Dapper.DependencyInjection.UnitTests
{
	[UnitTest]
	public class RepositoryConfigurationOptionsTests
	{
		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange & Act
			var options = new RepositoryConfigurationOptions<IDbClient>();

			// Assert
			options.Should().NotBeNull();
			options.CaseSensitiveColumnMapping.Should().BeFalse();
			options.ClientFactory.Should().NotBeNull();
			var a = () => options.ClientFactory();
			a.Should().Throw<NotImplementedException>();
		}

		[Theory]
		[MemberData(nameof(GetterSetterData))]
		public void SettersAndGetters_Succeed(bool caseMatching, Func<IDbClient> func)
		{
			// Arrange & Act
			var options = new RepositoryConfigurationOptions<IDbClient>
			{
				CaseSensitiveColumnMapping = caseMatching,
				ClientFactory = func
			};

			// Assert
			options.Should().NotBeNull();
			options.CaseSensitiveColumnMapping.Should().Be(caseMatching);
			options.ClientFactory.Should().NotBeNull();
		}

		public static TheoryData<bool, Func<IDbClient>> GetterSetterData => new()
		{
			{ true, Mock.Of<IDbClient> },
			{ false, Mock.Of<IDbClient> }
		};

	}
}