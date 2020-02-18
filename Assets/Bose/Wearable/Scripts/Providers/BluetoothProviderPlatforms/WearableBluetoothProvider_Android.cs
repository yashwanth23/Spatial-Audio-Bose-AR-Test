#if UNITY_ANDROID && !UNITY_EDITOR

using System;
using UnityEngine;

namespace Bose.Wearable
{
	internal sealed partial class WearableBluetoothProvider : IWearableBluetoothProviderPlatform
	{
		private BoseWearableAndroid AndroidPlugin
		{
			get
			{
				if (_androidPlugin == null)
				{
					_androidPlugin = new BoseWearableAndroid();
				}

				return _androidPlugin;
			}
		}

		private BoseWearableAndroid _androidPlugin;

		#region IWearableBluetoothProviderPlatform implementation

		public void WearableDeviceInitialize()
		{
			AndroidPlugin.Init();
		}

		public void SetDebugLoggingInternal(LogLevel logLevel)
		{
			AndroidPlugin.SetDebugLoggingInternal(logLevel > LogLevel.Error);
		}

		public void StartSearch(AppIntentProfile appIntentProfile, int rssiThreshold)
		{
			int sensors = GetSensorsFromAppIntentProfile(appIntentProfile);
			int samplePeriods = GetSamplePeriodsFromAppIntentProfile(appIntentProfile);
			int gestures = GetGesturesFromAppIntentProfile(appIntentProfile);

			AndroidPlugin.Scan(sensors, samplePeriods, gestures, rssiThreshold);
		}

		public void ReconnectToLastSuccessfulDeviceInternal(AppIntentProfile appIntentProfile)
		{
			int sensors = GetSensorsFromAppIntentProfile(appIntentProfile);
			int samplePeriods = GetSamplePeriodsFromAppIntentProfile(appIntentProfile);
			int gestures = GetGesturesFromAppIntentProfile(appIntentProfile);

			AndroidPlugin.ReconnectToLastSuccessfulDevice(sensors, samplePeriods, gestures);
		}

		public void CancelDeviceConnectionInternal()
		{
			AndroidPlugin.CancelConnection();
		}

		public void StopSearch()
		{
			AndroidPlugin.StopScan();
		}

		public void OpenSession(string uid)
		{
			AndroidPlugin.StartSession(uid);
		}

		public void CloseSession()
		{
			AndroidPlugin.CloseSession();
		}

		public DeviceConnectionInfo GetDeviceConnectionInfoInternal()
		{
			DeviceConnectionInfo deviceConnectionInfo = new DeviceConnectionInfo();
			deviceConnectionInfo.productId = AndroidPlugin.GetDeviceProductId();
			deviceConnectionInfo.variantId = AndroidPlugin.GetDeviceVariantId();
			return deviceConnectionInfo;
		}

		public FirmwareUpdateInformation GetFirmwareUpdateInformationInternal()
		{
			const string GetUpdateIconMethod = "GetAppIcon";

			// These strings really should be grabbed from the bridge, but we don't have
			//		access to them, so we might as well as save on the transport costs.
			const string FirmwareUpdateButtonTitle = "Update Firmware";
			const string FirmwareContinueButtonTitle = "Continue";
			const string FirmwareUpdateTitle = "Firmware Update";
			const string FirmwareUpdateMessage = "New firmware available.";

			FirmwareUpdateAlertOption okay = new FirmwareUpdateAlertOption();
			okay.style = AlertStyle.Affirmative;
			okay.title = FirmwareUpdateButtonTitle;

			FirmwareUpdateAlertOption cancel = new FirmwareUpdateAlertOption();
			cancel.style = AlertStyle.Negative;
			cancel.title = FirmwareContinueButtonTitle;

			AndroidJavaObject firmwareObject = AndroidPlugin.GetFirmwareUpdateInformation();

			FirmwareUpdateInformation firmwareUpdateInfo = new FirmwareUpdateInformation();
			firmwareUpdateInfo.icon = (BoseUpdateIcon)firmwareObject.Call<int>(GetUpdateIconMethod);
			firmwareUpdateInfo.title = FirmwareUpdateTitle;
			firmwareUpdateInfo.message = FirmwareUpdateMessage;
			firmwareUpdateInfo.options = new FirmwareUpdateAlertOption[] { okay, cancel };

			return firmwareUpdateInfo;
		}

