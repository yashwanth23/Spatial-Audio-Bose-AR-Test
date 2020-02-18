using System;

namespace Bose.Wearable
{
	/// <summary>
	/// The reason that the sensor service was suspended. Formed from the upper bits of the Device Status field.
	/// </summary>
	[Serializable]
	public enum SensorServiceSuspendedReason
	{
		UnknownReason = 0,
		VoiceAssistantInUse = 1 << 12,
		MultipointConnectionActive = 2 << 12,
		MusicSharingActive = 3 << 12,
		OTAFirmwareUpdate = 4 << 12
	}
}
