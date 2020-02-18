#if UNITY_IOS && !UNITY_EDITOR

using System;
using System.Runtime.InteropServices;
using UnityEngine.iOS;

namespace Bose.Wearable
{
	internal sealed partial class WearableBluetoothProvider : IWearableBluetoothProviderPlatform
	{
		#region IWearableBluetoothProviderPlatform implementation

		public void WearableDeviceInitialize()
		{
			var formattedMinimumVersion = WearableConstants.MINIMUM_SUPPORTED_IOS_VERSION.ToString("0.0");
			Version systemVersion = new Version(UnityEngine.iOS.Device.systemVersion);
			Version minimumVersion = new Version(formattedMinimumVersion);
			if (systemVersion.CompareTo(minimumVersion) < 0)
			{
				throw new Exception(
					string.Format(
						WearableConstants.IOS_VERSION_NOT_SUPPPORTED_FORMAT,
						formattedMinimumVersion)
					);
			}

			WearableInitialize();
		}

		public void SetDebugLoggingInternal(LogLevel logLevel)
		{
			WearableSetLogLevel(logLevel);
		}

		public void StartSearch(AppIntentProfile appIntentProfile, int rssiThreshold)
		{
			BridgeAppIntentProfile bridgeAppIntentProfile = CreateBridgeAppIntentProfile(appIntentProfile);
			WearableStartDeviceSearch(bridgeAppIntentProfile, rssiThreshold);
		}

		public void ReconnectToLastSuccessfulDeviceInternal(AppIntentProfile appIntentProfile)
		{
			BridgeAppIntentProfile bridgeAppIntentProfile = CreateBridgeAppIntentProfile(appIntentProfile);
			WearableConnectToLastDevice(bridgeAppIntentProfile);
		}

		public void CancelDeviceConnectionInternal()
		{
			// no-op, due to current native limitations.
		}

		public void StopSearch()
		{
			WearableStopDeviceSearch();
		}

		public void OpenSession(string uid)
		{
			WearableOpenSession(uid);
		}

		public void CloseSession()
		{
			WearableCloseSession();
		}

		public int GetSessionStatus(ref string errorMessage)
		{
			return WearableGetSessionStatus(ref errorMessage);
		}

		public ConnectionStatus GetConnectionStatus(ref string errorMessage)
		{
			return (ConnectionStatus)WearableGetConnectionStatus(ref errorMessage);
		}

		public void GetDeviceInfo(ref Device device)
		{
			WearableGetDeviceAddress(ref device.uid);
			device.productId = (ProductId)WearableGetDeviceProductID();
			device.variantId = (byte)WearableGetDeviceVariantID();
			device.availableSensors = (SensorFlags)WearableGetDeviceAvailableSensors();
			device.availableGestures = (GestureFlags)WearableGetDeviceAvailableGestures();
			string firmware = WearableConstants.DEFAULT_FIRMWARE_VERSION;
			WearableGetDeviceFirmwareVersion(ref firmware);
			device.firmwareVersion = firmware;
			device.deviceStatus = WearableGetDeviceStatus();
			device.transmissionPeriod = WearableGetTransmissionPeriod();
			device.maximumPayloadPerTransmissionPeriod = WearableGetMaximumPayloadPerTransmissionPeriod();
			device.maximumActiveSensors = WearableGetMaximumActiveSensors();
		}

		public bool CheckPermissionInternal(OSPermission permission)
		{
			switch (permission)
			{
				case OSPermission.Bluetooth:
					return WearableCheckBluetoothPermission();
				default:
					return true;
			}
		}

		public bool CheckServiceInternal(OSService service)
		{
			switch (service)
			{
				case OSService.Bluetooth:
					return WearableCheckBluetoothService();
				default:
					return true;
			}
		}

		public void RequestPermissionInternal(OSPermission permission)
		{
			switch (permission)
			{
				case OSPermission.Bluetooth:
					WearableRequestBluetoothPermission();
					break;
				default:
					break;
			}
		}