		public void SelectFirmwareUpdateOptionInternal(int index)
		{
			const string GetUpdateUrlMethod = "GetUpdateUrl";

			AndroidPlugin.SelectFirmwareUpdateOption(index);

			if (index == 0)
			{
				AndroidJavaObject firmwareObject = AndroidPlugin.GetFirmwareUpdateInformation();
				Application.OpenURL(firmwareObject.Call<string>(GetUpdateUrlMethod));
			}
		}

		public int GetSessionStatus(ref string errorMessage)
		{
			int result = (int)AndroidPlugin.GetSessionStatus();
			errorMessage = AndroidPlugin.GetSessionStatusError();
			return result;
		}

		public ConnectionStatus GetConnectionStatus(ref string errorMessage)
		{
			ConnectionStatus status = (ConnectionStatus) AndroidPlugin.GetConnectionStatus();
			errorMessage = AndroidPlugin.GetSessionStatusError();
			return status;
		}

		public void GetDeviceInfo(ref Device device)
		{
			if (_autoReconnectWithoutPrompts)
			{
				device.uid = AndroidPlugin.GetDeviceAddress();
			}
			device.productId = AndroidPlugin.GetDeviceProductId();
			device.variantId = AndroidPlugin.GetDeviceVariantId();
			device.availableSensors = (SensorFlags)AndroidPlugin.GetAvailableSensors();
			device.availableGestures = (GestureFlags)AndroidPlugin.GetAvailableGestures();
			device.firmwareVersion = AndroidPlugin.GetDeviceFirmwareVersion();
			device.deviceStatus = AndroidPlugin.GetDeviceStatus();
			device.transmissionPeriod = AndroidPlugin.GetTransmissionPeriod();
			device.maximumPayloadPerTransmissionPeriod = AndroidPlugin.GetMaximumPayloadPerTransmissionPeriod();
			device.maximumActiveSensors = AndroidPlugin.GetMaximumActiveSensors();
		}

		public bool CheckPermissionInternal(OSPermission permission)
		{
			switch (permission)
			{
				case OSPermission.Location:
					return AndroidPlugin.GetPermissionGranted(permission);
				default:
					return true;
			}
		}

		public bool CheckServiceInternal(OSService service)
		{
			switch (service)
			{
				case OSService.Bluetooth:
				case OSService.LocationServices:
					return AndroidPlugin.GetServiceAvailable(service);
				default:
					return true;
			}
		}

		public void RequestPermissionInternal(OSPermission permission)
		{
			switch (permission)
			{
				case OSPermission.Location:
					AndroidPlugin.RequestPermission(permission);
					break;
				default:
					break;
			}
		}

		public void SetActiveNoiseReductionModeProvider(ActiveNoiseReductionMode mode)
		{
			AndroidPlugin.SetActiveNoiseReductionMode((int)mode);
		}

		public void UpdateActiveNoiseReductionInformation() { }

		public void SetControllableNoiseCancellationLevelProvider(int level, bool enabled)
		{
			AndroidPlugin.SetControllableNoiseCancellationLevel(level, enabled);
		}

		public void UpdateControllableNoiseCancellationInformation() { }

		public bool GetDeviceProductSpecificControlSetFinished() { return true; }

		public DynamicDeviceInfo GetDynamicDeviceInfoInternal()
		{
			return AndroidPlugin.GetDynamicDeviceInfo(_supportsActiveNoiseReduction, _supportsControllableNoiseCancellation);
		}

		public Device[] GetDiscoveredDevicesInternal()
		{
			return AndroidPlugin.GetDevices();
		}

