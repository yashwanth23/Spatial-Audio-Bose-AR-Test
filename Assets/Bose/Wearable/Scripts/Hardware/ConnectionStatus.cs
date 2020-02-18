using System;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the current status of a connection task.
	/// </summary>
	[Serializable]
	public enum ConnectionStatus
	{
		/// <summary>
		/// No device is connected, and we are not searching for devices.
		/// </summary>
		Disconnected = 0,

		/// <summary>
		/// Trying to connect to the last successfully connected device. If that device is discovered in a
		/// search a connection attempt will automatically begin, otherwise it will transition to searching.
		/// </summary>
		AutoReconnect = 9,

		/// <summary>
		/// A permission is needed in order for the SDK to properly function which requires a user to grant it.
		/// </summary>
		PermissionRequired = 10,

		/// <summary>
		/// A service is needed in order for the SDK to properly function which requires a user to enable it.
		/// </summary>
		ServiceRequired = 11,

		/// <summary>
		/// All Bose AR SDK requirements for permissions and services have been met.
		/// </summary>
		RequirementsMet = 12,

		/// <summary>
		/// Searching for available devices.
		/// </summary>
		Searching = 1,

		/// <summary>
		/// Attempting to connect to a device.
		/// </summary>
		Connecting = 2,

		/// <summary>
		/// Waiting for secure pairing to complete (which may or may not involve user intervention).
		/// </summary>
		SecurePairingRequired = 3,

		/// <summary>
		/// A firmware update is available; waiting for user to resolve.
		/// </summary>
		FirmwareUpdateAvailable = 4,

		/// <summary>
		/// A firmware update is required to connect; waiting for user to resolve.
		/// </summary>
		FirmwareUpdateRequired = 5,

		/// <summary>
		/// The device is successfully connected.
		/// </summary>
		Connected = 6,

		/// <summary>
		/// The device failed to connect.
		/// </summary>
		Failed = 7,

		/// <summary>
		/// The device connection was cancelled before it could complete.
		/// </summary>
		Cancelled = 8
	}
}