		public Device[] GetDiscoveredDevicesInternal()
		{
			Device[] devices = WearableConstants.EMPTY_DEVICE_LIST;
			unsafe
			{
				BridgeDevice* nativeDevicesPtr = null;
				int count = 0;
				WearableGetDiscoveredDevices(&nativeDevicesPtr, &count);
				if (count > 0)
				{
					devices = new Device[count];
					for (int i = 0; i < count; i++)
					{
						var bridgeDevice = (BridgeDevice)Marshal.PtrToStructure(new IntPtr(nativeDevicesPtr + i), typeof(BridgeDevice));
						devices[i] = CreateDevice(bridgeDevice);
					}
				}
			}

			return devices;
		}

		public DeviceConnectionInfo GetDeviceConnectionInfoInternal()
		{
			BridgeDeviceConnectionInformation bridgeConnectionInfo = WearableGetDeviceConnectionInformation();
			DeviceConnectionInfo deviceConnectionInfo = new DeviceConnectionInfo();
			deviceConnectionInfo.productId = (ProductId)bridgeConnectionInfo.productId;
			deviceConnectionInfo.variantId = bridgeConnectionInfo.variantId;
			return deviceConnectionInfo;
		}

		public FirmwareUpdateInformation GetFirmwareUpdateInformationInternal()
		{
			BridgeFirmwareUpdateInformation bridgeFirmwareInformation = new BridgeFirmwareUpdateInformation();
			WearableGetFirmwareUpdateInformation(ref bridgeFirmwareInformation);
			FirmwareUpdateInformation firmwareUpdateInformation = new FirmwareUpdateInformation();
			firmwareUpdateInformation.icon = (BoseUpdateIcon)bridgeFirmwareInformation.updateIcon;
			firmwareUpdateInformation.title = bridgeFirmwareInformation.title;
			firmwareUpdateInformation.message = bridgeFirmwareInformation.message;
			firmwareUpdateInformation.options = new FirmwareUpdateAlertOption[bridgeFirmwareInformation.numOptions];

			for (int i = 0; i < bridgeFirmwareInformation.numOptions; i++)
			{
				BridgeFirmwareUpdateAlertOption alertOption = new BridgeFirmwareUpdateAlertOption();
				WearableGetFirmwareUpdateAlertOption(ref alertOption, i);
				firmwareUpdateInformation.options[i].style = (AlertStyle)alertOption.style;
				firmwareUpdateInformation.options[i].title = alertOption.title;
			}

			return firmwareUpdateInformation;
		}

		public void SelectFirmwareUpdateOptionInternal(int index)
		{
			WearableSelectAlertOption(index);
		}

