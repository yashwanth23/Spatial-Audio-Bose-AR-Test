using System;

namespace Bose.Wearable
{
	/// <summary>
	/// The ProductId of a hardware device.
	/// </summary>
	[Serializable]
	internal enum ProductId : ushort
	{
		Undefined = 0,
		Frames = 0x402C,
		Frames2 = 0x404C,
		QuietComfort35Two = 0x4020,
		NoiseCancellingHeadphones700 = 0x4024
	}
}
