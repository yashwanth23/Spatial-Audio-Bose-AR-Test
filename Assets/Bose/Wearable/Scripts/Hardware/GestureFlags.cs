using System;

namespace Bose.Wearable
{
	/// <summary>
	/// A bitfield that represents which sensors are available on a device.  These values are from
	/// the GATT spec, except low by one bit, because the original lowest-place gesture
	/// was not implemented. (This shift-by-one occurs in the associated bridges.) 
	/// </summary>
	[Flags]
	[Serializable]
	public enum GestureFlags
	{
		None = 0,

		// Device-specific gestures
		DoubleTap = 1 << 0,
		HeadNod = 1 << 1,
		HeadShake = 1 << 2,
		TouchAndHold = 1 << 3,

		// Device-agnostic gestures start at bit 15.  Every device with up-to-date firmware is
		// guaranteed to support all of the agnostic gestures but they may map to different
		// device-specific gestures.
		Input = 1 << 15,
		Affirmative = 1 << 16,
		Negative = 1 << 17
	}
}
