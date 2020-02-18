using System;
using System.Text;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Stores a device's opperational state, and provides properties for interpreting it.
	/// </summary>
	[Serializable]
	public struct DeviceStatus
	{
		/// <summary>
		/// The device has indicated that pairing is enabled.
		/// </summary>
		public bool PairingEnabled
		{
			get { return GetFlagValue(DeviceStatusFlags.PairingEnabled); }
		}

		/// <summary>
		/// The device has specified that secure pairing is required to connect.
		/// </summary>
		public bool SecurePairingRequired
		{
			get { return GetFlagValue(DeviceStatusFlags.SecurePairingRequired); }
		}

		/// <summary>
		/// The device has indicated that it is already paired to a client.
		/// </summary>
		public bool AlreadyPairedToClient
		{
			get { return GetFlagValue(DeviceStatusFlags.AlreadyPairedToClient); }
		}

		/// <summary>
		/// The SDK has indicated that the sensor service is suspended.
		/// </summary>
		public bool ServiceSuspended
		{
			get { return GetFlagValue(DeviceStatusFlags.SensorServiceSuspended); }
		}

		/// <summary>
		/// Extract the <see cref="GetServiceSuspendedReason"/> from the device status.
		/// </summary>
		/// <returns></returns>
		public SensorServiceSuspendedReason GetServiceSuspendedReason()
		{
			// The top 4 bits of the status form the service suspended reason
			SensorServiceSuspendedReason reason = (SensorServiceSuspendedReason)(_value & SERVICE_SUSPENSION_REASON_MASK);

			// Older versions of the firmware do not populate this field; treat this case as "unknown"
			if (reason == 0)
			{
				reason = SensorServiceSuspendedReason.UnknownReason;
			}

			return reason;
		}

		[SerializeField]
		private int _value;

		private const int SERVICE_SUSPENSION_REASON_MASK = 0xF000;

		public static implicit operator int(DeviceStatus deviceStatus)
		{
			return deviceStatus._value;
		}

		public static implicit operator DeviceStatus(int value)
		{
			return new DeviceStatus{_value = value};
		}

		/// <summary>
		/// Given the previous device status, return a new psuedo-status consisting of only the
		/// flags that were set since the last status.
		/// </summary>
		/// <param name="lastStatus"></param>
		/// <returns></returns>
		internal DeviceStatus GetRisingEdges(DeviceStatus lastStatus)
		{
			return new DeviceStatus{ _value = _value & ~lastStatus._value};
		}

		/// <summary>
		/// Given the previous device status, return a new psuedo-status consisting of only the
		/// flags that were cleared since the last status.
		/// </summary>
		/// <param name="lastStatus"></param>
		/// <returns></returns>
		internal DeviceStatus GetFallingEdges(DeviceStatus lastStatus)
		{
			return new DeviceStatus{ _value = ~_value & lastStatus._value};
		}

		/// <summary>
		/// Return the boolean value of a specified flag.
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		internal bool GetFlagValue(DeviceStatusFlags flag)
		{
			return (_value & (int)flag) != 0;
		}

		/// <summary>
		/// Set the boolean value of a specified flag.
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="state"></param>
		internal void SetFlagValue(DeviceStatusFlags flag, bool state)
		{
			if (state)
			{
				_value |= (int)flag;
			}
			else
			{
				_value &= ~(int)flag;
			}
		}

		/// <summary>
		/// Set the status's "service suspension reason" bits.
		/// </summary>
		/// <param name="reason"></param>
		internal void SetServiceSuspendedReason(SensorServiceSuspendedReason reason)
		{
			_value = (_value & ~SERVICE_SUSPENSION_REASON_MASK) | (int) reason;
		}
	}
}
