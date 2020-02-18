using System;
using System.Runtime.InteropServices;

namespace Bose.Wearable
{
	/// <summary>
	/// The time and type of one gesture detected by the device.
	/// </summary>
	[StructLayout(LayoutKind.Sequential), Serializable]
	public struct GestureData
	{
		public float timestamp;
		public GestureId gestureId;
	}
}
