using System;

namespace Bose.Wearable
{
	/// <summary>
	/// An operating-system permission that may be required of the device for the Bose AR SDK to properly
	/// function.
	/// </summary>
	[Flags]
	[Serializable]
	public enum OSPermissionFlags
	{
		None = 0,
		Bluetooth = 1,
		Location = 2
	}
}
