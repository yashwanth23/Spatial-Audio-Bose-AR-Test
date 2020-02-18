using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(DynamicDeviceInfo))]
	internal sealed class DynamicDeviceInfoDrawer : PropertyDrawer
	{
		private const float TOP_PADDING = 5f;
		private const string STATUS_VALUES_FIELD = "deviceStatus._value";
		private const string AVAILABLE_ANR_MODES_FIELD = "availableActiveNoiseReductionModes";
		private const string CNC_LEVEL_FIELD = "controllableNoiseCancellationLevel";
		private const string CNC_ENABLED_FIELD = "controllableNoiseCancellationEnabled";
		private const string TOTAL_CNC_LEVELS_FIELD = "totalControllableNoiseCancellationLevels";
		private const string CURRENT_ANR_MODE_FIELD = "activeNoiseReductionMode";

		private const string AVAILABLE_ANR_MODES_HEADING = "Available ANR Modes";
		private const string CURRENT_ANR_MODE_LABEL = "Current ANR Mode";
		private const string TOTAL_CNC_LEVELS_LABEL_TEXT = "Total CNC Levels";
		private const string CNC_LEVEL_LABEL_TEXT = "Current CNC Lavel";
		private const string DEVICE_STATUS_HEADING = "Device Status";
		private const string ANR_HEADING = "Active Noise Reduction";
		private const string CNC_HEADING = "Controllable Noise Cancellation";
		private const string CNC_ENABLED_LABEL = "CNC Enabled";

		private GUIContent _totalCncLevelsLabel;

		public DynamicDeviceInfoDrawer()
		{
			_totalCncLevelsLabel = new GUIContent(TOTAL_CNC_LEVELS_LABEL_TEXT);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.BeginProperty(position, label, property);
			Rect line = new Rect(
				position.x,
				position.y,
				position.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT);

			// Device Status
			EditorGUI.LabelField(line, DEVICE_STATUS_HEADING);
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;

			Rect box = new Rect(
				line.x,
				line.y,
				line.width,
				line.height * (WearableConstants.DEVICE_STATUS_FLAGS.Length - 2)); // Flag count less "None" and "Suspended"
			GUI.Box(box, GUIContent.none);

			var statusValueProp = property.FindPropertyRelative(STATUS_VALUES_FIELD);
			DeviceStatus status = statusValueProp.intValue;
			for (int i = 0; i < WearableConstants.DEVICE_STATUS_FLAGS.Length; i++)
			{
				DeviceStatusFlags flag = WearableConstants.DEVICE_STATUS_FLAGS[i];
				if (flag == DeviceStatusFlags.None ||
				    flag == DeviceStatusFlags.SensorServiceSuspended)
				{
					continue;
				}

				using (new EditorGUI.DisabledScope(flag == DeviceStatusFlags.SensorServiceSuspended))
				{
					bool value = EditorGUI.Toggle(
						line,
						flag.ToString(),
						status.GetFlagValue(flag));

					status.SetFlagValue(flag, value);
				}

				line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			}
			statusValueProp.intValue = status;


			// Transmission period
			// No-op

			// ANR header
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT * 0.5f;
			EditorGUI.LabelField(line, ANR_HEADING);
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			box = new Rect(
				line.x,
				line.y,
				line.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT * (WearableConstants.ACTIVE_NOISE_REDUCTION_MODES.Length + 1));
			GUI.Box(box, GUIContent.none);


			// ANR current mode (read-only)
			using (new EditorGUI.DisabledScope(true))
			{
				var anrModeProperty = property.FindPropertyRelative(CURRENT_ANR_MODE_FIELD);
				var anrMode = (ActiveNoiseReductionMode) anrModeProperty.intValue;
				EditorGUI.LabelField(line, CURRENT_ANR_MODE_LABEL, anrMode.ToString());
				line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			}

			// ANR available modes
			EditorGUI.LabelField(line, AVAILABLE_ANR_MODES_HEADING);
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;

			EditorGUI.indentLevel++;
			var availableAnrModesProperty = property.FindPropertyRelative(AVAILABLE_ANR_MODES_FIELD);
			int oldAnrModes = availableAnrModesProperty.intValue;
			int newAnrModes = 0;
			for (int i = 0; i < WearableConstants.ACTIVE_NOISE_REDUCTION_MODES.Length; i++)
			{
				ActiveNoiseReductionMode mode = WearableConstants.ACTIVE_NOISE_REDUCTION_MODES[i];

				if (mode == ActiveNoiseReductionMode.Invalid)
				{
					continue;
				}

				int flag = (1 << (int) mode);
				bool selected = EditorGUI.Toggle(line, mode.ToString(), (flag & oldAnrModes) != 0);
				line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
				if (selected)
				{
					newAnrModes |= flag;
				}
			}

			EditorGUI.indentLevel--;

			if (newAnrModes != oldAnrModes)
			{
				availableAnrModesProperty.intValue = newAnrModes;
			}

			// CNC header
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT * 0.5f;
			EditorGUI.LabelField(line, CNC_HEADING);
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			box = new Rect(
				line.x,
				line.y,
				line.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT * 3);
			GUI.Box(box, GUIContent.none);

			using (new EditorGUI.DisabledScope(true))
			{
				// CNC Level (read-only)
				var cncLevelProperty = property.FindPropertyRelative(CNC_LEVEL_FIELD);
				EditorGUI.LabelField(line, CNC_LEVEL_LABEL_TEXT, cncLevelProperty.intValue.ToString());
				line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;

				// CNC Enabled (read-only)
				var cncEnabledProperty = property.FindPropertyRelative(CNC_ENABLED_FIELD);
				EditorGUI.Toggle(line, CNC_ENABLED_LABEL, cncEnabledProperty.boolValue);
				line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			}

			// Total CNC levels
			var totalCncLevelsProperty = property.FindPropertyRelative(TOTAL_CNC_LEVELS_FIELD);
			EditorGUI.PropertyField(line, totalCncLevelsProperty, _totalCncLevelsLabel);
			line.y += WearableEditorConstants.SINGLE_LINE_HEIGHT;
			if (totalCncLevelsProperty.intValue < 0)
			{
				totalCncLevelsProperty.intValue = 0;
			}

			EditorGUI.EndProperty();
			property.serializedObject.ApplyModifiedProperties();
			EditorGUI.indentLevel = indent;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return WearableEditorConstants.SINGLE_LINE_HEIGHT * (
				       WearableConstants.DEVICE_STATUS_FLAGS.Length - 2 + // Flag count less "None" and "Suspended"
				       WearableConstants.ACTIVE_NOISE_REDUCTION_MODES.Length - 1 + // Mode count less "Invalid"
				       9) + // Device Status Header + ANR header + ANR mode + CNC header + CNC level + CNC enabled + 3x spacing
				       TOP_PADDING;
		}
	}
}
