using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(AppIntentProfile))]
	internal sealed class AppIntentProfileInspector : UnityEditor.Editor
	{
		private const string SENSORS_LABEL = "Sensors";
		private const string GESTURES_LABEL = "Gestures";
		private const string INTERVALS_LABEL = "Sensor Update Intervals";
		private const string NINE_DOF_LABEL = "Rotation (9DOF)";
		private const string SIX_DOF_LABEL = "Rotation (6DOF)";
		private const string INTERVAL_FORMAT = "{0} ms";

		private readonly List<SensorId> _newSensors;
		private readonly List<GestureId> _newGestures;
		private readonly List<SensorUpdateInterval> _newIntervals;

		private AppIntentProfileInspector()
		{
			_newSensors = new List<SensorId>();
			_newGestures = new List<GestureId>();
			_newIntervals = new List<SensorUpdateInterval>();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUI.changed = false;

			AppIntentProfile profile = target as AppIntentProfile;
			if (profile == null)
			{
				// Nothing we can do, so give up.
				return;
			}

			// Sensors
			EditorGUILayout.LabelField(SENSORS_LABEL, EditorStyles.boldLabel);
			_newSensors.Clear();
			bool sensorsChanged = false;
			for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
			{
				SensorId id = WearableConstants.SENSOR_IDS[i];

				bool prior = profile.GetSensorInProfile(id);
				bool post = EditorGUILayout.Toggle(id.ToString(), prior, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				sensorsChanged |= prior != post;

				if (post)
				{
					_newSensors.Add(id);
				}
			}

			if (sensorsChanged)
			{
				profile.SetSensorIntent(_newSensors);
			}

			// Intervals
			GUILayoutTools.LineSeparator();
			EditorGUILayout.LabelField(INTERVALS_LABEL, EditorStyles.boldLabel);
			_newIntervals.Clear();
			bool intervalsChanged = false;
			for (int i = 0; i < WearableConstants.UPDATE_INTERVALS.Length; i++)
			{
				SensorUpdateInterval interval = WearableConstants.UPDATE_INTERVALS[i];
				string label = string.Format(
					INTERVAL_FORMAT,
					((int) WearableTools.SensorUpdateIntervalToMilliseconds(interval)).ToString());
				bool prior = profile.GetIntervalInProfile(interval);
				bool post = EditorGUILayout.Toggle(label, prior, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				intervalsChanged |= prior != post;

				if (post)
				{
					_newIntervals.Add(interval);
				}
			}

			if (intervalsChanged)
			{
				profile.SetIntervalIntent(_newIntervals);
			}


			// Gestures
			GUILayoutTools.LineSeparator();
			EditorGUILayout.LabelField(GESTURES_LABEL, EditorStyles.boldLabel);

			_newGestures.Clear();
			bool gesturesChanged = false;
			for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				GestureId id = WearableConstants.GESTURE_IDS[i];

				if (id == GestureId.None)
				{
					continue;
				}

				bool prior = profile.GetGestureInProfile(id);
				bool post = EditorGUILayout.Toggle(id.ToString(), prior, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				gesturesChanged |= prior != post;

				if (post)
				{
					_newGestures.Add(id);
				}
			}

			if (gesturesChanged)
			{
				profile.SetGestureIntent(_newGestures);
			}

			if (HasDeviceSpecificGesturesEnabled(profile))
			{
				EditorGUILayout.HelpBox(WearableEditorConstants.DEVICE_SPECIFIC_GESTURE_DISCOURAGED_WARNING, MessageType.Warning);
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}
		}

		private bool HasDeviceSpecificGesturesEnabled(AppIntentProfile profile)
		{
			for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				GestureId id = WearableConstants.GESTURE_IDS[i];

				if (id == GestureId.None)
				{
					continue;
				}

				if (profile.GetGestureInProfile(id) && id.IsGestureDeviceSpecific())
				{
					return true;
				}
			}

			return false;
		}
	}
}
