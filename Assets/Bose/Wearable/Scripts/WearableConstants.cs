using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bose.Wearable
{
	public static class WearableConstants
	{
		public static readonly SensorFrame EMPTY_FRAME;
		public static readonly Device[] EMPTY_DEVICE_LIST;
		public static readonly GestureId[] GESTURE_IDS;
		public static readonly SensorId[] SENSOR_IDS;
		public static readonly SensorUpdateInterval[] UPDATE_INTERVALS;
		public static readonly SignalStrength[] SIGNAL_STRENGTHS;
		public static readonly ActiveNoiseReductionMode[] ACTIVE_NOISE_REDUCTION_MODES;
		public static readonly ActiveNoiseReductionMode[] EMPTY_ACTIVE_NOISE_REDUCTION_MODES;
		public static readonly DeviceStatusFlags[] DEVICE_STATUS_FLAGS;
		public static readonly OSPermission[] OS_PERMISSIONS;
		public static readonly OSService[] OS_SERVICES;

		public const string EMPTY_UID = "00000000-0000-0000-0000-000000000000";

		public const string DEFAULT_FIRMWARE_VERSION = "0.0.0";

		public const SensorUpdateInterval DEFAULT_UPDATE_INTERVAL = SensorUpdateInterval.EightyMs;

		public const SensorFlags ALL_SENSORS = ~SensorFlags.None;
		public const GestureFlags ALL_GESTURES = ~GestureFlags.None;

		public const OSPermissionFlags ALL_OS_PERMISSIONS = ~OSPermissionFlags.None;
		public const OSServiceFlags ALL_OS_SERVICES = ~OSServiceFlags.None;

		/// <summary>
		/// A list of <see cref="ConnectionStatus"/> values where we can cancel a device connection.
		/// </summary>
		public static readonly List<ConnectionStatus> CONNECTING_STATES;

		internal static readonly FirmwareUpdateInformation DEFAULT_FIRMWARE_UPDATE_INFORMATION;

		internal static readonly WearableDeviceConfig DISABLED_DEVICE_CONFIG;

		internal static readonly DeviceStatus EMPTY_DEVICE_STATUS;
		internal static readonly DynamicDeviceInfo EMPTY_DYNAMIC_DEVICE_INFO;

		public const string ENABLED = "Enabled";
		public const string DISABLED = "Disabled";

		/// <summary>
		/// Conversion from sensor timestamps (millis) to Unity time (seconds).
		/// </summary>
		public const float SENSOR2_UNITY_TIME = 0.001f;

		/// <summary>
		/// The default RSSI threshold that devices will be filtered by.
		/// </summary>
		public const int DEFAULT_RSSI_THRESHOLD = -65;

		/// <summary>
		/// The minimum rssi threshold value.
		/// </summary>
		public const int MINIMUM_RSSI_VALUE = -70;

		/// <summary>
		/// The maximum rssi threshold value.
		/// </summary>
		public const int MAXIMUM_RSSI_VALUE = -30;

		public const float DEFAULT_AUTO_RECONNECT_TIMEOUT = 2f;

		public const string PREF_LAST_CONNECTED_DEVICE_UID = "bosear.last_connected_device_udid";

		public const string PREF_PREFERRED_SYSTEM_LANGUAGE = "bosear.preferred_system_language";

		/// <summary>
		/// The number of seconds to wait when the sensor frequency update capability has been locked before
		/// unlocking it.
		/// </summary>
		public const float NUMBER_OF_SECONDS_TO_LOCK_SENSOR_FREQUENCY_UPDATE = 0.33f;

		public const float DEVICE_CONNECT_UPDATE_INTERVAL_IN_SECONDS = 1.0f;
		public const float DEVICE_USB_CONNECT_UPDATE_INTERVAL_IN_SECONDS = 2.0f;
		public const float DEVICE_USB_DYNAMIC_INFO_UPDATE_INTERVAL_IN_SECONDS = 0.2f;
		public const float DEVICE_PROVIDER_DYNAMIC_INFO_UPDATE_INTERVAL_IN_SECONDS = 2.5f;
		public const float DEVICE_SEARCH_UPDATE_INTERVAL_IN_SECONDS = 0.25f;

		// Console Warnings
		public const string UNSUPPORTED_PLATFORM_ERROR = "[Bose Wearable] The active provider does not support this platform.";
		public const string DEVICE_IS_NOT_CURRENTLY_CONNECTED = "[Bose Wearable] A device is not currently connected.";

		public const string DEVICE_CONNECTION_FAILED = "[Bose Wearable] The connection to the device has failed. " +
		                                             "Ending attempt to connect to device.";
		public const string DEVICE_CONNECTION_FAILED_WITH_MESSAGE = "[Bose Wearable] The connection to the device has failed with " +
		                                                        "error message '{0}'. Ending attempt to connect to device.";
		public const string DEVICE_CONNECTION_OPENED = "[Bose Wearable] The connection to the device has been opened.";
		public const string DEVICE_CONNECTION_MONITOR_WARNING = "[Bose Wearable] The connection to the device has ended.";
		public const string DEVICE_CONNECTION_MONITOR_WARNING_WITH_MESSAGE = "[Bose Wearable] The connection to the device has ended " +
		                                                                "with message '{0}'.";
		public const string SENSOR_UPDATE_INTERVAL_DECREASED_WARNING = "[BoseWearable] A SensorUpdateInterval of TwentyMs cannot be used when " +
		                                                           "three or more sensors are enabled; this value has been changed to FourtyMs " +
		                                                           "to prevent a fatal crash.";

		public const string GESTURE_ID_NONE_INVALID_ERROR = "[Bose Wearable] GestureId.None will not return a valid WearableGesture.";

		public const string WEARABLE_GESTURE_NOT_YET_SUPPORTED = "[Bose Wearable] GestureId.{0} is not yet supported and will not return a " +
		                                                     "valid WearableGesture.";

		public const string WEARABLE_SENSOR_NOT_YET_SUPPORTED = "[Bose Wearable] SensorId.{0} is not yet supported and will not return a " +
		                                                    "valid WearableSensor.";

		public const string REQUESTED_GESTURE_NOT_AVAILABLE_WARNING_FORMAT =
			"[Bose Wearable] Requested Gesture [{0}] is not available on the connected device and will not be enabled.";

		public const string REQUESTED_SENSOR_NOT_AVAILABLE_WARNING_FORMAT =
			"[Bose Wearable] Requested Sensor [{0}] is not available on the connected device and will not be enabled.";

		public const string ONLY_ONE_SENSOR_FREQUENCY_UPDATE_PER_FRAME_WARNING =
			"[Bose Wearable] Device state can only be updated once per frame and is locked for several seconds afterward; " +
		    "additional updates after the first will not occur until this has been unlocked.";

		public const string DUPLICATE_WEARABLE_MODEL_PRODUCT_TYPE_WARNING =
			"[Bose Wearable] There is a duplicate WearableModel ProductType " +
			"definition for {0} on this WearableModelLoader";

		public const string DUPLICATE_WEARABLE_MODEL_VARIANT_TYPE_WARNING =
			"[Bose Wearable] There is a duplicate WearableModel VariantType " +
			"definition for {0} on this WearableModelLoader";

		public const string NONE_IS_INVALID_GESTURE = "[Bose Wearable] GestureId.None is an invalid value to " +
		                                            "set on [{0}].";

		public const string INVALID_PROVIDER_TYPE_ERROR = "[Bose Wearable] The provided provider {0} is not valid.";
		public const string CONFIG_FAILED_WARNING = "[Bose Wearable] Failed to configure the connected device. " +
		                                          "Try sending the configuration again. Sensors: {0}; Gestures: {1}";

		public const string CANNOT_MODIFY_SENSOR_FLAGS_WARNING =
			"[Bose Wearable] Cannot modify available sensors while the virtual device is connected.";

		public const string CANNOT_MODIFY_GESTURE_FLAGS_WARNING =
			"[Bose Wearable] Cannot modify available gestures while the virtual device is connected.";

		public const string GESTURE_NOT_DEVICE_AGNOSTIC_WARNING = "[Bose Wearable] Gesture {0} is not device-agnostic.";

		public const string VALIDATING_INTENTS_BUT_NO_PROFILE_WARNING =
			"[Bose Wearable] App Intent validation is enabled, but no intent profile was provided. Skipping validation.";

		public const string VIOLATED_INTENT_PROFILE_WARNING_FORMAT =
			"[Bose Wearable] A sensor, gesture, or update interval was set that is not present in the currently-" +
			"specified app intent profile. While this will not affect app functionality, the profile should be " +
			"edited to include this configuration to fully take advantage of intent validation. The following " +
			"configurations were not specified in the intent profile: {0}";

		public const string INVALID_INTENTS_WARNING =
			"[Bose Wearable] The connected device has reported that the provided intent profile is invalid. Some " +
			"sensors, gestures, or update intervals might not be available on this hardware or firmware version.";

		public const string TOO_MANY_VALIDATION_REQUESTS_ERROR =
			"[Bose Wearable] Additional app intent profiles cannot be validated until the current request has " +
			"completed. Wait for a response then try again.";

		public const string INTENT_CHANGED_WHILE_SEARCHING_WARNING =
			"[Bose Wearable] Changing the app intent while searching for devices will not immediately take affect; " +
			"the new intent profile will be used the next time searching begins.";

		public const string SENSOR_SERVICE_SUSPENDED_WARNING = "[Bose Wearable] The Wearable Sensor Service was suspended, " +
			"and new data will not be available until it resumes. Reason: {0}";

		public const string SENSOR_SERVICE_RESUMED_INFO = "[Bose Wearable] The Wearable Sensor Service has resumed.";

		public const string INVALID_AUTO_RECONNECT_TIMEOUT_PERIOD_WARNING =
			"[Bose Wearable] An auto-reconnect attempt was made with an invalid timeout period of [{0}], " +
			"this value should be positive.";

		public const string INVALID_IS_INVALID_ANR_MODE = "[Bose Wearable] Invalid is not a valid ANR mode.";

		public const string ANR_CNC_WRITE_LOCK_ERROR =
			"[Bose Wearble] The Active Noise Reduction mode or Controllable Noise Cancellation level cannot be set " +
			"until the previous configuration attempt has completed. Note that these writes to either of these " +
			"features are mutually exclusive, and both must wait for either to complete.";

		public const string DEPRECATION_UNITY_2017_WARNING =
			"Support for Unity 2017.X has been deprecated and will no longer be supported. To ensure compatibility with" +
			"future updates, please upgrade your project to the latest of Unity 2018.4 LTS.";

		// Rotation Matcher Messages
		public const string ROTATION_MATCHER_ADDED_REQUIREMENT_MESSAGE_FORMAT =
			"[Bose Wearable] A WearableRequirement has been automatically added by the Rotation Matcher component " +
			"attached to \"{0}\".";
		public const string ROTATION_MATCHER_CHANGED_REQUIREMENT_SENSORS_WARNING_FORMAT =
			"[Bose Wearable] The Rotation Matcher component has altered the WearableRequirement attached to \"{0}\" " +
			"to enable a required sensor.";
		public const string ROTATION_MATCHER_CHANGED_REQUIREMENT_UPDATE_INTERVAL_WARNING_FORMAT =
			"[Bose Wearable] The Rotation Matcher component has altered the WearableRequirement attached to \"{0}\" " +
			"in order to meet its requested update interval.";

		// Debug Provider Defaults
		public const string DEBUG_PROVIDER_DEFAULT_DEVICE_NAME = "Debug Device";
		internal const ProductId DEBUG_PROVIDER_DEFAULT_PRODUCT_ID = ProductId.Undefined;
		internal const byte DEBUG_PROVIDER_DEFAULT_VARIANT_ID = 0;
		public const int DEBUG_PROVIDER_DEFAULT_RSSI = 0;
		public const string DEBUG_PROVIDER_DEFAULT_UID = EMPTY_UID;
		public const float DEBUG_PROVIDER_DEFAULT_DELAY_TIME = 0.5f;
		public const ActiveNoiseReductionMode DEBUG_PROVIDER_DEFAULT_ANR_MODE = ActiveNoiseReductionMode.Off;
		public static readonly ActiveNoiseReductionMode[] DEBUG_PROVIDER_DEFAULT_AVAILABLE_ANR_MODES;
		public const int DEBUG_PROVIDER_DEFAULT_CNC_LEVEL = 0;
		public const int DEBUG_PROVIDER_DEFAULT_TOTAL_CNC_LEVELS = 11;
		public const bool DEBUG_PROVIDER_DEFAULT_CNC_ENABLED = true;

		// Debug Provider Messages
		public const string DEBUG_PROVIDER_INIT = "[Bose Wearable] Debug provider initialized.";
		public const string DEBUG_PROVIDER_DESTROY = "[Bose Wearable] Debug provider destroyed.";
		public const string DEBUG_PROVIDER_ENABLE = "[Bose Wearable] Debug provider enabled.";
		public const string DEBUG_PROVIDER_DISABLE = "[Bose Wearable] Debug provider disabled.";
		public const string DEBUG_PROVIDER_SEARCHING_FOR_DEVICES = "[Bose Wearable] Debug provider searching for devices...";
		public const string DEBUG_PROVIDER_STOPPED_SEARCHING = "[Bose Wearable] Debug provider stopped searching for devices.";
		public const string DEBUG_PROVIDER_CONNECTING_TO_DEVICE = "[Bose Wearable] Debug provider connecting to virtual device...";
		public const string DEBUG_PROVIDER_CONNECTED_TO_DEVICE = "[Bose Wearable] Debug provider connected to virtual device.";
		public const string DEBUG_PROVIDER_FAILED_TO_CONNECT = "[Bose Wearable] Debug provider failed to connect to the virtual device.";
		public const string DEBUG_PROVIDER_DISCONNECTED_TO_DEVICE = "[Bose Wearable] Debug provider disconnected from virtual device.";
		public const string DEBUG_PROVIDER_START_SENSOR = "[Bose Wearable] Debug provider starting sensor {0}";
		public const string DEBUG_PROVIDER_STOP_SENSOR = "[Bose Wearable] Debug provider stopping sensor {0}";
		public const string DEBUG_PROVIDER_SET_UPDATE_INTERVAL = "[Bose Wearable] Debug provider setting update interval to {0}";
		public const string DEBUG_PROVIDER_INVALID_CONNECTION_WARNING = "[Bose Wearable] Debug provider may only connect to " +
																	"its own virtual device as returned by SearchForDevices().";
		public const string DEBUG_PROVIDER_SIMULATE_DISCONNECT = "[Bose Wearable] Debug provider simulating a disconnected device.";
		public const string DEBUG_PROVIDER_ENABLE_GESTURE = "[Bose Wearable] Debug provider starting gesture {0}";
		public const string DEBUG_PROVIDER_DISABLE_GESTURE = "[Bose Wearable] Debug provider stopping gesture {0}";
		public const string DEBUG_PROVIDER_APP_HAS_GAINED_FOCUS = "[Bose Wearable] Application Focus has been regained.";
		public const string DEBUG_PROVIDER_APP_HAS_LOST_FOCUS = "[Bose Wearable] Application Focus has been lost.";

		public const string DEBUG_PROVIDER_TRIGGER_DISABLED_GESTURE_WARNING =
			"[Bose Wearable] A gesture was triggered that is not currently enabled. This gesture will be ignored.";
		public const string DEBUG_PROVIDER_TRIGGER_GESTURE = "[Bose Wearable] Simulating gesture {0}.";
		public const string DEBUG_PROVIDER_FOUND_DEVICES = "[Bose Wearable] Found virtual device.";
		public const string DEBUG_PROVIDER_CANNOT_MODIFY_WHILE_CONNECTED = "[Bose Wearable] The virtual device's hardware " +
		                                                              "characteristics can't be modified while connected.";
		public const string DEBUG_PROVIDER_INTENT_VALIDATION_REQUESTED = "[Bose Wearable] Debug provider validating intents " +
		                                                             "against the virtual device.";
		public const string DEBUG_PROVIDER_SET_CONFIG_WHILE_SUSPENDED_WARNING =
			"[Bose Wearable] The sensor service is currently suspended; the virtual device will ignore all " +
			"configuration attempts until service is resumed.";
		public const string DEBUG_PROVIDER_FIRMWARE_UPDATE_WARNING =
			"[Bose Wearable] On a device, the relevant Bose app would be opened and the firmware updated. To simulate " +
			"that here, indicate that the firmware version is now valid in WearableControl's inspector and connect to " +
			"the virtual device again.";
		public const string DEBUG_PROVIDER_SKIPPED_OPTIONAL_UPDATE =
			"[Bose Wearable] An optional firmware update was skipped, and connection will proceed as normal.";
		public const string DEBUG_PROVIDER_SKIPPED_REQUIRED_UPDATE =
			"[Bose Wearable] A mandatory firmware update was skipped; connection will fail.";
		public const string DEBUG_PROVIDER_DISCONNECTED_FOR_UPDATE =
			"[Bose Wearable] Cancelling the connection process to allow firmware update.";
		public const string DEBUG_PROVIDER_NO_FIRMWARE_UPDATE_AVAILABLE_ERROR =
			"[Bose Wearable] The current config specifies that the virtual firmware version is insufficient, but no " +
			"firmware updates are available. The connection process will fail, indicating an unsupported device.";

		public const string DEBUG_PROVIDER_FIRMWARE_UPDATE_REQUIRED_INFO =
			"[Bose Wearable] The current config specified that the virtual firmware version is insufficient, but a " +
			"firmware update adding support is available. The connection process will prompt for an update, then " +
			"cancel the connection attempt to allow for a firmware update to take place.";
		public const string DEBUG_PROVIDER_FIRMWARE_SUFFICIENT =
			"[Bose Wearable] Firmware check indicated sufficient support. Connection will continue.";
		public const string DEBUG_PROVIDER_INTENTS_VALID = "[Bose Wearable] The specified app intent profile is valid.";
		public const string DEBUG_PROVIDER_INTENTS_NOT_VALID_WARNING =
			"[Bose Wearable] The specified app intent profile is not valid. Connection will not continue.";
		public const string DEBUG_PROVIDER_FIRMWARE_UPDATE_AVAILABLE = "[Bose Wearable] An optional firmware update is available.";
		public const string DEBUG_PROVIDER_CHECKING_INTENTS = "[Bose Wearable] Validating intents...";
		public const string DEBUG_PROVIDER_NO_INTENTS_SPECIFIED = "[Bose Wearable] ...but none were specified.";
		public const string DEBUG_PROVIDER_START_SECURE_PAIRING = "[Bose Wearable] Requesting secure pairing with device...";
		public const string DEBUG_PROVIDER_SECURE_PAIRING_ACCEPTED = "[Bose Wearable] Secure pairing accepted.";
		public const string DEBUG_PROVIDER_SECURE_PAIRING_REJECTED_WARNING = "[Bose Wearable] Secure pairing rejected.";
		public const string DEBUG_PROVIDER_CANCELLED_CONNECTION = "[Bose Wearable] Debug provider cancelled connection.";
		public const string DEBUG_PROVIDER_CANCELLED_CONNECTION_PROMPTED = "[Bose Wearable] Debug provider cancelled connection in response to user/developer.";
		public const string DEBUG_PROVIDER_SET_CNC_LEVEL_FORMAT = "[Bose Wearable] Debug provider setting CNC level to {0}/{1} ({2}).";
		public const string DEBUG_PROVIDER_SET_ANR_MODE_FORMAT = "[Bose Wearable] Debug Provider setting ANR mode to {0}.";
		public const string DEBUG_PROVIDER_SET_INVALID_ANR_MODE_WARNING =
			"[Bose Wearable] Debug Provider cannot set ANR mode to {0}, as it is not in the set of available modes.";
		public const string DEBUG_PROVIDER_CNC_NOT_ENABLED_WARNING =
			"[Bose Wearable] The CNC feature is not currently enabled on the Debug Provider; this call will be " +
			"ignored. Set the Max CNC Level to a positive number to enable this feature.";
		public const string DEBUG_PROVIDER_ANR_NOT_ENABLED_WARNING =
			"[Bose Wearable] The ANR feature is not currently enabled on the Debug Provider; this call will be " +
			"ignored. Add available ANR modes to enable this feature.";
		public const string DEBUG_PROVIDER_SENSOR_CONFIG_FAILURE_WARNING =
			"[Bose Wearable] The Debug Provider is currently configured to simulate sensor configuration failure. " +
			"The current sensor configuration and update rate will not be changed.";
		public const string DEBUG_PROVIDER_GESTURE_CONFIG_FAILURE_WARNING =
			"[Bose Wearable] The Debug Provider is currently configured to simulate gesture configuration failure. " +
			"The current gesture configuration will not be changed.";

		// Localization messages
		public const string LOCALE_KEY_ALREADY_EXISTS_FORMAT
			= "[Bose Wearable] Key [{0}] already exists in this LocaleData instance and will not be added.";

		public const string LOCALE_LANGUAGE_ALREADY_SUPPORTED_FORMAT
			= "[Bose Wearable] Skipping loading [{0}] as the [{1}] language is already supported by [{2}].";

		public const string LOCALE_KEY_NOT_FOUND_FORMAT
		 = "[Bose Wearable] Key [{0}] was not found for language {1}.";

		public const string LOCALE_KEY_ARGUMENT_NOT_FOUND_FORMAT
			= "[Bose Wearable] Cannot find localized format argument {0} for {1}.";

		// USB provider messages
		public const string USB_PROVIDER_NO_CNC_WARNING = "[Bose Wearable] The Controllable Noise Cancellation feature is not available on the connected device.";
		public const string USB_PROVIDER_ANR_NOT_AVAILABLE_WARNING = "[Bose Wearable] The USB provider does not support Active Noise Reduction.";

		// Runtime Dialogs
		public const string WAIT_FOR_CALIBRATION_MESSAGE = "Please look forward \nand remain still...";

		// Providers
		public const ProviderId EDITOR_DEFAULT_PROVIDER = ProviderId.DebugProvider;
		public static readonly ProviderId[] DISALLOWED_EDITOR_PROVIDERS;

		public const ProviderId RUNTIME_DEFAULT_PROVIDER = ProviderId.BluetoothProvider;
		public static readonly ProviderId[] DISALLOWED_RUNTIME_PROVIDERS;

		// Supported iOS Versions
		public const float MINIMUM_COMPILABLE_IOS_VERSION = 11.0f;
		public const float MINIMUM_SUPPORTED_IOS_VERSION = 12.0f;

		public const string IOS_VERSION_NOT_SUPPPORTED_FORMAT = "[Bose Wearable] Current device does not meet minimum iOS support of iOS {0} or greater.";

		// Supported Android SDK Versions
		public const int MINIMUM_COMPILABLE_ANDROID_VERSION = 21;
		public const int MINIMUM_SUPPORTED_ANDROID_VERSION = 26;

		static WearableConstants()
		{
			// Ensure that empty frame has a valid rotation quaternion
			EMPTY_FRAME = new SensorFrame
			{
				rotationNineDof = new SensorQuaternion {value = Quaternion.identity},
				rotationSixDof = new SensorQuaternion {value = Quaternion.identity}
			};

			CONNECTING_STATES = new List<ConnectionStatus>
			{
				ConnectionStatus.FirmwareUpdateRequired,
				ConnectionStatus.FirmwareUpdateAvailable,
				ConnectionStatus.Connecting,
				ConnectionStatus.AutoReconnect,
				ConnectionStatus.SecurePairingRequired
			};

			EMPTY_DEVICE_LIST = new Device[0];

			GESTURE_IDS = (GestureId[])Enum.GetValues(typeof(GestureId));
			SENSOR_IDS = (SensorId[])Enum.GetValues(typeof(SensorId));
			UPDATE_INTERVALS = (SensorUpdateInterval[])Enum.GetValues(typeof(SensorUpdateInterval));
			SIGNAL_STRENGTHS = (SignalStrength[])Enum.GetValues(typeof(SignalStrength));
			ACTIVE_NOISE_REDUCTION_MODES = (ActiveNoiseReductionMode[]) Enum.GetValues(typeof(ActiveNoiseReductionMode));
			EMPTY_ACTIVE_NOISE_REDUCTION_MODES = new ActiveNoiseReductionMode[0];
			DEVICE_STATUS_FLAGS = (DeviceStatusFlags[])Enum.GetValues(typeof(DeviceStatusFlags));
			OS_PERMISSIONS = (OSPermission[])Enum.GetValues(typeof(OSPermission));
			OS_SERVICES = (OSService[])Enum.GetValues(typeof(OSService));

			DISABLED_DEVICE_CONFIG = new WearableDeviceConfig();
			DISABLED_DEVICE_CONFIG.DisableAllSensors();
			DISABLED_DEVICE_CONFIG.DisableAllGestures();

			#pragma warning disable 618
			DISALLOWED_EDITOR_PROVIDERS = new[]{ ProviderId.BluetoothProvider };
			DISALLOWED_RUNTIME_PROVIDERS = new[]{ ProviderId.USBProvider };
			#pragma warning restore 618

			DEBUG_PROVIDER_DEFAULT_AVAILABLE_ANR_MODES = new[] {
				ActiveNoiseReductionMode.Off,
				ActiveNoiseReductionMode.Low,
				ActiveNoiseReductionMode.High
			};

			EMPTY_DEVICE_STATUS = new DeviceStatus();
			EMPTY_DYNAMIC_DEVICE_INFO = new DynamicDeviceInfo
			{
				transmissionPeriod = -1,
				activeNoiseReductionMode = ActiveNoiseReductionMode.Invalid,
				availableActiveNoiseReductionModes = WearableTools.GetActiveNoiseReductionModesAsInt(EMPTY_ACTIVE_NOISE_REDUCTION_MODES),
				controllableNoiseCancellationLevel = 0,
				controllableNoiseCancellationEnabled = false,
				totalControllableNoiseCancellationLevels = 0
			};
		}
	}
}
