using System;
using System.Runtime.InteropServices;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents an Wearable device.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct Device : IEquatable<Device>
	{
		/// <summary>
		/// The Unique Identifier for this device. On Android this will be the device's address, since
		/// we cannot get the uuid of the device in some circumstances.
		/// </summary>
		public string uid;

		/// <summary>
		/// The name of this device.
		/// </summary>
		public string name;

		/// <summary>
		/// The firmware version of the device.
		/// </summary>
		public string firmwareVersion;

		/// <summary>
		/// A bitfield that contains status of this device.
		/// </summary>
		public DeviceStatus deviceStatus;

		/// <summary>
		/// The RSSI of the device at the time it was first located.
		/// NB: this value is not updated after searching is stopped.
		/// </summary>
		public int rssi;

		/// <summary>
		/// A bitfield listing the available sensor on a device.
		/// </summary>
		public SensorFlags availableSensors;

		/// <summary>
		/// A bitfield listing the available gestures on a device.
		/// </summary>
		public GestureFlags availableGestures;

		/// <summary>
		/// The ProductId of the device.
		/// </summary>
		internal ProductId productId;

		/// <summary>
		/// The VariantId of the device.
		/// </summary>
		internal byte variantId;

		/// <summary>
		/// Indicates how often samples are transmitted from the product over the air, in milliseconds. A special value
		/// of zero indicates that the samples are sent as soon as they are available.
		/// </summary>
		internal int transmissionPeriod;

		/// <summary>
		/// Indicates the maximum payload size in bytes of all combined active sensors that can be sent every transmission
		/// period. The special value of zero indicates that there are no limitations on the sensor payload size.
		/// </summary>
		internal int maximumPayloadPerTransmissionPeriod;

		/// <summary>
		/// Indicates the maximum number of sensors that can be active simultaneously.
		/// </summary>
		internal int maximumActiveSensors;

		/// <summary>
		/// The current Active Noise Reduction Mode of the device.
		/// </summary>
		public ActiveNoiseReductionMode activeNoiseReductionMode;

		/// <summary>
		/// A bitfield listing the available Active Noise Reduction Modes on the device. Use
		/// <see cref="GetAvailableActiveNoiseReductionModes"/> and <see cref="IsActiveNoiseReductionModeAvailable"/> for
		/// methods making use of this field.
		/// </summary>
		public int availableActiveNoiseReductionModes;

		/// <summary>
		/// Returns true if the Active Noise Reduction feature is available, otherwise false.
		/// </summary>
		public bool IsActiveNoiseReductionAvailable
		{
			get
			{
				return WearableTools.GetActiveNoiseReductionModesAsList(availableActiveNoiseReductionModes).Length > 0;
			}
		}

		/// <summary>
		/// The current Controllable Noise Cancellation level of the device. Values range between zero and
		/// (<see cref="totalControllableNoiseCancellationLevels"/> - 1).
		/// </summary>
		public int controllableNoiseCancellationLevel;

		/// <summary>
		/// Whether or not the Controllable Noise Cancellation feature is currently enabled on this device.
		/// </summary>
		public bool controllableNoiseCancellationEnabled;

		/// <summary>
		///The total number of Controllable Noise Cancellation levels on this device. If the feature is not available
		/// on the device, this value will be zero.
		/// </summary>
		public int totalControllableNoiseCancellationLevels;

		/// <summary>
		/// Returns true if the Noise Cancellation feature is available on this device, otherwise false.
		/// </summary>
		public bool IsControllableNoiseCancellationAvailable
		{
			get { return totalControllableNoiseCancellationLevels > 0; }
		}

		/// <summary>
		/// Returns the <see cref="ProductType"/> of the device.
		/// </summary>
		/// <returns></returns>
		public ProductType GetProductType()
		{
			return WearableTools.GetProductType(productId);
		}

		/// <summary>
		/// Returns the Product <see cref="VariantType"/> of the device.
		/// </summary>
		/// <returns></returns>
		public VariantType GetVariantType()
		{
			return WearableTools.GetVariantType(GetProductType(), variantId);
		}

		/// <summary>
		/// Returns true if a device supports using a sensor with <see cref="SensorId"/>
		/// <paramref name="sensorId"/>, otherwise false if it does not.
		/// </summary>
		/// <param name="sensorId"></param>
		/// <returns></returns>
		public bool IsSensorAvailable(SensorId sensorId)
		{
			var sensorFlag = WearableTools.GetSensorFlag(sensorId);
			return (availableSensors & sensorFlag) == sensorFlag;
		}

		/// <summary>
		/// Returns true if a device supports using a gesture with <see cref="GestureId"/>
		/// <paramref name="gestureId"/>, otherwise false if it does not.
		/// </summary>
		/// <param name="gestureId"></param>
		/// <returns></returns>
		public bool IsGestureAvailable(GestureId gestureId)
		{
			var gestureFlag = WearableTools.GetGestureFlag(gestureId);
			return (availableGestures & gestureFlag) == gestureFlag;
		}

		/// <summary>
		/// Returns the device's <see cref="SignalStrength"/> based on its <seealso cref="rssi"/>.
		/// </summary>
		/// <returns></returns>
		public SignalStrength GetSignalStrength()
		{
			var signalStrength = SignalStrength.Weak;
			if (rssi > -35)
			{
				signalStrength = SignalStrength.Full;
			}
			else if (rssi > -45)
			{
				signalStrength = SignalStrength.Strong;
			}
			else if (rssi > -55)
			{
				signalStrength = SignalStrength.Moderate;
			}

			return signalStrength;
		}

		/// <summary>
		/// Get a list of <see cref="ActiveNoiseReductionMode"/>s supported by the device. An empty list will be
		/// returned if the feature is not supported.
		/// </summary>
		/// <returns></returns>
		public ActiveNoiseReductionMode[] GetAvailableActiveNoiseReductionModes()
		{
			return WearableTools.GetActiveNoiseReductionModesAsList(availableActiveNoiseReductionModes);
		}

		/// <summary>
		/// Returns true if a given <see cref="ActiveNoiseReductionMode"/> is available on this device.
		/// </summary>
		/// <param name="mode"></param>
		/// <returns></returns>
		public bool IsActiveNoiseReductionModeAvailable(ActiveNoiseReductionMode mode)
		{
			if (mode == ActiveNoiseReductionMode.Invalid)
			{
				return false;
			}

			// Check if the corresponding bit is set
			return ((1 << (int) mode) & availableActiveNoiseReductionModes) != 0;
		}

		/// <summary>
		/// Copies dynamic info into this device.
		/// </summary>
		internal void SetDynamicInfo(DynamicDeviceInfo dynamicDeviceInfo)
		{
			deviceStatus = dynamicDeviceInfo.deviceStatus;
			transmissionPeriod = dynamicDeviceInfo.transmissionPeriod;
			activeNoiseReductionMode = dynamicDeviceInfo.activeNoiseReductionMode;
			availableActiveNoiseReductionModes = dynamicDeviceInfo.availableActiveNoiseReductionModes;
			controllableNoiseCancellationLevel = dynamicDeviceInfo.controllableNoiseCancellationLevel;
			totalControllableNoiseCancellationLevels = dynamicDeviceInfo.totalControllableNoiseCancellationLevels;
			controllableNoiseCancellationEnabled = dynamicDeviceInfo.controllableNoiseCancellationEnabled;
		}

		#region IEquatable<Device>

		public bool Equals(Device other)
		{
			return uid == other.uid;
		}

		public static bool operator ==(Device lhs, Device rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Device lhs, Device rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			return obj is Device && Equals((Device) obj);
		}

		public override int GetHashCode()
		{
			return (uid != null ? uid.GetHashCode() : 0);
		}

		#endregion
	}
}
