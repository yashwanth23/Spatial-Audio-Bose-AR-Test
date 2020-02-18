#if UNITY_EDITOR

using UnityEngine;

namespace Bose.Wearable
{
	internal sealed partial class WearableBluetoothProvider : IWearableBluetoothProviderPlatform
	{
		#region IWearableBluetoothProviderPlatform implementation

		public void WearableDeviceInitialize()
		{
			Debug.LogError(WearableConstants.UNSUPPORTED_PLATFORM_ERROR);
		}

		public void SetDebugLoggingInternal(LogLevel logLevel) { }

		public void StartSearch(AppIntentProfile appIntentProfile, int rssiThreshold) { }
		public void ReconnectToLastSuccessfulDeviceInternal(AppIntentProfile appIntentProfile) { }
		public void CancelDeviceConnectionInternal() { }
		public void StopSearch() { }
		public void OpenSession(string uid) { }
		public void CloseSession() { }

		public DeviceConnectionInfo GetDeviceConnectionInfoInternal()
		{
			return new DeviceConnectionInfo();
		}

		public FirmwareUpdateInformation GetFirmwareUpdateInformationInternal()
		{
			return WearableConstants.DEFAULT_FIRMWARE_UPDATE_INFORMATION;
		}

		public void SelectFirmwareUpdateOptionInternal(int index) { }

		public int GetSessionStatus(ref string errorMessage)
		{
			return (int)SessionStatus.Closed;
		}

		public ConnectionStatus GetConnectionStatus(ref string errorMessage)
		{
			return ConnectionStatus.Connected;
		}

		public void GetDeviceInfo(ref Device device) { }

		public bool CheckPermissionInternal(OSPermission permission) { return true; }
		public bool CheckServiceInternal(OSService service) { return true; }
		public void RequestPermissionInternal(OSPermission permission) { }

		public Device[] GetDiscoveredDevicesInternal()
		{
			return WearableConstants.EMPTY_DEVICE_LIST;
		}

		public void GetLatestSensorUpdatesInternal() { }
		public void GetLatestGestureUpdatesInternal() { }

		public WearableDeviceConfig GetDeviceConfigurationInternal()
		{
			return WearableConstants.DISABLED_DEVICE_CONFIG;
		}

		public void SetDeviceConfigurationInternal(WearableDeviceConfig config) { }

		public ConfigStatus GetSensorConfigStatusInternal()
		{
			return ConfigStatus.Idle;
		}

		public ConfigStatus GetGestureConfigStatusInternal()
		{
			return ConfigStatus.Idle;
		}

		public bool IsAppIntentProfileValid(AppIntentProfile appIntentProfile)
		{
			return true;
		}

		public void SetActiveNoiseReductionModeProvider(ActiveNoiseReductionMode mode) { }
		public void UpdateActiveNoiseReductionInformation() { }
		public void SetControllableNoiseCancellationLevelProvider(int level, bool enabled) { }
		public void UpdateControllableNoiseCancellationInformation() { }
		public bool GetDeviceProductSpecificControlSetFinished() { return false; }

		public DynamicDeviceInfo GetDynamicDeviceInfoInternal()
		{
			return WearableConstants.EMPTY_DYNAMIC_DEVICE_INFO;
		}

		public void SetAppFocusChangedInternal(bool hasFocus)
		{
			// no-op.
		}

		#endregion
	}
}

#endif
