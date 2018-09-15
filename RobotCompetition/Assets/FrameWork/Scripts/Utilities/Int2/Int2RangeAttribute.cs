using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class Int2RangeAttribute : PropertyAttribute
{
	public readonly int Min;
	public readonly int Max;

	public Int2RangeAttribute(int min, int max)
	{
		this.Min = min;
		this.Max = max;
	}
}
