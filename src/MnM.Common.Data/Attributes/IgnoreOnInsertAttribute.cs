using System;

// ReSharper disable once CheckNamespace
namespace MnM.Common.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreOnInsertAttribute : Attribute
    {
    }
}