using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Moq;
using Xunit;
using MnM.Common.Data.Dapper.UnitTests.TestClasses;
using System.Data.OracleClient;
using System.Diagnostics.CodeAnalysis;
using System.Data.Odbc;
using System.Data.OleDb;
using FluentAssertions;
using Xunit.Categories;

namespace MnM.Common.Data.Dapper.UnitTests
{
	[ExcludeFromCodeCoverage]
	[UnitTest]
	public class ParameterManagerTests
	{
		[Fact]
		public void AddDbParameterWithNullTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			
			// Act
			mgr.AddDbParameter(null);

			// Assert
			mgr.DbParameters.Count.Should().Be(0);
		}


		[Fact]
		public void AddDbParameterTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			var param = new Mock<IDataParameter>().Object;

			// Act
			mgr.AddDbParameter(param);

			// Assert
			mgr.DbParameters.Count.Should().Be(1);
			mgr.NamedParameters.Count.Should().Be(0);
		}

		[Fact]
		public void AddNamedParameterWithNullTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(null, CrudMethod.None);

			// Assert
			mgr.NamedParameters.Count.Should().Be(0);
		}

		[Fact]
		public void AddNamedParameterTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePoco{ Param1 = 1, Param2 = "2"}, CrudMethod.None);
			var p1 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param1");
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(2);
			p1.Value.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithIgnoreTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithIgnore { Param1 = 1, Param2 = "2" }, CrudMethod.None);
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithIgnoreOnInsertWithInsertTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithIgnoreOnInsert { Param1 = 1, Param2 = "2" }, CrudMethod.Insert);
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithIgnoreOnInsertWithUpdateTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithIgnoreOnInsert { Param1 = 1, Param2 = "2" }, CrudMethod.Update);
			var p1 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param1");
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(2);
			p1.Value.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithIgnoreOnUpdateWithUpdateTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithIgnoreOnUpdate { Param1 = 1, Param2 = "2" }, CrudMethod.Update);
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithIgnoreOnUpdateWithInsertTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithIgnoreOnUpdate { Param1 = 1, Param2 = "2" }, CrudMethod.Insert);
			var p1 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param1");
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(2);
			p1.Value.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithColumnMappingTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithColumnMapping { Param1 = 1, Param2 = "2" }, CrudMethod.None);
			var p1 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Column1");
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(2);
			p1.Value.Should().Be(1);
			p2.Value.Should().Be("2");
		}

		[Fact]
		public void AddNamedParameterWithNullValueTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePocoWithNull { Param1 = 1, Param2 = DBNull.Value }, CrudMethod.None);
			var p1 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param1");
			var p2 = mgr.NamedParameters.FirstOrDefault(p => p.Key == "Param2");

			// Assert
			mgr.NamedParameters.Count.Should().Be(2);
			p1.Value.Should().Be(1);
			p2.Value.Should().BeNull();
		}
		
		[Fact]
		public void ClearTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			mgr.AddNamedParameters(new SimplePoco(), CrudMethod.None);
			mgr.AddDbParameter(new Mock<IDataParameter>().Object);
			var namedBeforeCount = mgr.NamedParameters.Count;
			var dbBeforeCount = mgr.DbParameters.Count;
			mgr.Clear();

			// Assert
			namedBeforeCount.Should().Be(2);
			dbBeforeCount.Should().Be(1);
			mgr.NamedParameters.Count.Should().Be(0);
			mgr.DbParameters.Count.Should().Be(0);
		}

		[Fact]
		public void BuildDapperParametersNoParametersTest()
		{
			// Arrange
			var mgr = new ParameterManager();

			// Act
			var dapper = mgr.BuildDapperParameters();

			// Assert
			dapper.Should().NotBeNull();
			dapper.ParameterNames.Should().BeEmpty();
		}

		[Fact]
		public void BuildDapperParametersDbOnlyTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			mgr.AddDbParameter(new SqlParameter() { ParameterName = "Param", DbType = DbType.AnsiString, Value = "test" });

			// Act
			var dapper = mgr.BuildDapperParameters();

			// Assert
			dapper.Should().NotBeNull();
			dapper.ParameterNames.Should().ContainSingle();
		}

		[Fact]
		public void BuildDapperParametersNamedOnlyTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			mgr.AddNamedParameters(new SimplePoco(), CrudMethod.None);

			// Act
			var dapper = mgr.BuildDapperParameters();

			// Assert
			dapper.Should().NotBeNull();
			dapper.ParameterNames.Should().HaveCount(2);
		}

		[Fact]
		public void BuildDapperParametersTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			mgr.AddNamedParameters(new SimplePoco(), CrudMethod.None);
			mgr.AddDbParameter(new SqlParameter() { ParameterName = "Param", DbType = DbType.AnsiString, Value = "test" });

			// Act
			var dapper = mgr.BuildDapperParameters();

			// Assert
			dapper.Should().NotBeNull();
			dapper.ParameterNames.Should().HaveCount(3);
		}

		[Fact]
		public void ExtractOutputParametersWithNoParametersTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			var @params = mgr.BuildDapperParameters();

			// Act
			var a = () => mgr.ExtractOutputParameters(@params);

			// Assert
			a.Should().NotThrow<Exception>();
		}

		[Fact]
		public void ExtractOutputParametersWithNullMappedParametersTest()
		{
			// Arrange
			var mgr = new ParameterManager();
			var @params = mgr.BuildDapperParameters();

			// Act
			var a = () => mgr.ExtractOutputParameters(@params);

			// Assert
			a.Should().NotThrow<Exception>();
		}

		[Theory]
		[ClassData(typeof(SqlParameterTestData))]
		public void SqlExtractOutputParametersTest(string paramName, DbType dbType, Type valueType, object value, ParameterDirection direction, object expected)
		{
			ExtractTest<SqlParameter>(paramName, dbType, valueType, value, direction, expected);
		}

		[Theory]
		[ClassData(typeof(SqlParameterExtractFailureTestData))]
		public void SqlExtractOutputParametersExceptionTest(string paramName, DbType dbType, ParameterDirection direction, object value, Type exceptionType)
		{
			ExtractExceptionTest<SqlParameter>(paramName, dbType, direction, value, exceptionType);
		}

		[Theory]
		[ClassData(typeof(OracleParameterTestData))]
		public void OracleExtractOutputParametersTest(string paramName, DbType dbType, Type valueType, object value, ParameterDirection direction, object expected)
		{
			ExtractTest<OracleParameter>(paramName, dbType, valueType, value, direction, expected);
		}

		[Theory]
		[ClassData(typeof(OracleParameterExtractFailureTestData))]
		public void OracleExtractOutputParametersExceptionTest(string paramName, DbType dbType, ParameterDirection direction, object value, Type exceptionType)
		{
			ExtractExceptionTest<OracleParameter>(paramName, dbType, direction, value, exceptionType);
		}

		[Theory]
		[ClassData(typeof(OdbcParameterTestData))]
		public void OdbcExtractOutputParametersTest(string paramName, DbType dbType, Type valueType, object value, ParameterDirection direction, object expected)
		{
			ExtractTest<OdbcParameter>(paramName, dbType, valueType, value, direction, expected);
		}

		[Theory]
		[ClassData(typeof(OdbcParameterExtractFailureTestData))]
		public void OdbcExtractOutputParametersExceptionTest(string paramName, DbType dbType, ParameterDirection direction, object value, Type exceptionType)
		{
			ExtractExceptionTest<OdbcParameter>(paramName, dbType, direction, value, exceptionType);
		}

		private static void ExtractTest<T>(string paramName, DbType dbType, Type valueType, object value,
			ParameterDirection direction, object expected) where T : IDbDataParameter, new()
		{
			// Arrange
			var param = new T { ParameterName = paramName, DbType = dbType, Value = value, Direction = direction };
			var mgr = new ParameterManager();
			mgr.AddDbParameter(param);
			var @params = mgr.BuildDapperParameters();
			param.Value = null; // clear value so extract can be tested

			// Act
			mgr.ExtractOutputParameters(@params);

			// Assert
			param.Value.Should().BeOfType(valueType);
			Convert.ChangeType(param.Value, valueType).Should().Be(Convert.ChangeType(expected, valueType));
		}

		private static void ExtractExceptionTest<T>(string paramName, DbType dbType, ParameterDirection direction, object value, Type exceptionType) where T: IDataParameter, new()
		{
			// Arrange
			var param = new T { ParameterName = paramName, DbType = dbType, Direction = direction, Value = value };
			var mgr = new ParameterManager();
			mgr.AddDbParameter(param);
			var @params = mgr.BuildDapperParameters();
			param.Value = null;

			// Act
			var ex = Record.Exception(() => mgr.ExtractOutputParameters(@params));

			// Assert
			ex.Should().NotBeNull().And.BeOfType(exceptionType);
		}
	}
}
