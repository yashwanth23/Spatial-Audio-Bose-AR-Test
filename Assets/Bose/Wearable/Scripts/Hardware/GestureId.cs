using System;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents a specific gesture type with an associated value.  The associated values reflect the values
	/// for each GestureId in the underlying SDK.
	/// </summary>
	[Serializable]
	public enum GestureId : byte
	{
		None = 0x0,

		// Device-specific gesture IDs are for internal use.  Apps should use device-agnostic gestures, below.
		DoubleTap = 0x81,
		HeadNod = 0x82,
		HeadShake = 0x83,
		TouchAndHold = 0x84,

		// Device-agnostic gesture IDs start at 0xc0 (192).
		Input = 0xc0,
		Affirmative = 0xc1,
		Negative = 0xc2
	}
}