		public void GetLatestSensorUpdatesInternal()
		{
			unsafe
			{
				BridgeSensorFrame* frames = null;
				int count = 0;
				WearableGetSensorFrames(&frames, &count);
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						var frame = frames + i;
						_currentSensorFrames.Add(new SensorFrame
						{
							timestamp = WearableConstants.SENSOR2_UNITY_TIME * frame->timestamp,
							deltaTime = WearableConstants.SENSOR2_UNITY_TIME * frame->deltaTime,
							acceleration = frame->acceleration,
							angularVelocity = frame->angularVelocity,
							rotationNineDof = frame->rotationNineDof,
							rotationSixDof = frame->rotationSixDof,
						});
					}
				}
			}
		}

		public void GetLatestGestureUpdatesInternal()
		{
			unsafe
			{
				BridgeGestureData* gestureData = null;
				int count = 0;
				WearableGetGestureData(&gestureData, &count);
				if (count > 0)
				{
					for (int i = 0; i < count; i++)
					{
						var gesture = gestureData + i;
						_currentGestureData.Add(new GestureData
						{
							timestamp = WearableConstants.SENSOR2_UNITY_TIME * gesture->timestamp,
							gestureId = gesture->gestureId
						});
					}
				}
			}
		}

		public WearableDeviceConfig GetDeviceConfigurationInternal()
		{
			BridgeDeviceConfiguration config = new BridgeDeviceConfiguration();
			WearableGetDeviceConfiguration(ref config);

			return CreateWearableConfig(config);
		}

		public void SetDeviceConfigurationInternal(WearableDeviceConfig config)
		{
			WearableSetDeviceConfiguration(CreateBridgeConfig(config));
		}

		public ConfigStatus GetSensorConfigStatusInternal()
		{
			return (ConfigStatus)WearableDeviceGetSensorConfigurationStatus();
		}

		public ConfigStatus GetGestureConfigStatusInternal()
		{
			return (ConfigStatus)WearableDeviceGetGestureConfigurationStatus();
		}

		public void SetActiveNoiseReductionModeProvider(ActiveNoiseReductionMode mode)
		{
			WearableSetActiveNoiseReductionMode((int)mode);
		}

		public void UpdateActiveNoiseReductionInformation()
		{
			WearableRequestActiveNoiseReductionInformation();
		}

		public void SetControllableNoiseCancellationLevelProvider(int level, bool enabled)
		{
			WearableSetControllableNoiseCancellationLevel(level, enabled);
		}

		public void UpdateControllableNoiseCancellationInformation()
		{
			WearableRequestControllableNoiseCancellationInformation();
		}

		public bool GetDeviceProductSpecificControlSetFinished()
		{
			return WearableDeviceCheckProductControlSetFinished();
		}

		public DynamicDeviceInfo GetDynamicDeviceInfoInternal()
		{
			BridgeDynamicDeviceInfo bridgeDynamicDeviceInfo = new BridgeDynamicDeviceInfo();
			WearableGetDynamicDeviceInfo(ref bridgeDynamicDeviceInfo);

			return CreateDynamicDeviceInfo(bridgeDynamicDeviceInfo);
		}

		public bool IsAppIntentProfileValid(AppIntentProfile appIntentProfile)
		{
			BridgeAppIntentProfile bridgeAppIntentProfile = CreateBridgeAppIntentProfile(appIntentProfile);
			return WearableValidateAppIntents(bridgeAppIntentProfile);
		}

		public void SetAppFocusChangedInternal(bool hasFocus)
		{
			WearableSetAppFocusChanged(hasFocus);
		}

		#endregion

		#region iOS Interop
		/// <summary>
		/// This struct matches the plugin bridge definition and is only used as a temporary convert from the native
		/// code struct to the public struct.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct BridgeDeviceConnectionInformation
		{
			public int productId;
			public byte variantId;
		}

		/// <summary>
		/// This struct matches the plugin bridge definition and is only used as a temporary convert from the native
		/// code struct to the public struct.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private unsafe struct BridgeDevice
		{
			public char* uid;
			public char* name;
			public char* firmwareVersion;
			public bool isConnected;
			public int rssi;
			public SensorFlags availableSensors;
			public GestureFlags availableGestures;
			public int productId;
			public int variantId;
			public int transmissionPeriod;
			public int maximumPayloadPerTransmission;
			public int maximumActiveSensors;
		}

		/// <summary>
		/// This struct matches the plugin bridge definition and is only used as a temporary convert from the native
		/// code struct to the public struct.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeSensorFrame
		{
			public int timestamp;
			public int deltaTime;
			public SensorVector3 acceleration;
			public SensorVector3 angularVelocity;
			public SensorQuaternion rotationNineDof;
			public SensorQuaternion rotationSixDof;
		}

		/// <summary>
		/// This struct matches the plugin bridge definition and is only used as a temporary convert from the native
		/// code struct to the public struct.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeGestureData
		{
			public int timestamp;
			public GestureId gestureId;
		}

		/// <summary>
		/// This struct allows passing a device configuration to the bridge, and acts only as a temporary passthrough.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeDeviceConfiguration
		{
			public int updateInterval;

			public int sensorAccelerometer;
			public int sensorGyroscope;
			public int sensorRotationNineDof;
			public int sensorRotationSixDof;

			public int gestureDoubleTap;
			public int gestureHeadNod;
			public int gestureHeadShake;
			public int gestureTouchAndHold;

			public int gestureInput;
			public int gestureAffirmative;
			public int gestureNegative;
		};

		/// <summary>
		/// Struct that acts as a go-between for getting information about firmware alerts.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeFirmwareUpdateInformation
		{
			public int updateIcon;
			public string title;
			public string message;
			public int numOptions;
		}

		/// <summary>
		/// Struct that acts as a go-between for getting information about firmware alert options.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeFirmwareUpdateAlertOption
		{
			public int style;
			public string title;
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeDynamicDeviceInfo
		{
			public int deviceStatus;
			public int transmissionPeriod;
			public int activeNoiseReductionMode;
			public int availableActiveNoiseReductionModes;
			public int controllableNoiseCancellationLevel;
			public bool controllableNoiseCancellationEnabled;
			public int totalControllableNoiseCancellationLevels;
		}

		/// <summary>
		/// This struct allows passing an AppIntentProfile to the bridge as bitfields instead of arrays.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct BridgeAppIntentProfile
		{
			public int sensors;
			public int samplePeriods;
			public int gestures;
		}

		/// <summary>
		/// Helper function to convert our AppIntentProfile to something more easily consumable
		/// by the bridge layer.
		/// </summary>
		private static BridgeAppIntentProfile CreateBridgeAppIntentProfile(AppIntentProfile appIntentProfile)
		{
			BridgeAppIntentProfile bridgeAppIntentProfile = new BridgeAppIntentProfile();
			int sensors = 0;
			int samplePeriods = 0;
			int gestures = 0;

			if (appIntentProfile != null)
			{
				for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
				{
					if (appIntentProfile.GetSensorInProfile(WearableConstants.SENSOR_IDS[i]))
					{
						sensors |= (1 << i);
					}
				}

				for (int i = 0; i < WearableConstants.UPDATE_INTERVALS.Length; i++)
				{
					if (appIntentProfile.GetIntervalInProfile(WearableConstants.UPDATE_INTERVALS[i]))
					{
						samplePeriods |= (1 << i);
					}
				}

				for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
				{
					if (WearableConstants.GESTURE_IDS[i] == GestureId.None)
					{
						continue;
					}

					if (appIntentProfile.GetGestureInProfile(WearableConstants.GESTURE_IDS[i]))
					{
						gestures |= (1 << (i - 1));
					}
				}
			}

			bridgeAppIntentProfile.sensors = sensors;
			bridgeAppIntentProfile.samplePeriods = samplePeriods;
			bridgeAppIntentProfile.gestures = gestures;

			return bridgeAppIntentProfile;
		}

		/// <summary>
		/// Helper function to convert our WearableDeviceConfig to something more easily consumable
		/// by the bridge layer.
		/// </summary>
		private static BridgeDeviceConfiguration CreateBridgeConfig(WearableDeviceConfig config)
		{
			BridgeDeviceConfiguration bridgeConfig;
			bridgeConfig.updateInterval = (int)config.updateInterval;
			bridgeConfig.sensorAccelerometer = config.accelerometer.isEnabled ? 1 : 0;
			bridgeConfig.sensorGyroscope = config.gyroscope.isEnabled ? 1 : 0;
			bridgeConfig.sensorRotationNineDof = config.rotationNineDof.isEnabled ? 1 : 0;
			bridgeConfig.sensorRotationSixDof = config.rotationSixDof.isEnabled ? 1 : 0;
			bridgeConfig.gestureDoubleTap = config.doubleTapGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureHeadNod = config.headNodGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureHeadShake = config.headShakeGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureTouchAndHold = config.touchAndHoldGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureInput = config.inputGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureAffirmative = config.affirmativeGesture.isEnabled ? 1 : 0;
			bridgeConfig.gestureNegative = config.negativeGesture.isEnabled ? 1 : 0;

			return bridgeConfig;
		}

		/// <summary>
		/// Helper function to convert our WearableDeviceConfig to something more easily consumable
		/// by the bridge layer.
		/// </summary>
		private static WearableDeviceConfig CreateWearableConfig(BridgeDeviceConfiguration config)
		{
			WearableDeviceConfig wearableConfig = new WearableDeviceConfig();
			wearableConfig.updateInterval = (SensorUpdateInterval)config.updateInterval;
			wearableConfig.accelerometer.isEnabled = config.sensorAccelerometer != 0;
			wearableConfig.gyroscope.isEnabled = config.sensorGyroscope != 0;
			wearableConfig.rotationNineDof.isEnabled = config.sensorRotationNineDof != 0;
			wearableConfig.rotationSixDof.isEnabled = config.sensorRotationSixDof != 0;
			wearableConfig.doubleTapGesture.isEnabled = config.gestureDoubleTap != 0;
			wearableConfig.headNodGesture.isEnabled = config.gestureHeadNod != 0;
			wearableConfig.headShakeGesture.isEnabled = config.gestureHeadShake != 0;
			wearableConfig.touchAndHoldGesture.isEnabled = config.gestureTouchAndHold != 0;
			wearableConfig.inputGesture.isEnabled = config.gestureInput != 0;
			wearableConfig.affirmativeGesture.isEnabled = config.gestureAffirmative != 0;
			wearableConfig.negativeGesture.isEnabled = config.gestureNegative != 0;

			return wearableConfig;
		}

		private static DynamicDeviceInfo CreateDynamicDeviceInfo(BridgeDynamicDeviceInfo bridgeDynamicDeviceInfo)
		{
			DynamicDeviceInfo dynamicDeviceInfo = new DynamicDeviceInfo();
			dynamicDeviceInfo.deviceStatus = bridgeDynamicDeviceInfo.deviceStatus;
			dynamicDeviceInfo.transmissionPeriod = bridgeDynamicDeviceInfo.transmissionPeriod;
			dynamicDeviceInfo.activeNoiseReductionMode = (ActiveNoiseReductionMode)bridgeDynamicDeviceInfo.activeNoiseReductionMode;
			dynamicDeviceInfo.availableActiveNoiseReductionModes = bridgeDynamicDeviceInfo.availableActiveNoiseReductionModes;
			dynamicDeviceInfo.controllableNoiseCancellationLevel = bridgeDynamicDeviceInfo.controllableNoiseCancellationLevel;
			dynamicDeviceInfo.controllableNoiseCancellationEnabled = bridgeDynamicDeviceInfo.controllableNoiseCancellationEnabled;
			dynamicDeviceInfo.totalControllableNoiseCancellationLevels = bridgeDynamicDeviceInfo.totalControllableNoiseCancellationLevels;

			return dynamicDeviceInfo;
		}

		private static unsafe Device CreateDevice(BridgeDevice bridgeDevice)
		{
			return new Device
			{
				// NB: The Ansi string marshaling variant is needed here to match the iOS character size.
				// Despite the name, Unicode <i>is</i> supported and properly marshals from the bridge.
				uid = Marshal.PtrToStringAnsi((IntPtr)bridgeDevice.uid) ?? string.Empty,
				name = Marshal.PtrToStringAnsi((IntPtr)bridgeDevice.name) ?? string.Empty,
				firmwareVersion = Marshal.PtrToStringAnsi((IntPtr)bridgeDevice.firmwareVersion) ?? string.Empty,
				rssi = bridgeDevice.rssi,
				availableSensors = bridgeDevice.availableSensors,
				availableGestures = bridgeDevice.availableGestures,
				productId = (ProductId) bridgeDevice.productId,
				variantId = (byte) bridgeDevice.variantId,
				transmissionPeriod = bridgeDevice.transmissionPeriod,
				maximumPayloadPerTransmissionPeriod = bridgeDevice.maximumPayloadPerTransmission,
				maximumActiveSensors = bridgeDevice.maximumActiveSensors
			};
		}

		/// <summary>
		/// Initializes the Wearable SDK and Plugin Bridge;
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableInitialize();

		/// <summary>
		/// Sets the verbosity level of logging coming from the Plugin Bridge.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableSetLogLevel(LogLevel level);

		/// <summary>
		/// Starts the search for available Wearable devices in range whose RSSI is greater than <paramref name="rssiThreshold"/>
		/// </summary>
		/// <param name="bridgeAppIntentProfile"></param>
		/// <param name="rssiThreshold"></param>
		[DllImport("__Internal")]
		private static extern void WearableStartDeviceSearch(BridgeAppIntentProfile bridgeAppIntentProfile, int rssiThreshold);

		/// <summary>
		/// Reconnects to the last successfully connected device with the given intents.
		/// </summary>
		/// <param name="bridgeAppIntentProfile"></param>
		[DllImport("__Internal")]
		private static extern void WearableConnectToLastDevice(BridgeAppIntentProfile bridgeAppIntentProfile);

		/// <summary>
		/// Returns all available Wearable devices.
		/// </summary>
		/// <param name="devices"></param>
		/// <param name="count"></param>
		[DllImport("__Internal")]
		private static extern unsafe void WearableGetDiscoveredDevices(BridgeDevice** devices, int* count);

		/// <summary>
		/// Stops searching for available Wearable devices in range.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableStopDeviceSearch();

		/// <summary>
		/// Attempts to open a session with a specific Wearable Device by way of <paramref name="deviceUid"/>.
		/// </summary>
		/// <param name="deviceUid"></param>
		[DllImport("__Internal")]
		private static extern void WearableOpenSession(string deviceUid);

		/// <summary>
		/// Assesses the SessionStatus of the currently opened session for a specific Wearable device. If there has
		/// been an error, <paramref name="errorMsg"/> will be populated with the text contents of the error message.
		/// </summary>
		/// <param name="errorMsg"></param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetSessionStatus(ref string errorMsg);

		/// <summary>
		/// Assesses the ConnectionStatus of the currently opened session for a specific Wearable device. If there has
		/// been an error, <paramref name="errorMsg"/> will be populated with the text contents of the error message.
		/// </summary>
		/// <param name="errorMsg"></param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetConnectionStatus(ref string errorMsg);

		[DllImport("__Internal")]
		private static extern void WearableCloseSession();

		/// <summary>
		/// Grabs the deviceStatus of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetDeviceStatus();

		/// <summary>
		/// Grabs necessary information to identify a device before we connect.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern BridgeDeviceConnectionInformation WearableGetDeviceConnectionInformation();

		/// <summary>
		/// Grabs necessary information to present a firmware update screen to the user.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableGetFirmwareUpdateInformation(ref BridgeFirmwareUpdateInformation updateInformation);

		/// <summary>
		/// Gets information about an individual alert option in a firmware update screen.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableGetFirmwareUpdateAlertOption(ref BridgeFirmwareUpdateAlertOption alertOption, int index);

		/// <summary>
		/// Conveys to the native sdk that the user has selected an option to deal with out-of-date firmware.
		/// </summary>
		/// <param name="index">The index of the button that was selected.</param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableSelectAlertOption(int index);

		/// <summary>
		/// Returns all unread BridgeSensorFrames from the Wearable Device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern unsafe void WearableGetSensorFrames(BridgeSensorFrame** sensorFrames, int* count);

		/// <summary>
		/// Returns all unread BridgeGestureData from the Wearable Device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern unsafe void WearableGetGestureData(BridgeGestureData** sensorFrames, int* count);


		/// <summary>
		/// Enables and disables all sensors, gestures, rotation source, and update interval.
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableSetDeviceConfiguration(BridgeDeviceConfiguration config);

		/// <summary>
		/// Returns the current device configuration of all sensors, gestures, rotation source, and update interval.
		/// </summary>
		/// <returns>BridgeDeviceConfiguration</returns>
		[DllImport("__Internal")]
		private static extern void WearableGetDeviceConfiguration(ref BridgeDeviceConfiguration config);

		/// <summary>
		/// Returns the UID of a device.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableGetDeviceAddress(ref string uid);

		/// <summary>
		/// Returns the ProductId of a device. This will default to 0 if there is not an open session yet. The ProductId of a device is only available once a session has been opened.
		/// </summary>
		[DllImport("__Internal")]
		private static extern int WearableGetDeviceProductID();

		/// <summary>
		/// Returns the VariantId of a device. This will default to 0 if there is not an open session yet. The VariantId of a device is only available once a session has been opened.
		/// </summary>
		[DllImport("__Internal")]
		private static extern int WearableGetDeviceVariantID();

		/// <summary>
		/// Returns the sensors available on a device. This will default to 0 if there is not an open session yet. The available sensors of a device is only available once a session has been opened.
		/// </summary>
		[DllImport("__Internal")]
		private static extern int WearableGetDeviceAvailableSensors();

		/// <summary>
		/// Returns the gestures available on a device. This will default to 0 if there is not an open session yet. The available gestures of a device is only available once a session has been opened.
		/// </summary>
		[DllImport("__Internal")]
		private static extern int WearableGetDeviceAvailableGestures();

		/// <summary>
		/// Returns the Firmware Version of a device. This will default to an empty string if there is not an open session yet.
		/// The Firmware Version of a device is only available once a session has been opened.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableGetDeviceFirmwareVersion(ref string version);

		/// <summary>
		/// Fetches the sensor configuration status of the bridge.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableDeviceGetSensorConfigurationStatus();

		/// <summary>
		/// Fetches the gesture configuration status of the bridge.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableDeviceGetGestureConfigurationStatus();

		/// <summary>
		/// Grabs the transmissionPeriod of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetTransmissionPeriod();

		/// <summary>
		/// Grabs the maximumPayloadPerTransmissionPeriod of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetMaximumPayloadPerTransmissionPeriod();

		/// <summary>
		/// Grabs the maximumActiveSensors of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern int WearableGetMaximumActiveSensors();

		/// <summary>
		/// Set the active noise reduction mode on a wearable device that supports it.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableSetActiveNoiseReductionMode(int mode);

		/// <summary>
		/// Create an active noise reduction info request of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableRequestActiveNoiseReductionInformation();

		/// <summary>
		/// Sets the level and enabledness of CNC for the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableSetControllableNoiseCancellationLevel(int level, bool enabled);

		/// <summary>
		/// Create a controllable noise cancellation info request of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableRequestControllableNoiseCancellationInformation();

		/// <summary>
		/// Check the status of a WearableSetActiveNoiseReductionMode or WearableSetControllableNoiseCancellationLevel call.
		/// </summary>
		/// <returns>Whether or not the previous Set call finished.</returns>
		[DllImport("__Internal")]
		private static extern bool WearableDeviceCheckProductControlSetFinished();

		/// <summary>
		/// Grabs the dynamicDeviceInfo of the currently opened session for a Wearable device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern void WearableGetDynamicDeviceInfo(ref BridgeDynamicDeviceInfo dynamicDeviceInfo);

		/// <summary>
		/// Validates an app intent against the connected device.
		/// </summary>
		/// <returns></returns>
		[DllImport("__Internal")]
		private static extern bool WearableValidateAppIntents(BridgeAppIntentProfile bridgeAppIntentProfile);

		/// <summary>
		/// Notifies the underlying SDK that the application focus has changed.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableSetAppFocusChanged(bool hasFocus);

		/// <summary>
		/// Checks with an iOS 13+ device to make sure bluetooth permission was granted.
		/// </summary>
		[DllImport("__Internal")]
		private static extern bool WearableCheckBluetoothPermission();

		/// <summary>
		/// Checks with the OS to make sure bluetooth is available.
		/// </summary>
		[DllImport("__Internal")]
		private static extern bool WearableCheckBluetoothService();

		/// <summary>
		/// Opens the settings app so the user can give us permission.
		/// </summary>
		[DllImport("__Internal")]
		private static extern void WearableRequestBluetoothPermission();

		#endregion
	}
}

#endif
