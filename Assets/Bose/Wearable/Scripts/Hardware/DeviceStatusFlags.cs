using System;

namespace Bose.Wearable
{
	/// <summary>
	/// A bitfield that represents the status of a connected device.
	/// </summary>
	[Serializable]
	public enum DeviceStatusFlags
	{
		None = 0,
		PairingEnabled = 1 << 0,
		SecurePairingRequired = 1 << 1,
		AlreadyPairedToClient = 1 << 2,
		SensorServiceSuspended = 1 << 3
	}
}
