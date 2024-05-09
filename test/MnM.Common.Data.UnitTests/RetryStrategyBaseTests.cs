using System;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.UnitTests
{
	[UnitTest]
	public class RetryStrategyBaseTests
	{
		private class NullTestStrategy : RetryStrategyBase<Exception>
		{
			public NullTestStrategy() : base(null)
			{
			}
		}
		private class TestStrategy : RetryStrategyBase<Exception>
		{
			public TestStrategy() : base((s, ex) => true)
			{
			}
		}

		[Fact]
		public void Constructor_Succeeds()
		{
			// Arrange &  Act
			var s = new TestStrategy();


			// Assert
			s.Should().NotBeNull();
		}

		[Fact]
		public void Constructor_ThrowsException_WithNullContinueFunc()
		{
			// Arrange & Act
			var a = () => new NullTestStrategy();

			// Assert
			a.Should()
				.Throw<ArgumentNullException>()
				.WithParameterName("continueFunc");
		}
	}
}