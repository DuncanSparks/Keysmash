using Godot;
using System;

public static class Tools
{

	private static Random rand = new Random();

	// ================================================================

	public static T Choose<T>(params T[] items)
	{
		return items[Tools.rand.Next(items.Length)];
	}
}