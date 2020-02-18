using System;

namespace Bose.Wearable
{
	/// <summary>
	/// An operating-system service that may be required to be enabled on the device for the Bose AR SDK
	/// to properly function.
	/// </summary>
	[Flags]
	[Serializable]
	public enum OSServiceFlags
	{
		None = 0,
		Bluetooth = 1,
		LocationServices = 2
	}
}
