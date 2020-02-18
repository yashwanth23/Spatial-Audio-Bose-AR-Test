using UnityEngine;

namespace Bose.Wearable.Extensions
{
	/// <summary>
	/// <see cref="QuaternionExtensions"/> contains extension methods for <see cref="Quaternion"/>
	/// </summary>
	internal static class QuaternionExtensions
	{
		/// <summary>
		/// Returns a <see cref="Vector3"/> containing the X, Y, and Z components of thi <see cref="Quaternion"/>.
		/// </summary>
		/// <param name="q"></param>
		/// <returns></returns>
		public static Vector3 XYZ(this Quaternion q)
		{
			return new Vector3(q.x, q.y, q.z);
		}
	}
}
