using System;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// Helper methods for reflection
	/// </summary>
	internal static class ReflectionTools
	{
		public static bool IsObsolete(Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return attributes.Length > 0;
		}
	}
}
