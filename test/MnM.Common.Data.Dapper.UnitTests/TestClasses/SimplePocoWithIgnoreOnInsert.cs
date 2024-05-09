using MnM.Common.Data.Attributes;

namespace MnM.Common.Data.Dapper.UnitTests.TestClasses
{
	public class SimplePocoWithIgnoreOnInsert
	{
		[IgnoreOnInsert]
		public int Param1 { get; set; }
		public string Param2 { get; set; }
	}
}