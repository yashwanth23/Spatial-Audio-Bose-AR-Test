using System;
using UnityEngine.Assertions;

namespace Bose.Wearable
{
	/// <summary>
	/// Methods which work with GestureIds.
	/// </summary>
	public static class GestureIdExtensions
	{
		/// <summary>
		/// Device-agnostic gestures start at 0xc0.  Lower values are device-specific.
		/// </summary>
		public static bool IsGestureDeviceSpecific(this GestureId gestureId)
		{
			Assert.AreNotEqual(gestureId, GestureId.None);
			return (int) gestureId < (int) GestureId.Input;
		}

		/// <summary>
		/// Device-agnostic gestures start at 0xc0.  Lower values are device-specific.
		/// </summary>
		public static bool IsGestureDeviceAgnostic(this GestureId gestureId)
		{
			Assert.AreNotEqual(gestureId, GestureId.None);
			return (int)gestureId >= (int) GestureId.Input;
		}

	}
}
