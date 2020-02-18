using Bose.Wearable.Extensions;
using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor.Inspectors
{
	[CustomEditor(typeof(RotationMatcher))]
	public sealed class RotationMatcherInspector : UnityEditor.Editor
	{
		private const string ROTATION_SOURCE_FIELD = "_rotationSource";
		private const string UPDATE_INTERVAL_FIELD = "_updateInterval";

		private const string DESCRIPTION_BOX =
			"Automatically rotates the attached GameObject to match the orientation of the connected device. Either " +
			"rotation sensor may be used.";

		private const string REQUIREMENT_WILL_BE_CREATED_INFO =
			"A WearableRequirement component will automatically be added to this GameObject at runtime. You may also " +
			"add one manually to provide more control over device configuration.";

		private const string REQUIREMENT_REMOVED_OR_DISABLED_WARNING =
			"The WearableRequirement component on this GameObject has been removed or disabled. If no other " +
			"Requirements in your project enable the proper rotation sensor, the Rotation Matcher will not function.";

		private const string REQUIRED_ROTATION_SENSOR_MANUALLY_DISABLED_WARNING =
			"The required rotation sensor has been manually disabled on the attached WearableRequirement component. " +
			"If no other Requirements in your project enable the proper rotation sensor, the Rotation Matcher will " +
			"not function.";

		private const string REQUIRED_UPDATE_INTERVAL_MANUALLY_LOWERED_WARNING =
			"The update interval on the attached WearableRequirement component has been manually changed to an " +
			"interval slower than that requested above. If no other Requirements in your project request a faster " +
			"rate, the Rotation Matcher will update at a lower rate than requested.";

		private const string REQUIREMENT_SENSORS_WILL_BE_ALTERED_INFO =
			"The attached WearableRequirement does not enable the rotation sensor required by the Rotation Matcher " +
			"component. It will be automatically altered at runtime to enable the proper sensor.";

		private const string REQUIREMENT_UPDATE_INTERVAL_WILL_BE_ALTERED_INFO =
			"The attached WearableRequirement has a slower update interval than that requested above. The requirement " +
			" will be automatically altered at runtime to ensure the proper update interval is reached.";

		private const string CAMERA_FOUND_WARNING =
			"A camera was detected in the hierarchy of this GameObject. Using a RotationMatcher to drive the rotation " +
			"of a camera is not recommended.";

		private SerializedProperty _rotationSource;
		private SerializedProperty _updateInterval;

		private void OnEnable()
		{
			_rotationSource = serializedObject.FindProperty(ROTATION_SOURCE_FIELD);
			_updateInterval = serializedObject.FindProperty(UPDATE_INTERVAL_FIELD);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			RotationMatcher matcher = target as RotationMatcher;

			CheckAndWarnForCameraUsage(matcher);

			EditorGUILayout.HelpBox(DESCRIPTION_BOX, MessageType.None);
			EditorGUILayout.PropertyField(_rotationSource);
			EditorGUILayout.PropertyField(_updateInterval);

			WearableRequirement requirement = matcher.GetComponent<WearableRequirement>();

			if (Application.isPlaying)
			{
				if (requirement == null || !requirement.enabled)
				{
					// Runtime, manually removed
					EditorGUILayout.HelpBox(REQUIREMENT_REMOVED_OR_DISABLED_WARNING, MessageType.Warning);
				}
			}
			else
			{
				if (requirement == null)
				{
					// Editor, no user-provided
					EditorGUILayout.HelpBox(REQUIREMENT_WILL_BE_CREATED_INFO, MessageType.Info);
				}
			}

			if (requirement != null && requirement.enabled)
			{
				// Editor, user provided OR runtime

				// Check sensor validity
				var source = (RotationMatcher.RotationSensorSource) _rotationSource.enumValueIndex;
				SensorId rotationSensorId =
					source == RotationMatcher.RotationSensorSource.SixDof ?
						SensorId.RotationSixDof :
						SensorId.RotationNineDof;
				if (!requirement.DeviceConfig.GetSensorConfig(rotationSensorId).isEnabled)
				{
					if (Application.isPlaying)
					{
						EditorGUILayout.HelpBox(REQUIRED_ROTATION_SENSOR_MANUALLY_DISABLED_WARNING, MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox(REQUIREMENT_SENSORS_WILL_BE_ALTERED_INFO, MessageType.Info);
					}
				}

				// Check rate validity
				var interval = (SensorUpdateInterval) _updateInterval.enumValueIndex;
				if (requirement.DeviceConfig.updateInterval.IsSlowerThan(interval))
				{
					if (Application.isPlaying)
					{
						EditorGUILayout.HelpBox(REQUIRED_UPDATE_INTERVAL_MANUALLY_LOWERED_WARNING, MessageType.Warning);
					}
					else
					{
						EditorGUILayout.HelpBox(REQUIREMENT_UPDATE_INTERVAL_WILL_BE_ALTERED_INFO, MessageType.Info);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		private void CheckAndWarnForCameraUsage(RotationMatcher matcher)
		{
			var camera = matcher.GetComponentInChildren<Camera>();

			if (camera != null)
			{
				EditorGUILayout.HelpBox(CAMERA_FOUND_WARNING, MessageType.Warning);
			}
		}
	}
}