		public void GetLatestSensorUpdatesInternal()
		{
			const string GetLengthMethod = "length";
			const string GetFrameAtIndexMethod = "getFrameAtIndex";
			const string GetAccelerationMethod = "getAcceleration";
			const string GetAngularVelocityMethod = "getAngularVelocity";
			const string GetRotationSixDofMethod = "getRotationSixDof";
			const string GetRotationNineDofMethod = "getRotationNineDof";

			const string GetWMethod = "getW";
			const string GetXMethod = "getX";
			const string GetYMethod = "getY";
			const string GetZMethod = "getZ";
			const string GetAccuracyMethod = "getAccuracyValue";
			const string GetUncertaintyMethod = "getAccuracy";

			const string GetTimestampMethod = "getTimestamp";
			const string GetDeltaTimeMethod = "getDeltaTime";

			AndroidJavaObject androidObj = AndroidPlugin.GetFrames();
			int count = androidObj.Call<int>(GetLengthMethod);

			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AndroidJavaObject frame = androidObj.Call<AndroidJavaObject>(GetFrameAtIndexMethod, i);

					AndroidJavaObject accelValue = frame.Call<AndroidJavaObject>(GetAccelerationMethod);
					AndroidJavaObject angVelValue = frame.Call<AndroidJavaObject>(GetAngularVelocityMethod);
					AndroidJavaObject rotSixDofValue = frame.Call<AndroidJavaObject>(GetRotationSixDofMethod);
					AndroidJavaObject rotNineDofValue = frame.Call<AndroidJavaObject>(GetRotationNineDofMethod);

					SensorVector3 accel = new SensorVector3();
					SensorVector3 gyro = new SensorVector3();
					SensorQuaternion rotSixDof = new SensorQuaternion();
					SensorQuaternion rotNineDof = new SensorQuaternion();

					accel.value = new Vector3(
						(float) accelValue.Call<double>(GetXMethod),
						(float) accelValue.Call<double>(GetYMethod),
						(float) accelValue.Call<double>(GetZMethod)
					);
					accel.accuracy = (SensorAccuracy) accelValue.Call<sbyte>(GetAccuracyMethod);

					gyro.value = new Vector3(
						(float) angVelValue.Call<double>(GetXMethod),
						(float) angVelValue.Call<double>(GetYMethod),
						(float) angVelValue.Call<double>(GetZMethod)
					);
					gyro.accuracy = (SensorAccuracy) angVelValue.Call<sbyte>(GetAccuracyMethod);

					rotSixDof.value = new Quaternion(
						(float) rotSixDofValue.Call<double>(GetXMethod),
						(float) rotSixDofValue.Call<double>(GetYMethod),
						(float) rotSixDofValue.Call<double>(GetZMethod),
						(float) rotSixDofValue.Call<double>(GetWMethod)
					);
					rotSixDof.measurementUncertainty = (float) rotSixDofValue.Call<double>(GetUncertaintyMethod);

					rotNineDof.value = new Quaternion(
						(float)rotNineDofValue.Call<double>(GetXMethod),
						(float)rotNineDofValue.Call<double>(GetYMethod),
						(float)rotNineDofValue.Call<double>(GetZMethod),
						(float)rotNineDofValue.Call<double>(GetWMethod)
					);
					rotNineDof.measurementUncertainty = (float)rotNineDofValue.Call<double>(GetUncertaintyMethod);


					_currentSensorFrames.Add(
						new SensorFrame
						{
							timestamp = WearableConstants.SENSOR2_UNITY_TIME * frame.Call<int>(GetTimestampMethod),
							deltaTime = WearableConstants.SENSOR2_UNITY_TIME * frame.Call<int>(GetDeltaTimeMethod),
							acceleration = accel,
							angularVelocity = gyro,
							rotationSixDof = rotSixDof,
							rotationNineDof = rotNineDof
						}
					);
				}
			}
		}

		public void GetLatestGestureUpdatesInternal()
		{
			const string GetLengthMethod = "length";
			const string GetDataAtIndexMethod = "getDataAtIndex";
			const string GetGestureMethod = "getGesture";
			const string GetTimestampMethod = "getTimestamp";

			AndroidJavaObject androidObj = AndroidPlugin.GetGestureData();
			int count = androidObj.Call<int>(GetLengthMethod);

			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					AndroidJavaObject gestureData = androidObj.Call<AndroidJavaObject>(GetDataAtIndexMethod, i);

					_currentGestureData.Add(new GestureData
					{
						timestamp = WearableConstants.SENSOR2_UNITY_TIME * gestureData.Call<int>(GetTimestampMethod),
						gestureId = (GestureId)gestureData.Call<int>(GetGestureMethod)
					});
				}
			}
		}

		public WearableDeviceConfig GetDeviceConfigurationInternal()
		{
			return AndroidPlugin.GetDeviceConfiguration();
		}

		public void SetDeviceConfigurationInternal(WearableDeviceConfig config)
		{
			AndroidPlugin.SetDeviceConfiguration(config);
		}

		public ConfigStatus GetSensorConfigStatusInternal()
		{
			return AndroidPlugin.GetSensorConfigStatus();
		}

		public ConfigStatus GetGestureConfigStatusInternal()
		{
			return AndroidPlugin.GetGestureConfigStatus();
		}

		public bool IsAppIntentProfileValid(AppIntentProfile appIntentProfile)
		{
			int sensors = GetSensorsFromAppIntentProfile(appIntentProfile);
			int samplePeriods = GetSamplePeriodsFromAppIntentProfile(appIntentProfile);
			int gestures = GetGesturesFromAppIntentProfile(appIntentProfile);

			return AndroidPlugin.ValidateAppIntents(sensors, samplePeriods, gestures);
		}

		public void SetAppFocusChangedInternal(bool hasFocus)
		{
			AndroidPlugin.SetAppFocusChanged(hasFocus);
		}

		#endregion

		#region Helper Functions

		private int GetSensorsFromAppIntentProfile(AppIntentProfile profile)
		{
			int sensors = 0;
			if (profile != null)
			{
				for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
				{
					if (profile.GetSensorInProfile(WearableConstants.SENSOR_IDS[i]))
					{
						sensors |= (1 << i);
					}
				}
			}

			return sensors;
		}

		private int GetSamplePeriodsFromAppIntentProfile(AppIntentProfile profile)
		{
			int samplePeriods = 0;
			if (profile != null)
			{
				for (int i = 0; i < WearableConstants.UPDATE_INTERVALS.Length; i++)
				{
					if (profile.GetIntervalInProfile(WearableConstants.UPDATE_INTERVALS[i]))
					{
						samplePeriods |= (1 << i);
					}
				}
			}

			return samplePeriods;
		}

		private int GetGesturesFromAppIntentProfile(AppIntentProfile profile)
		{
			int gestures = 0;
			if (profile != null)
			{
				for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
				{
					if (WearableConstants.GESTURE_IDS[i] == GestureId.None)
					{
						continue;
					}

					if (profile.GetGestureInProfile(WearableConstants.GESTURE_IDS[i]))
					{
						gestures |= (1 << (i - 1));
					}
				}
			}

			return gestures;
		}

		#endregion

		private class BoseWearableAndroid
		{
			private const string PackageName = "unity.bose.com.wearableplugin.WearablePlugin";

			private const string EnableSensorMethod = "WearableEnableSensor";
			private const string DisableSensorMethod = "WearableDisableSensor";
			private const string EnableGestureMethod = "WearableSetGestureEnabled";
			private const string SetDeviceConfigurationMethod = "WearableSetDeviceConfiguration";
			private const string GetDeviceConfigurationMethod = "WearableGetDeviceConfiguration";
			private AndroidJavaObject _wearablePlugin;

			/// <summary>
			/// Initialize the Android Native environment.
			///   Note: This will result in an UnsupportedOperationException in Java if below
			///   the minimum Supported API. <see cref="WearableConstants.MINIMUM_SUPPORTED_ANDROID_VERSION"/>
			/// </summary>
			public void Init()
			{
				const string GetInstanceMethod = "GetInstance";
				const string InitializeMethod = "WearableInitialize";

				if (_wearablePlugin == null)
				{
					AndroidJavaClass wearablePluginClass = new AndroidJavaClass(PackageName);
					_wearablePlugin = wearablePluginClass.CallStatic<AndroidJavaObject>(GetInstanceMethod);
				}

				_wearablePlugin.Call(InitializeMethod, GetActivity(), GetContext());
			}

			public void SetDebugLoggingInternal(bool enabled)
			{
				const string SetDebugLogLevelMethod = "WearableSetLoggingEnabled";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(SetDebugLogLevelMethod, enabled);
				}
			}

			public void Scan(int sensorsInIntent, int samplePeriodsInIntent, int gesturesInIntent, int threshold)
			{
				const string StartSearchMethod = "WearableStartDeviceSearch";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(StartSearchMethod, sensorsInIntent, samplePeriodsInIntent, gesturesInIntent, threshold);
				}
			}

			public Device[] GetDevices()
			{
				const string GetDiscoveredDevicesMethod = "WearableGetDiscoveredDevices";
				const string GetDeviceMethod = "getDevice";
				const string GetDeviceCountMethod = "getDeviceCount";
				const string GetAddressMethod = "getAddress";
				const string GetNameMethod = "getName";
				const string GetRssiMethod = "getRSSI";

				Device[] devices = WearableConstants.EMPTY_DEVICE_LIST;

				if (_wearablePlugin != null)
				{
					AndroidJavaObject deviceList = _wearablePlugin.Call<AndroidJavaObject>(GetDiscoveredDevicesMethod);
					int deviceCount = deviceList.Call<int>(GetDeviceCountMethod);

					devices = new Device[deviceCount];

					for (int i = 0; i < deviceCount; i++)
					{
						AndroidJavaObject deviceObj = deviceList.Call<AndroidJavaObject>(GetDeviceMethod, i);

						Device device = new Device
						{
							uid = deviceObj.Call<string>(GetAddressMethod),
							name = deviceObj.Call<string>(GetNameMethod),
							rssi = deviceObj.Call<int>(GetRssiMethod)
						};

						devices[i] = device;
					}
				}

				return devices;
			}

			public void StopScan()
			{
				const string StopSearchMethod = "WearableStopDeviceSearch";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(StopSearchMethod);
				}
			}

			public void ReconnectToLastSuccessfulDevice(int sensorsInIntent, int samplePeriodsInIntent, int gesturesInIntent)
			{
				const string ReconnectMethod = "WearableReconnectToLastDevice";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(ReconnectMethod, sensorsInIntent, samplePeriodsInIntent, gesturesInIntent);
				}
			}

			public void StartSession(string deviceAddress)
			{
				const string OpenSessionMethod = "WearableOpenSession";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(OpenSessionMethod, deviceAddress);
				}
			}

			public SessionStatus GetSessionStatus()
			{
				const string GetStatusMethod = "WearableGetSessionStatus";
				if (_wearablePlugin != null)
				{
					return (SessionStatus) _wearablePlugin.Call<int>(GetStatusMethod);
				}

				return (SessionStatus) 0;
			}

			public string GetSessionStatusError()
			{
				const string GetLastErrorMethod = "WearableGetLastSessionError";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<string>(GetLastErrorMethod);
				}

				return null;
			}

			public int GetConnectionStatus()
			{
				const string GetConnectionStatusMethod = "WearableGetConnectionStatus";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<int>(GetConnectionStatusMethod);
				}

				return 0;
			}

			public bool GetPermissionGranted(OSPermission permission)
			{
				const string GetPermissionGrantedMethod = "WearableGetPermissionGranted";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<bool>(GetPermissionGrantedMethod, (int)permission);
				}

				return false;
			}

			public bool GetServiceAvailable(OSService service)
			{
				const string GetServiceAvailableMethod = "WearableGetServiceAvailable";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<bool>(GetServiceAvailableMethod, (int)service);
				}

				return false;
			}

			public void RequestPermission(OSPermission permission)
			{
				const string RequestLocationPermissionMethod = "WearableRequestPermission";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(RequestLocationPermissionMethod, (int)permission);
				}
			}

			public AndroidJavaObject GetFirmwareUpdateInformation()
			{
				const string GetFirmwareUpdateInformationMethod = "WearableGetFirmwareUpdateInformation";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<AndroidJavaObject>(GetFirmwareUpdateInformationMethod);
				}

				return null;
			}

			public void SelectFirmwareUpdateOption(int index)
			{
				const string SelectFirmwareUpdateOptionMethod = "WearableSelectFirmwareUpdateOption";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(SelectFirmwareUpdateOptionMethod, index);
				}
			}

			public void CancelConnection()
			{
				const string CancelConnectionMethod = "WearableCancelConnection";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(CancelConnectionMethod);
				}
			}

			public void CloseSession()
			{
				const string CloseSessionMethod = "WearableCloseSession";
				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call<bool>(CloseSessionMethod);
				}
			}

			public AndroidJavaObject GetFrames()
			{
				const string GetFramesMethod = "WearableGetSensorFrames";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<AndroidJavaObject>(GetFramesMethod);
				}

				return null;
			}

			public AndroidJavaObject GetGestureData()
			{
				const string GetGestureDataMethod = "WearableGetGestureData";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<AndroidJavaObject>(GetGestureDataMethod);
				}

				return null;
			}

			public void SetDeviceConfiguration(WearableDeviceConfig config)
			{
				if (_wearablePlugin != null)
				{
					bool[] sensors = new bool[WearableConstants.SENSOR_IDS.Length];
					for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
					{
						sensors[i] = config.GetSensorConfig(WearableConstants.SENSOR_IDS[i]).isEnabled;
					}
					IntPtr sensorsJava = AndroidJNIHelper.ConvertToJNIArray(sensors);

					bool[] gestures = new bool[WearableConstants.GESTURE_IDS.Length-1]; // -1 to Exclude .None
					for (int i = 1; i < WearableConstants.GESTURE_IDS.Length; i++)
					{
						gestures[i-1] = config.GetGestureConfig(WearableConstants.GESTURE_IDS[i]).isEnabled;
					}
					IntPtr gesturesJava = AndroidJNIHelper.ConvertToJNIArray(gestures);

					// The AndroidJavaObject.Call method doesn't support arrays, so we have to convert & pass them more deliberately.
					jvalue[] args = new jvalue[4];
					args[0].l = sensorsJava;
					args[1].l = gesturesJava;
					args[2].i = (int)config.updateInterval;
					IntPtr setMethod = AndroidJNIHelper.GetMethodID(_wearablePlugin.GetRawClass(), SetDeviceConfigurationMethod);
					AndroidJNI.CallVoidMethod(_wearablePlugin.GetRawObject(), setMethod, args);
				}
			}

			public WearableDeviceConfig GetDeviceConfiguration()
			{
				const string GetSampleRateMethod = "GetSamplePeriod";
				const string GetSensorEnabledMethod = "GetSensorIsEnabled";
				const string GetGestureEnabledMethod = "GetGestureIsEnabled";

				WearableDeviceConfig config = new WearableDeviceConfig();
				if (_wearablePlugin != null)
				{
					AndroidJavaObject deviceConfig = _wearablePlugin.Call<AndroidJavaObject>(GetDeviceConfigurationMethod);

					config.updateInterval = (SensorUpdateInterval)deviceConfig.Call<int>(GetSampleRateMethod);
					config.accelerometer.isEnabled = deviceConfig.Call<bool>(GetSensorEnabledMethod, (int)SensorId.Accelerometer);
					config.gyroscope.isEnabled = deviceConfig.Call<bool>(GetSensorEnabledMethod, (int)SensorId.Gyroscope);
					config.rotationSixDof.isEnabled = deviceConfig.Call<bool>(GetSensorEnabledMethod, (int)SensorId.RotationSixDof);
					config.rotationNineDof.isEnabled = deviceConfig.Call<bool>(GetSensorEnabledMethod, (int)SensorId.RotationNineDof);
					config.doubleTapGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.DoubleTap);
					config.headNodGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.HeadNod);
					config.headShakeGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.HeadShake);
					config.touchAndHoldGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.TouchAndHold);
					config.inputGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.Input);
					config.affirmativeGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.Affirmative);
					config.negativeGesture.isEnabled = deviceConfig.Call<bool>(GetGestureEnabledMethod, (int)GestureId.Negative);
				}

				return config;
			}

			public ConfigStatus GetSensorConfigStatus()
			{
				const string GetSensorConfigStatusMethod = "WearableGetDeviceSensorConfigurationStatus";

				if (_wearablePlugin != null)
				{
					return (ConfigStatus)_wearablePlugin.Call<int>(GetSensorConfigStatusMethod);
				}

				return ConfigStatus.Idle;
			}

			public ConfigStatus GetGestureConfigStatus()
			{
				const string GetGestureConfigStatusMethod = "WearableGetDeviceGestureConfigurationStatus";

				if (_wearablePlugin != null)
				{
					return (ConfigStatus)_wearablePlugin.Call<int>(GetGestureConfigStatusMethod);
				}

				return ConfigStatus.Idle;
			}

			public string GetDeviceAddress()
			{
				const string GetDeviceAddressMethod = "WearableGetDeviceAddress";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<string>(GetDeviceAddressMethod);
				}

				return "";
			}

			public ProductId GetDeviceProductId()
			{
				const string GetProductIdMethod = "WearableGetDeviceProductID";

				ProductId productId = 0;
				if (_wearablePlugin != null)
				{
					productId = (ProductId) _wearablePlugin.Call<int>(GetProductIdMethod);
				}

				return productId;
			}

			public byte GetDeviceVariantId()
			{
				const string GetVariantIdMethod = "WearableGetDeviceVariantID";

				byte variantId = 0;
				if (_wearablePlugin != null)
				{
					variantId = (byte) _wearablePlugin.Call<int>(GetVariantIdMethod);
				}

				return variantId;
			}

			public int GetAvailableSensors()
			{
				const string GetAvailableSensorsMethod = "WearableGetDeviceAvailableSensors";
				int sensors = 0;

				if (_wearablePlugin != null)
				{
					sensors = _wearablePlugin.Call<int>(GetAvailableSensorsMethod);
				}

				return sensors;
			}

			public int GetAvailableGestures()
			{
				const string GetAvailableGesturesMethod = "WearableGetDeviceAvailableGestures";
				int gestures = 0;

				if (_wearablePlugin != null)
				{
					gestures = _wearablePlugin.Call<int>(GetAvailableGesturesMethod);
				}

				return gestures;
			}

			public string GetDeviceFirmwareVersion()
			{
				const string GetFirmwareVersionIdMethod = "WearableGetDeviceFirmwareVersion";

				string firmwareVersion = WearableConstants.DEFAULT_FIRMWARE_VERSION;
				if (_wearablePlugin != null)
				{
					firmwareVersion = _wearablePlugin.Call<string>(GetFirmwareVersionIdMethod);
				}

				return firmwareVersion;
			}

			public DeviceStatus GetDeviceStatus()
			{
				const string GetDeviceStatusMethod = "WearableGetDeviceStatus";

				DeviceStatus status = 0;
				if (_wearablePlugin != null)
				{
					status = _wearablePlugin.Call<int>(GetDeviceStatusMethod);
				}

				return status;
			}

			public int GetTransmissionPeriod()
			{
				const string GetTransmissionPeriodMethod = "WearableGetTransmissionPeriod";

				int transmissionPeriod = 0;
				if (_wearablePlugin != null)
				{
					transmissionPeriod = _wearablePlugin.Call<int>(GetTransmissionPeriodMethod);
				}

				return transmissionPeriod;
			}

			public int GetMaximumPayloadPerTransmissionPeriod()
			{
				const string GetMaximumPayloadPerTransmissionPeriodMethod
					= "WearableGetMaximumPayloadPerTransmissionPeriod";

				int maximumPayload = 0;
				if (_wearablePlugin != null)
				{
					maximumPayload = _wearablePlugin.Call<int>(GetMaximumPayloadPerTransmissionPeriodMethod);
				}

				return maximumPayload;
			}

			public int GetMaximumActiveSensors()
			{
				const string WearableGetMaximumActiveSensorsMethod
					= "WearableGetMaximumActiveSensors";

				int maximumActiveSensors = 0;
				if (_wearablePlugin != null)
				{
					maximumActiveSensors = _wearablePlugin.Call<int>(WearableGetMaximumActiveSensorsMethod);
				}

				return maximumActiveSensors;
			}

			public void SetActiveNoiseReductionMode(int mode)
			{
				const string WearableSetActiveNoiseReductionModeMethod = "WearableSetDeviceActiveNoiseReductionMode";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(WearableSetActiveNoiseReductionModeMethod, mode);
				}
			}

			public void SetControllableNoiseCancellationLevel(int level, bool enabled)
			{
				const string WearableSetControllableNoiseCancellationMethod = "WearableSetDeviceControllableNoiseCancellationLevel";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(WearableSetControllableNoiseCancellationMethod, level, enabled);
				}
			}

			private int GetCurrentActiveNoiseReductionMode()
			{
				const string WearableGetCurrentActiveNoiseReductionModeMethod = "WearableGetDeviceCurrentActiveNoiseReductionMode";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<int>(WearableGetCurrentActiveNoiseReductionModeMethod);
				}

				return 0;
			}

			private int GetAvailableActiveNoiseReductionModes()
			{
				const string WearableGetAvailableActiveNoiseReductionModeMethod = "WearableGetDeviceAvailableActiveNoiseReductionModes";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<int>(WearableGetAvailableActiveNoiseReductionModeMethod);
				}

				return 0;
			}

			private int GetCurrentControllableNoiseCancellationLevel()
			{
				const string WearableGetCurrentControllableNoiseCancellationLevelMethod = "WearableGetDeviceCurrentControllableNoiseCancellationLevel";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<int>(WearableGetCurrentControllableNoiseCancellationLevelMethod);
				}

				return 0;
			}

			private bool GetControllableNoiseCancellationEnabled()
			{
				const string WearableGetControllableNoiseCancellationEnabledMethod = "WearableGetDeviceControllableNoiseCancellationEnabled";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<bool>(WearableGetControllableNoiseCancellationEnabledMethod);
				}

				return false;
			}

			private int GetTotalControllableNoiseCancellationLevels()
			{
				const string WearableGetTotalControllableNoiseCancellationLevelsMethod = "WearableGetDeviceTotalControllableNoiseCancellationLevels";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<int>(WearableGetTotalControllableNoiseCancellationLevelsMethod);
				}

				return 0;
			}

			public DynamicDeviceInfo GetDynamicDeviceInfo(bool supportsANR, bool supportsCNC)
			{
				DynamicDeviceInfo dynamicDeviceInfo = new DynamicDeviceInfo();
				dynamicDeviceInfo.deviceStatus = GetDeviceStatus();
				dynamicDeviceInfo.transmissionPeriod = GetTransmissionPeriod();
				if (supportsANR)
				{
					dynamicDeviceInfo.activeNoiseReductionMode = (ActiveNoiseReductionMode)GetCurrentActiveNoiseReductionMode();
					dynamicDeviceInfo.availableActiveNoiseReductionModes = GetAvailableActiveNoiseReductionModes();
				}
				if (supportsCNC)
				{
					dynamicDeviceInfo.controllableNoiseCancellationLevel = GetCurrentControllableNoiseCancellationLevel();
					dynamicDeviceInfo.controllableNoiseCancellationEnabled = GetControllableNoiseCancellationEnabled();
					dynamicDeviceInfo.totalControllableNoiseCancellationLevels = GetTotalControllableNoiseCancellationLevels();
				}

				return dynamicDeviceInfo;
			}

			public bool ValidateAppIntents(int sensors, int samplePeriods, int gestures)
			{
				const string ValidateAppIntentsMethod = "WearableValidateAppIntents";

				if (_wearablePlugin != null)
				{
					return _wearablePlugin.Call<bool>(ValidateAppIntentsMethod, sensors, samplePeriods, gestures);
				}

				return false;
			}

			public void SetAppFocusChanged(bool hasFocus)
			{
				const string SetAppFocusChangedMethod = "WearableSetAppFocusChanged";

				if (_wearablePlugin != null)
				{
					_wearablePlugin.Call(SetAppFocusChangedMethod, hasFocus);
				}
			}

			private static AndroidJavaObject GetActivity()
			{
				const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";
				const string CurrentActivityMethod = "currentActivity";

				AndroidJavaClass unityPlayer = new AndroidJavaClass(UnityPlayerClass);
				return unityPlayer.GetStatic<AndroidJavaObject>(CurrentActivityMethod);
			}

			private static AndroidJavaObject GetContext()
			{
				const string GetAppContextMethod = "getApplicationContext";

				AndroidJavaObject activity = GetActivity();
				return activity.Call<AndroidJavaObject>(GetAppContextMethod);
			}
		}
	}
}

#endif
