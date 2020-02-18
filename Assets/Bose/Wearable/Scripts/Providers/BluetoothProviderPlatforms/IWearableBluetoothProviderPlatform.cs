
namespace Bose.Wearable
{
	internal interface IWearableBluetoothProviderPlatform
	{
		void WearableDeviceInitialize();

		void SetDebugLoggingInternal(LogLevel logLevel);

		void StartSearch(AppIntentProfile appIntentProfile, int rssiThreshold);
		void ReconnectToLastSuccessfulDeviceInternal(AppIntentProfile appIntentProfile);
		void CancelDeviceConnectionInternal();
		void StopSearch();
		void OpenSession(string uid);
		void CloseSession();
		DeviceConnectionInfo GetDeviceConnectionInfoInternal();
		FirmwareUpdateInformation GetFirmwareUpdateInformationInternal();
		void SelectFirmwareUpdateOptionInternal(int index);
		int GetSessionStatus(ref string errorMessage);
		ConnectionStatus GetConnectionStatus(ref string errorMessage);
		void GetDeviceInfo(ref Device device);
		bool CheckPermissionInternal(OSPermission permission);
		bool CheckServiceInternal(OSService service);
		void RequestPermissionInternal(OSPermission permission);

		Device[] GetDiscoveredDevicesInternal();
		void GetLatestSensorUpdatesInternal();
		void GetLatestGestureUpdatesInternal();

		WearableDeviceConfig GetDeviceConfigurationInternal();
		void SetDeviceConfigurationInternal(WearableDeviceConfig config);

		bool IsAppIntentProfileValid(AppIntentProfile appIntentProfile);

		ConfigStatus GetSensorConfigStatusInternal();
		ConfigStatus GetGestureConfigStatusInternal();

		void SetActiveNoiseReductionModeProvider(ActiveNoiseReductionMode mode);
		void UpdateActiveNoiseReductionInformation();
		void SetControllableNoiseCancellationLevelProvider(int level, bool enabled);
		void UpdateControllableNoiseCancellationInformation();
		bool GetDeviceProductSpecificControlSetFinished();

		DynamicDeviceInfo GetDynamicDeviceInfoInternal();

		void SetAppFocusChangedInternal(bool hasChanged);
	}
}
