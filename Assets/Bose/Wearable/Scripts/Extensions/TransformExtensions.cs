using UnityEngine;

namespace Bose.Wearable.Extensions
{
	/// <summary>
	/// Helper methods for <see cref="Transform"/>
	/// </summary>
	internal static class TransformExtensions
	{
		/// <summary>
		/// Deletes all child <see cref="GameObject"/>s of this <see cref="Transform"/>.
		/// </summary>
		/// <param name="tr"></param>
		public static void DeleteAllChildren(this Transform tr)
		{
			var childCount = tr.childCount;
			for (var i = childCount - 1; i >= 0; i--)
			{
				Object.Destroy(tr.GetChild(i).gameObject);
			}
		}
	}
}
