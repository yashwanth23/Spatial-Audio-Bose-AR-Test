using System;

namespace Bose.Wearable
{
	/// <summary>
	/// DynamicDeviceInfo is used to pass a subset of device information that may be updated
	/// throughout a given session.
	/// </summary>
	[Serializable]
	internal struct DynamicDeviceInfo
	{
		/// <summary>
		/// The current status of the device, and any associated information about sensor service suspension.
		/// </summary>
		public DeviceStatus deviceStatus;

		/// <summary>
		/// Indicates (in milliseconds) how often samples are transmitted from the product over the air.
		/// A special value of zero indicates that the samples are sent as soon as they are available.
		/// </summary>
		public int transmissionPeriod;

		/// <summary>
		/// The current active noise reduction mode.
		/// </summary>
		public ActiveNoiseReductionMode activeNoiseReductionMode;

		/// <summary>
		/// A bitfield representing active active noise reduction modes.
		/// </summary>
		public int availableActiveNoiseReductionModes;

		/// <summary>
		/// The current controllable noise cancellation level.
		/// </summary>
		public int controllableNoiseCancellationLevel;

		/// <summary>
		/// Whether or not the controllable noise cancellation feature is enabled.
		/// </summary>
		public bool controllableNoiseCancellationEnabled;

		/// <summary>
		/// The total number of controllable noise cancellation levels available.
		/// </summary>
		public int totalControllableNoiseCancellationLevels;
	}

}
