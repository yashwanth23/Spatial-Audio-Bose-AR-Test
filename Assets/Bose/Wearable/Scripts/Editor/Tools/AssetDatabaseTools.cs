using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// Helper methods for the <see cref="AssetDatabase"/>.
	/// </summary>
	internal static class AssetDatabaseTools
	{
		/// <summary>
		/// Returns an <see cref="UnityEngine.Object"/> asset with unique identifier matching
		/// <paramref name="guid"/>.
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		public static Object LoadAsset(string guid)
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(guid);
			return AssetDatabase.LoadAssetAtPath<Object>(assetPath);
		}
	}
}
