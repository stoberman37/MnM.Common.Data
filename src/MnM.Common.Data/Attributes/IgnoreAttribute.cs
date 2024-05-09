using System;

namespace MnM.Common.Data.Attributes
{
    // For explicit pocos, marks property as a column and optionally supplies column name
    // For non-explicit pocos, causes a property to be ignored
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}