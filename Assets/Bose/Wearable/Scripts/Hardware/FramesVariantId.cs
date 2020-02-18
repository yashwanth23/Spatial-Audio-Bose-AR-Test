using System;

namespace Bose.Wearable
{
	/// <summary>
	/// The VariantId of a Frames hardware device.
	/// </summary>
	[Serializable]
	public enum FramesVariantId : byte
	{
		Undefined = 0,
		Alto = 0x01,
		Rondo = 0x02
	}
}
