using System;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the result of a connection attempt made through the <see cref="WearableConnectUIPanel"/>.
	/// </summary>
	[Serializable]
	public enum WearableConnectUIResult
	{
		/// <summary>
		/// The attempt was successful and a device is now connected.
		/// </summary>
		Successful,

		/// <summary>
		/// The user decided to leave the connection UI and not connect to a device or the UI was
		/// programmatically hidden by a developer.
		/// </summary>
		Cancelled
	}
}
