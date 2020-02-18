using System;
using System.Collections.Generic;
using System.Linq;
using Bose.Wearable.Extensions;
using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(WearableDebugProvider))]
	internal sealed class WearableDebugProviderDrawer : PropertyDrawer
	{
		private const string DEVICE_NAME_FIELD = "_name";
		private const string FIRMWARE_VERSION_FIELD = "_firmwareVersion";
		private const string GRANTED_PERMISSIONS_FIELD = "_grantedPermissionsFlags";
		private const string ENABLED_SERVICES_FIELD = "_enabledServicesFlags";
		private const string GRANT_PERMISSIONS_FIELD = "_grantPermission";
		private const string BOSE_AR_ENABLED_FIELD = "_boseArEnabled";
		private const string FIRMWARE_UPDATE_AVAILABLE_FIELD = "_firmwareUpdateAvailable";
		private const string ACCEPT_SECURE_PAIRING_FIELD = "_acceptSecurePairing";
		private const string RSSI_FIELD = "_rssi";
		private const string UID_FIELD = "_uid";
		private const string PRODUCT_ID_FIELD = "_productId";
		private const string VARIANT_ID_FIELD = "_variantId";
		private const string SENSOR_FLAGS_FIELD = "_availableSensors";
		private const string GESTURE_FLAGS_FIELD = "_availableGestures";
		private const string DEBUG_LOGGING_FIELD = "_debugLogging";
		private const string DELAY_TIME_FIELD = "_simulatedDelayTime";
		private const string MOVEMENT_SIMULATION_MODE_FIELD = "_simulatedMovementMode";
		private const string ROTATION_TYPE_FIELD = "_rotationType";
		private const string EULER_SPIN_RATE_FIELD = "_eulerSpinRate";
		private const string AXIS_ANGLE_SPIN_RATE_FIELD = "_axisAngleSpinRate";
		private const string DYNAMIC_DEVICE_INFO_FIELD = "_dynamicDeviceInfo";
		private const string SENSOR_CONFIG_RESULT_FIELD = "_sensorConfigurationResult";
		private const string GESTURE_CONFIG_RESULT_FIELD = "_gestureConfigurationResult";

		private const string ROTATION_TYPE_EULER = "Euler";
		private const string ROTATION_TYPE_AXIS_ANGLE = "AxisAngle";

		private const string DESCRIPTION_BOX =
			"Provides a minimal data provider that allows connection to a virtual device, and " +
			"logs messages when provider methods are called. If Simulate Movement is enabled, data " +
			"will be generated for all enabled sensors.";

		private const string PRODUCT_TYPE_LABEL = "Product Type";
		private const string VARIANT_TYPE_LABEL = "Variant Type";
		private const string GRANTED_PERMISSIONS_LABEL = "Granted Permissions";
		private const string ENABLED_SERVICES_LABEL = "Enabled Services";
		private const string USER_GRANTED_PERMISSIONS_LABEL = "User Grants Permissions";
		private const string USER_ENABLED_SERVICES_LABEL = "User Enables Services";
		private const string GESTURES_LABEL = "Simulate Gesture";
		private const string DISCONNECT_LABEL = "Simulate Device Disconnect";
		private const string EULER_RATE_BOX =
			"Simulates device rotation by changing each Euler angle (pitch, yaw, roll) at a fixed rate in degrees per second.";
		private const string RATE_LABEL = "Rate";
		private const string AXIS_ANGLE_BOX =
			"Simulates rotation around a fixed world-space axis at a specified rate in degrees per second.";
		private const string AXIS_LABEL = "Axis";
		private const string DEVICE_CONFIG_FIELDS_DISABLED_BOX_FORMAT =
			"Some properties of the virtual device are not configurable while the device is connected. To change " +
			"these fields, disconnect the device using the \"{0}\" button below.";
		private const string USE_SIMULATED_MOVEMENT_MOBILE_DEVICE_HELP_BOX =
			"You may simulate movement with a mobile device's IMU by either:\n"+
			"• Connecting a mobile device via USB and running the Unity Remote app. (Unity may take a few "+
			"seconds to recognize that the Remote has been connected.)\n" +
			"• Building and deploying to your mobile device.";
		private const string FIRMWARE_DEVICE_NOT_SUPPORTED_BOX =
			"The virtual firmware is not sufficient for connection, and no updates are available. Connection to the " +
			"virtual device will not be supported.";
		private const string FIRMWARE_UPDATE_REQUIRED_BOX =
			"The virtual firmware is not sufficient for connection, but an update is available. A firmware update will " +
			"be required before connection will succeed.";
		private const string FIRMWARE_GOOD_BOX = "The virtual firmware is sufficient for connection.";
		private const string FIRMWARE_UPDATE_AVAILABLE_BOX =
			"The virtual firmware is sufficient for connection, but an optional newer version is available.";
		private const string SECURE_PAIRING_ACCEPTED_BOX =
			"If the device status indicates that secure pairing is required, the provider will simulate a user " +
			"accepting that request.";
		private const string SECURE_PAIRING_REJECTED_BOX =
			"If the device status indicates that secure pairing is required, the provider will simulate a user " +
			"denying that request, and connection will fail.";
		private const string SENSOR_SERVICE_SUSPENSION_REASON_LABEL = "Sensor Service Suspension Reason";
		private const string SUSPEND_SENSOR_SERVICE_LABEL = "Suspend Sensor Service";
		private const string RESUME_SENSOR_SERVICE_LABEL = "Resume Sensor Service";
		private const string SENSOR_CONFIG_WILL_FAIL_BOX =
			"Any attempts to configure the device will fail, and the sensor configuration will remain unchanged.";
		private const string GESTURE_CONFIG_WILL_FAIL_BOX =
			"Any attempts to configure the device will fail, and the gesture configuration will remain unchanged.";
		private const string BOTH_CONFIG_WILL_FAIL_BOX =
			"Any attempts to configure the device will fail, and the sensor and gesture configuration will remain unchanged.";
		private const string ALL_PERMISSIONS_GRANTED_BOX = "All permissions have been granted by the user.";
		private const string ALL_SERVICES_GRANTED_BOX = "All services have been granted by the user.";
		private const string SOME_PERMISSIONS_MISSING_BOX
			= "Some permissions are missing, but will be accepted by the user.";
		private const string SOME_PERMISSIONS_MISSING_FAIL_BOX
			= "Some permissions are missing and will not be accepted by the user. This will halt the user " +
			  "until they grant the permissions or continue without Bose AR";
		private const string SOME_SERVICES_MISSING_FAIL_BOX
			= "Some services are missing and will not be enabled by the user. This will halt the user " +
			  "until they enable the service or continue without Bose AR";

		private SensorServiceSuspendedReason _sensorServiceSuspendedReason;

		private ProductType _productType;
		private Dictionary<string, byte> _editorVariantMap;
		private string[] _editorVariantOptions;
		private int _editorVariantIndex;
		private bool _editorVariantOptionsAreDirty;

		public WearableDebugProviderDrawer()
		{
			_sensorServiceSuspendedReason = SensorServiceSuspendedReason.UnknownReason;
			_editorVariantOptionsAreDirty = true;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			WearableDebugProvider provider = (WearableDebugProvider)fieldInfo.GetValue(property.serializedObject.targetObject);

			EditorGUI.BeginProperty(position, label, property);

			EditorGUILayout.HelpBox(DESCRIPTION_BOX, MessageType.None);
			EditorGUILayout.Space();

			// Virtual device config
			if (provider.ConnectedDevice.HasValue)
			{
				EditorGUILayout.HelpBox(string.Format(DEVICE_CONFIG_FIELDS_DISABLED_BOX_FORMAT, DISCONNECT_LABEL), MessageType.Info);
			}

			using (new EditorGUI.DisabledScope(provider.ConnectedDevice.HasValue))
			{
				// Device properties
				EditorGUILayout.IntSlider(
					property.FindPropertyRelative(RSSI_FIELD),
					WearableConstants.MINIMUM_RSSI_VALUE,
					WearableConstants.MAXIMUM_RSSI_VALUE,
					WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

				EditorGUILayout.Separator();
				EditorGUILayout.PropertyField(
					property.FindPropertyRelative(DEVICE_NAME_FIELD),
					WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				EditorGUILayout.PropertyField(
					property.FindPropertyRelative(UID_FIELD),
					WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

				// draw product and variant types based on ids
				var productIdProp = property.FindPropertyRelative(PRODUCT_ID_FIELD);
				_productType = WearableTools.GetProductType((ProductId)productIdProp.intValue);
				var variantProp = property.FindPropertyRelative(VARIANT_ID_FIELD);

				EditorGUI.BeginChangeCheck();
				_productType = (ProductType)EditorGUILayout.EnumPopup(
					PRODUCT_TYPE_LABEL,
					_productType
				);
				if (EditorGUI.EndChangeCheck())
				{
					// if we have changed the product, we need to reset the variants
					productIdProp.intValue = (int)WearableTools.GetProductId(_productType);
					variantProp.intValue = 0;
					_editorVariantOptionsAreDirty = true;
				}

				if (_editorVariantOptionsAreDirty)
				{
					_editorVariantMap = GetVariantMap(_productType);
					_editorVariantOptions = _editorVariantMap.Keys.ToArray();
					_editorVariantOptionsAreDirty = false;
				}

				string variantName = GetNameForProductAndVariantId(_productType, (byte)variantProp.intValue);
				var optionIndex = Array.IndexOf(_editorVariantOptions, variantName);
				_editorVariantIndex = EditorGUILayout.Popup(
					VARIANT_TYPE_LABEL,
					optionIndex >= 0 ? optionIndex : 0,
					_editorVariantOptions
				);

				variantProp.intValue = _editorVariantMap[_editorVariantOptions[_editorVariantIndex]];

				GUILayoutTools.LineSeparator();

				// Permissions and services
				var permissionsProperty = property.FindPropertyRelative(GRANTED_PERMISSIONS_FIELD);
				var permissionsEnumValue = EditorGUILayout.EnumFlagsField(
					new GUIContent(GRANTED_PERMISSIONS_LABEL),
					provider.GrantedPermissions);
				var permissionFlags = (OSPermissionFlags)Convert.ChangeType(permissionsEnumValue, typeof(OSPermissionFlags));
				permissionsProperty.intValue = (int)permissionFlags;

				var allPermissionsGranted = true;
				var allPermissionFlags = ((OSPermissionFlags[])Enum.GetValues(typeof(OSPermissionFlags)))
					.Where(x => x != OSPermissionFlags.None);
				foreach (var flag in allPermissionFlags)
				{
					if ((permissionFlags & flag) != flag)
					{
						allPermissionsGranted = false;
						break;
					}
				}

				EditorGUILayout.PropertyField(
					property.FindPropertyRelative(GRANT_PERMISSIONS_FIELD),
					new GUIContent(USER_GRANTED_PERMISSIONS_LABEL));

				if (allPermissionsGranted)
				{
					EditorGUILayout.HelpBox(ALL_PERMISSIONS_GRANTED_BOX, MessageType.Info);
				}
				else if(provider.GrantPermissions)
				{
					EditorGUILayout.HelpBox(SOME_PERMISSIONS_MISSING_BOX, MessageType.Warning);
				}
				else
				{
					EditorGUILayout.HelpBox(SOME_PERMISSIONS_MISSING_FAIL_BOX, MessageType.Error);
				}

				var servicesProperty = property.FindPropertyRelative(ENABLED_SERVICES_FIELD);
				var servicesEnumValue = EditorGUILayout.EnumFlagsField(
					new GUIContent(ENABLED_SERVICES_LABEL),
					provider.EnabledServices);
				var servicesFlag = (OSServiceFlags)Convert.ChangeType(servicesEnumValue, typeof(OSServiceFlags));
				servicesProperty.intValue = (int)servicesFlag;

				var allServicesEnabled = true;
				var allServicesFlags = ((OSServiceFlags[])Enum.GetValues(typeof(OSServiceFlags)))
					.Where(x => x != OSServiceFlags.None);
				foreach (var flag in allServicesFlags)
				{
					if ((servicesFlag & flag) != flag)
					{
						allServicesEnabled = false;
						break;
					}
				}

				if (allServicesEnabled)
				{
					EditorGUILayout.HelpBox(ALL_SERVICES_GRANTED_BOX, MessageType.Info);
				}
				else
				{
					EditorGUILayout.HelpBox(SOME_SERVICES_MISSING_FAIL_BOX, MessageType.Error);
				}

				GUILayoutTools.LineSeparator();

				// Firmware simulation
				EditorGUILayout.PropertyField(
					property.FindPropertyRelative(FIRMWARE_VERSION_FIELD),
					WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				var firmwareSufficient = property.FindPropertyRelative(BOSE_AR_ENABLED_FIELD);
				var firmwareAvailable = property.FindPropertyRelative(FIRMWARE_UPDATE_AVAILABLE_FIELD);
				EditorGUILayout.PropertyField(firmwareSufficient, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				EditorGUILayout.PropertyField(firmwareAvailable, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

				if (firmwareSufficient.boolValue)
				{
					if (firmwareAvailable.boolValue)
					{
						EditorGUILayout.HelpBox(FIRMWARE_UPDATE_AVAILABLE_BOX, MessageType.Info);
					}
					else
					{
						EditorGUILayout.HelpBox(FIRMWARE_GOOD_BOX, MessageType.Info);
					}
				}
				else
				{
					if (firmwareAvailable.boolValue)
					{
						EditorGUILayout.HelpBox(FIRMWARE_UPDATE_REQUIRED_BOX, MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox(FIRMWARE_DEVICE_NOT_SUPPORTED_BOX, MessageType.Error);
					}
				}

				// Secure pairing
				var acceptSecurePairing = property.FindPropertyRelative(ACCEPT_SECURE_PAIRING_FIELD);
				EditorGUILayout.PropertyField(acceptSecurePairing, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				if (acceptSecurePairing.boolValue)
				{
					EditorGUILayout.HelpBox(SECURE_PAIRING_ACCEPTED_BOX, MessageType.Info);
				}
				else
				{
					EditorGUILayout.HelpBox(SECURE_PAIRING_REJECTED_BOX, MessageType.Error);
				}

				// Sensor and gesture availability
				var sensorFlagsProp = property.FindPropertyRelative(SENSOR_FLAGS_FIELD);
				var sensorFlagsEnumValue = EditorGUILayout.EnumFlagsField(sensorFlagsProp.displayName, provider.AvailableSensors);
				sensorFlagsProp.intValue = (int) Convert.ChangeType(sensorFlagsEnumValue, typeof(SensorFlags));

				var gestureFlagsProp = property.FindPropertyRelative(GESTURE_FLAGS_FIELD);
				var gestureFlagsEnumValue = EditorGUILayout.EnumFlagsField(gestureFlagsProp.displayName, provider.AvailableGestures);
				gestureFlagsProp.intValue = (int) Convert.ChangeType(gestureFlagsEnumValue, typeof(GestureFlags));
			}
			EditorGUILayout.Space();

			// Simulated delay
			var delayProp = property.FindPropertyRelative(DELAY_TIME_FIELD);
			EditorGUILayout.PropertyField(delayProp, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			if (delayProp.floatValue < 0.0f)
			{
				delayProp.floatValue = 0.0f;
			}

			EditorGUILayout.Space();

			// Device status, ANR, and CNC via dynamic info
			GUILayoutTools.LineSeparator();
			EditorGUILayout.PropertyField(property.FindPropertyRelative(DYNAMIC_DEVICE_INFO_FIELD), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			GUILayoutTools.LineSeparator();

			DynamicDeviceInfo dynamicDeviceInfo = provider.GetDynamicDeviceInfo();
			DeviceStatus status = dynamicDeviceInfo.deviceStatus;
			using (new EditorGUI.DisabledScope(!provider.ConnectedDevice.HasValue))
			{
				bool serviceSuspended = status.GetFlagValue(DeviceStatusFlags.SensorServiceSuspended);

				// Service suspended
				using (new EditorGUI.DisabledScope(serviceSuspended))
				{
					// Only allow selecting a reason if the service isn't suspended
					_sensorServiceSuspendedReason = (SensorServiceSuspendedReason)EditorGUILayout.EnumPopup(
						SENSOR_SERVICE_SUSPENSION_REASON_LABEL,
						_sensorServiceSuspendedReason,
						WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				}

				if (serviceSuspended)
				{
					bool shouldResume = GUILayout.Button(RESUME_SENSOR_SERVICE_LABEL, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					if (shouldResume)
					{
						provider.SimulateSensorServiceResumed();
					}
				}
				else
				{
					bool shouldSuspend = GUILayout.Button(SUSPEND_SENSOR_SERVICE_LABEL, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					if (shouldSuspend)
					{
						provider.SimulateSensorServiceSuspended(_sensorServiceSuspendedReason);
					}
				}
			}

			EditorGUILayout.Space();

			// Configuration results
			var sensorConfigResultProperty = property.FindPropertyRelative(SENSOR_CONFIG_RESULT_FIELD);
			EditorGUILayout.PropertyField(sensorConfigResultProperty);
			var gestureConfigResultProperty = property.FindPropertyRelative(GESTURE_CONFIG_RESULT_FIELD);
			EditorGUILayout.PropertyField(gestureConfigResultProperty);

			var sensorConfigResult = (Result)sensorConfigResultProperty.enumValueIndex;
			var gestureConfigResult = (Result) gestureConfigResultProperty.enumValueIndex;
			if (sensorConfigResult == Result.Failure &&
				gestureConfigResult == Result.Failure)
			{
				EditorGUILayout.HelpBox(BOTH_CONFIG_WILL_FAIL_BOX, MessageType.Warning);
			}
			else if (sensorConfigResult == Result.Failure)
			{
				EditorGUILayout.HelpBox(SENSOR_CONFIG_WILL_FAIL_BOX, MessageType.Warning);
			}
			else if (gestureConfigResult == Result.Failure)
			{
				EditorGUILayout.HelpBox(GESTURE_CONFIG_WILL_FAIL_BOX, MessageType.Warning);
			}
			EditorGUILayout.Space();


			// Movement simulation
			SerializedProperty simulateMovementProperty = property.FindPropertyRelative(MOVEMENT_SIMULATION_MODE_FIELD);
			EditorGUILayout.PropertyField(simulateMovementProperty, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			var simulatedMovementMode = (WearableDebugProvider.MovementSimulationMode)simulateMovementProperty.enumValueIndex;

			if (simulatedMovementMode == WearableDebugProvider.MovementSimulationMode.ConstantRate)
			{
				SerializedProperty rotationTypeProperty = property.FindPropertyRelative(ROTATION_TYPE_FIELD);
				EditorGUILayout.PropertyField(rotationTypeProperty, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

				string rotationType = rotationTypeProperty.enumNames[rotationTypeProperty.enumValueIndex];
				if (rotationType == ROTATION_TYPE_EULER)
				{
					EditorGUILayout.HelpBox(EULER_RATE_BOX, MessageType.None);
					EditorGUILayout.PropertyField(property.FindPropertyRelative(EULER_SPIN_RATE_FIELD), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				}
				else if (rotationType == ROTATION_TYPE_AXIS_ANGLE)
				{
					EditorGUILayout.HelpBox(AXIS_ANGLE_BOX, MessageType.None);
					SerializedProperty axisAngleProperty = property.FindPropertyRelative(AXIS_ANGLE_SPIN_RATE_FIELD);
					Vector4 previousValue = axisAngleProperty.vector4Value;
					Vector4 newValue = EditorGUILayout.Vector3Field(
						AXIS_LABEL,
						new Vector3(previousValue.x, previousValue.y, previousValue.z),
						WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					if (newValue.sqrMagnitude < float.Epsilon)
					{
						newValue.x = 1.0f;
					}

					newValue.w = EditorGUILayout.FloatField(RATE_LABEL, previousValue.w, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					axisAngleProperty.vector4Value = newValue;
				}
			}
			else if (simulatedMovementMode == WearableDebugProvider.MovementSimulationMode.MobileDevice)
			{
				EditorGUILayout.HelpBox(USE_SIMULATED_MOVEMENT_MOBILE_DEVICE_HELP_BOX, MessageType.Info);
			}

			// Gesture triggers
			GUILayout.Label(GESTURES_LABEL, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				GestureId gesture = WearableConstants.GESTURE_IDS[i];

				if (gesture == GestureId.None)
				{
					continue;
				}

				using (new EditorGUI.DisabledScope(
					!(provider.GetCachedDeviceConfiguration().GetGestureConfig(gesture).isEnabled &&
					  EditorApplication.isPlaying)))
				{
					bool shouldTrigger = GUILayout.Button(Enum.GetName(typeof(GestureId), gesture), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					if (shouldTrigger)
					{
						provider.SimulateGesture(gesture);
					}
				}
			}

			// Disconnect button
			EditorGUILayout.Space();
			using (new EditorGUI.DisabledScope(!provider.ConnectedDevice.HasValue))
			{
				bool shouldDisconnect = GUILayout.Button(DISCONNECT_LABEL, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				if (shouldDisconnect)
				{
					provider.SimulateDisconnect();
				}
			}

			// Debug Logging
			GUI.changed = false;
			var debugLoggingProp = property.FindPropertyRelative(DEBUG_LOGGING_FIELD);
			EditorGUILayout.PropertyField(debugLoggingProp, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			if (Application.isPlaying && GUI.changed)
			{
				var activeProvider = WearableControl.Instance.ActiveProvider;
				if (activeProvider != null)
				{
					activeProvider.ConfigureDebugLogging();
				}
			}

			EditorGUI.EndProperty();
		}

		private Dictionary<string, byte> GetVariantMap(ProductType productType)
		{
			var map = new Dictionary<string, byte>();
			var names = WearableTools.GetVariantNamesForProduct(productType);
			var values = WearableTools.GetVariantValuesForProduct(productType);

			if (names != null)
			{
				for (int i = 0; i < names.Length; ++i)
				{
					map.Add(names[i].Nicify(), values[i]);
				}
			}

			return map;
		}

		private string GetNameForProductAndVariantId(ProductType productType, byte variantId)
		{
			string result = string.Empty;

			switch (productType)
			{
				case ProductType.Frames:
					result = Enum.GetName(typeof(FramesVariantId), variantId);
					break;
				case ProductType.QuietComfort35Two:
					result = Enum.GetName(typeof(QuietComfort35TwoVariantId), variantId);
					break;
				case ProductType.NoiseCancellingHeadphones700:
					result = Enum.GetName(typeof(NoiseCancellingHeadphones700VariantId), variantId);
					break;
				case ProductType.Unknown:
					result = ProductId.Undefined.ToString();
					break;
			}

			result = result == null ? string.Empty : result.Nicify();

			return result;
		}
	}
}
