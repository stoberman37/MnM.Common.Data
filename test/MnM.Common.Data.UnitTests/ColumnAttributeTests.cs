using System.Diagnostics.CodeAnalysis;
using MnM.Common.Data.Attributes;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

namespace MnM.Common.Data.UnitTests;

[UnitTest]
[ExcludeFromCodeCoverage]
public class ColumnAttributeTests
{
	[Fact]
	public void DefaultConstructorTest()
	{
		// Arrange & Act
		var a = new ColumnAttribute();

		// Assert
		a.Should().NotBeNull();
		a.Name.Should().BeNullOrEmpty();
	}

	[Fact]
	public void ConstructorTest()
	{
		// Arrange & Act
		var a = new ColumnAttribute("column");

		// Assert
		a.Should().NotBeNull();
		a.Name.Should().Be("column");
	}
}
