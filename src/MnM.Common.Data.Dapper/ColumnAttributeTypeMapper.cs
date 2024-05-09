using System;
using System.Linq;
using MnM.Common.Data.Attributes;
using Dapper;

namespace MnM.Common.Data.Dapper
{
	/// <summary>
	/// Uses the Name value of the <see cref="ColumnAttribute"/> specified to determine
	/// the association between the name of the column in the query results and the member to
	/// which it will be extracted. If no column mapping is present all members are mapped as
	/// usual.
	/// </summary>
	/// <param name="caseSensitive">set to true to do a case-sensitive string comparison. Default is case-insensitive.</param>
	/// <typeparam name="T">The type of the object that this association between the mapper applies to.</typeparam>
	public class ColumnAttributeTypeMapper<T> : ColumnAttributeTypeMapper
	{
		public ColumnAttributeTypeMapper(bool caseSensitive = false)
			: base(typeof(T), caseSensitive)
		{
		}
	}

	/// <summary>
	/// Uses the Name value of the <see cref="ColumnAttribute"/> specified to determine
	/// the association between the name of the column in the query results and the member to
	/// which it will be extracted. If no column mapping is present all members are mapped as
	/// usual.
	/// </summary>
	/// <param name="type">The type of the object that this association between the mapper applies to.</param>
	/// <param name="caseSensitive">set to true to do a case-sensitive string comparison. Default is case-insensitive.</param>
	public class ColumnAttributeTypeMapper : FallbackTypeMapper
	{
		public ColumnAttributeTypeMapper(Type type, bool caseSensitive = false)
			: base(new SqlMapper.ITypeMap[]
			{
				new CustomPropertyTypeMap(
					type,
					(t, columnName) =>
						type.GetProperties().FirstOrDefault(prop =>
							prop.GetCustomAttributes(false)
								.OfType<ColumnAttribute>()
								.Any(attr => caseSensitive ? attr.Name == columnName :
									attr.Name.Equals(columnName,  StringComparison.InvariantCultureIgnoreCase))
						)
				),
				new DefaultTypeMap(type)
			})
		{
		}

	}
}