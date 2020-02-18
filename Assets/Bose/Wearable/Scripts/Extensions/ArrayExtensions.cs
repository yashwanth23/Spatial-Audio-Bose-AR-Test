namespace Bose.Wearable.Extensions
{
	/// <summary>
	/// Helper methods for arrays
	/// </summary>
	internal static class ArrayExtensions
	{
		/// <summary>
		/// Returns true if <paramref name="array"/> contains <paramref name="value"/>, otherwise false.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool Contains<T>(this T[] array, T value)
		{
			if (array == null || array.Length == 0)
			{
				return false;
			}

			var found = false;
			for (var i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(value))
				{
					found = true;
					break;
				}
			}

			return found;
		}
	}
}
