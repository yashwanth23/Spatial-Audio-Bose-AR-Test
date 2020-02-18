using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(WearableDeviceConfig))]
	internal sealed class WearableDeviceConfigDrawer : PropertyDrawer
	{
		// Property names
		private const string SENSOR_UPDATE_INTERVAL_PROPERTY_NAME = "updateInterval";
		private const string ACCELEROMETER_CONFIG_PROPERTY_NAME = "accelerometer";
		private const string GYROSCOPE_CONFIG_PROPERTY_NAME = "gyroscope";
		private const string ROTATION_NINE_DOF_CONFIG_PROPERTY_NAME = "rotationNineDof";
		private const string ROTATION_SIX_DOF_CONFIG_PROPERTY_NAME = "rotationSixDof";
		private const string DOUBLE_TAP_PROPERTY_NAME = "doubleTapGesture";
		private const string HEAD_NOD_PROPERTY_NAME = "headNodGesture";
		private const string HEAD_SHAKE_PROPERTY_NAME = "headShakeGesture";
		private const string TOUCH_AND_HOLD_PROPERTY_NAME = "touchAndHoldGesture";
		private const string INPUT_PROPERTY_NAME = "inputGesture";
		private const string AFFIRMATIVE_PROPERTY_NAME = "affirmativeGesture";
		private const string NEGATIVE_PROPERTY_NAME = "negativeGesture";

		private const string ENABLED_PROPERTY_NAME = "isEnabled";

		// UI
		private const string UNAVAILABLE_SENSOR_WARNING = "{0} Sensor Not Available on the Connected Device.";
		private const string UNAVAILABLE_GESTURE_WARNING = "{0} Gesture Not Available on the Connected Device.";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			var wearableControl = Application.isPlaying && WearableControl.Exists
				? WearableControl.Instance
				: null;
			var canShowWarning = Application.isPlaying &&
			                     wearableControl != null &&
			                     wearableControl.ConnectedDevice.HasValue;

			// Title
			var titleRect = new Rect(
				position.x,
				position.y,
				position.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT);
			GUI.Label(titleRect, "Sensors", EditorStyles.boldLabel);

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Update interval
			var sensorUpdateIntervalRect = new Rect(
				position.x,
				titleRect.y + titleRect.height,
				position.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT);

			EditorGUI.BeginDisabledGroup(HasAnySensorsEnabled(property));
			var sensorUpdateProp = property.FindPropertyRelative(SENSOR_UPDATE_INTERVAL_PROPERTY_NAME);
			EditorGUI.PropertyField(sensorUpdateIntervalRect, sensorUpdateProp);
			EditorGUI.EndDisabledGroup();


			// Accelerometer
			var accelProp = property.FindPropertyRelative(ACCELEROMETER_CONFIG_PROPERTY_NAME);
			var accelRect = new Rect(
				position.x,
				sensorUpdateIntervalRect.y + sensorUpdateIntervalRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(accelProp));
			EditorGUI.PropertyField(accelRect, accelProp);

			var accelWarningRect = new Rect(accelRect);
			if (canShowWarning && !wearableControl.GetWearableSensorById(SensorId.Accelerometer).IsAvailable)
			{
				accelWarningRect = new Rect(
					position.x,
					accelRect.y + accelRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					accelWarningRect,
					string.Format(UNAVAILABLE_SENSOR_WARNING, SensorId.Accelerometer),
					MessageType.Warning);
			}

			// Gyroscope
			var gyroProp = property.FindPropertyRelative(GYROSCOPE_CONFIG_PROPERTY_NAME);
			var gyroRect = new Rect(
				position.x,
				accelWarningRect.y + accelWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(gyroProp));
			EditorGUI.PropertyField(gyroRect, gyroProp);

			var gyroWarningRect = new Rect(gyroRect);
			if (canShowWarning && !wearableControl.GetWearableSensorById(SensorId.Gyroscope).IsAvailable)
			{
				gyroWarningRect = new Rect(
					position.x,
					gyroRect.y + gyroRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					gyroWarningRect,
					string.Format(UNAVAILABLE_SENSOR_WARNING, SensorId.Gyroscope),
					MessageType.Warning);
			}

			// Rotation (Nine Dof)
			var rotNineProp = property.FindPropertyRelative(ROTATION_NINE_DOF_CONFIG_PROPERTY_NAME);
			var rotNineRect = new Rect(
				position.x,
				gyroWarningRect.y + gyroWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(rotNineProp));
			EditorGUI.PropertyField(rotNineRect, rotNineProp);

			var rotNineWarningRect = new Rect(rotNineRect);
			if (canShowWarning && !wearableControl.GetWearableSensorById(SensorId.RotationNineDof).IsAvailable)
			{
				rotNineWarningRect = new Rect(
					position.x,
					rotNineRect.y + rotNineRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					rotNineWarningRect,
					string.Format(UNAVAILABLE_SENSOR_WARNING, SensorId.RotationNineDof),
					MessageType.Warning);
			}

			// Rotation (Six Dof)
			var rotSixProp = property.FindPropertyRelative(ROTATION_SIX_DOF_CONFIG_PROPERTY_NAME);
			var rotSixRect = new Rect(
				position.x,
				rotNineRect.y + rotNineRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(rotSixProp));
			EditorGUI.PropertyField(rotSixRect, rotSixProp);

			var rotSixWarningRect = new Rect(rotSixRect);
			if (canShowWarning && !wearableControl.GetWearableSensorById(SensorId.RotationSixDof).IsAvailable)
			{
				rotSixWarningRect = new Rect(
					position.x,
					rotSixRect.y + rotSixRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					rotSixWarningRect,
					string.Format(UNAVAILABLE_SENSOR_WARNING, SensorId.RotationSixDof),
					MessageType.Warning);
			}

			// Gestures
			var gesturesLabelRect = new Rect(
				position.x,
				rotSixWarningRect.y + rotSixWarningRect.height,
				position.width,
				WearableEditorConstants.SINGLE_LINE_HEIGHT);
			EditorGUI.LabelField(gesturesLabelRect, "Gestures", EditorStyles.boldLabel);

			// Double Tap
			var doubleTapProp = property.FindPropertyRelative(DOUBLE_TAP_PROPERTY_NAME);
			var doubleTapRect = new Rect(
				position.x,
				gesturesLabelRect.y + gesturesLabelRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(doubleTapProp));
			EditorGUI.PropertyField(doubleTapRect, doubleTapProp, new GUIContent(GestureId.DoubleTap.ToString()));

			var doubleTapWarningRect = new Rect(doubleTapRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.DoubleTap).IsAvailable)
			{
				doubleTapWarningRect = new Rect(
					position.x,
					doubleTapRect.y + doubleTapRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					doubleTapWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.DoubleTap),
					MessageType.Warning);
			}

			// Head Nod
			var headNodProp = property.FindPropertyRelative(HEAD_NOD_PROPERTY_NAME);
			var headNodRect = new Rect(
				position.x,
				doubleTapWarningRect.y + doubleTapWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(headNodProp));
			EditorGUI.PropertyField(headNodRect, headNodProp, new GUIContent(GestureId.HeadNod.ToString()));

			var headNodWarningRect = new Rect(headNodRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.HeadNod).IsAvailable)
			{
				headNodWarningRect = new Rect(
					position.x,
					headNodRect.y + headNodRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					headNodWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.HeadNod),
					MessageType.Warning);
			}

			// Head Shake
			var headShakeProp = property.FindPropertyRelative(HEAD_SHAKE_PROPERTY_NAME);
			var headShakeRect = new Rect(
				position.x,
				headNodWarningRect.y + headNodWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(headShakeProp));
			EditorGUI.PropertyField(headShakeRect, headShakeProp, new GUIContent(GestureId.HeadShake.ToString()));

			var headShakeWarningRect = new Rect(headShakeRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.HeadShake).IsAvailable)
			{
				headShakeWarningRect = new Rect(
					position.x,
					headShakeRect.y + headShakeRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					headShakeWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.HeadShake),
					MessageType.Warning);
			}

			// Touch and Hold
			var touchAndHoldProp = property.FindPropertyRelative(TOUCH_AND_HOLD_PROPERTY_NAME);
			var touchAndHoldRect = new Rect(
				position.x,
				headShakeWarningRect.y + headShakeWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(touchAndHoldProp));
			EditorGUI.PropertyField(touchAndHoldRect, touchAndHoldProp, new GUIContent(GestureId.TouchAndHold.ToString()));

			var touchAndHoldWarningRect = new Rect(touchAndHoldRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.TouchAndHold).IsAvailable)
			{
				touchAndHoldWarningRect = new Rect(
					position.x,
					touchAndHoldRect.y + touchAndHoldRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					touchAndHoldWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.TouchAndHold),
					MessageType.Warning);
			}

			// Input
			var inputProp = property.FindPropertyRelative(INPUT_PROPERTY_NAME);
			var inputRect = new Rect(
				position.x,
				touchAndHoldWarningRect.y + touchAndHoldWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(inputProp));
			EditorGUI.PropertyField(inputRect, inputProp, new GUIContent(GestureId.Input.ToString()));

			var inputWarningRect = new Rect(inputRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.Input).IsAvailable)
			{
				inputWarningRect = new Rect(
					position.x,
					inputRect.y + inputRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					inputWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.Input),
					MessageType.Warning);
			}

			// Affirmative
			var affirmativeProp = property.FindPropertyRelative(AFFIRMATIVE_PROPERTY_NAME);
			var affirmativeRect = new Rect(
				position.x,
				inputWarningRect.y + inputWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(affirmativeProp));
			EditorGUI.PropertyField(affirmativeRect, affirmativeProp, new GUIContent(GestureId.Affirmative.ToString()));

			var affirmativeWarningRect = new Rect(affirmativeRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.Affirmative).IsAvailable)
			{
				affirmativeWarningRect = new Rect(
					position.x,
					affirmativeRect.y + affirmativeRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					affirmativeWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.Affirmative),
					MessageType.Warning);
			}

			// Negative
			var negativeProp = property.FindPropertyRelative(NEGATIVE_PROPERTY_NAME);
			var negativeRect = new Rect(
				position.x,
				affirmativeWarningRect.y + affirmativeWarningRect.height,
				position.width,
				EditorGUI.GetPropertyHeight(negativeProp));
			EditorGUI.PropertyField(negativeRect, negativeProp, new GUIContent(GestureId.Negative.ToString()));

			var negativeWarningRect = new Rect(negativeRect);
			if (canShowWarning && !wearableControl.GetWearableGestureById(GestureId.Negative).IsAvailable)
			{
				negativeWarningRect = new Rect(
					position.x,
					negativeRect.y + negativeRect.height,
					position.width,
					WearableEditorConstants.SINGLE_LINE_HEIGHT * 2);
				EditorGUI.HelpBox(
					negativeWarningRect,
					string.Format(UNAVAILABLE_GESTURE_WARNING, GestureId.Negative),
					MessageType.Warning);
			}

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var newProps = new[]
			{
				property.FindPropertyRelative(SENSOR_UPDATE_INTERVAL_PROPERTY_NAME),
				property.FindPropertyRelative(ACCELEROMETER_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(GYROSCOPE_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(ROTATION_NINE_DOF_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(ROTATION_SIX_DOF_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(DOUBLE_TAP_PROPERTY_NAME),
				property.FindPropertyRelative(HEAD_NOD_PROPERTY_NAME),
				property.FindPropertyRelative(HEAD_SHAKE_PROPERTY_NAME),
				property.FindPropertyRelative(TOUCH_AND_HOLD_PROPERTY_NAME),
				property.FindPropertyRelative(INPUT_PROPERTY_NAME),
				property.FindPropertyRelative(AFFIRMATIVE_PROPERTY_NAME),
				property.FindPropertyRelative(NEGATIVE_PROPERTY_NAME)
			};

			var height = WearableEditorConstants.SINGLE_LINE_HEIGHT * 2;
			for (var i = 0; i < newProps.Length; i++)
			{
				height += EditorGUI.GetPropertyHeight(newProps[i]);
			}

			var wearableControl = Application.isPlaying && WearableControl.Exists
				? WearableControl.Instance
				: null;
			var canShowWarning = Application.isPlaying &&
			                     wearableControl != null &&
			                     wearableControl.ConnectedDevice.HasValue;

			if (canShowWarning)
			{
				for (var i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
				{
					var sensorId = WearableConstants.SENSOR_IDS[i];
					var sensor = wearableControl.GetWearableSensorById(sensorId);
					if (!sensor.IsAvailable)
					{
						height += WearableEditorConstants.SINGLE_LINE_HEIGHT * 2;
					}
				}

				for (var i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
				{
					var gestureId = WearableConstants.GESTURE_IDS[i];
					if (gestureId == GestureId.None)
					{
						continue;
					}

					var gesture = wearableControl.GetWearableGestureById(gestureId);
					if (!gesture.IsAvailable)
					{
						height += WearableEditorConstants.SINGLE_LINE_HEIGHT * 2;
					}
				}
			}

			return height;
		}

		/// <summary>
		/// Returns true if any sensors are enabled.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private static bool HasAnySensorsEnabled(SerializedProperty property)
		{
			var newProps = new[]
			{
				property.FindPropertyRelative(ACCELEROMETER_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(GYROSCOPE_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(ROTATION_NINE_DOF_CONFIG_PROPERTY_NAME),
				property.FindPropertyRelative(ROTATION_SIX_DOF_CONFIG_PROPERTY_NAME)
			};

			var numberOfSensorsActive = 0;
			for (var i = 0; i < newProps.Length; i++)
			{
				if (!newProps[i].FindPropertyRelative(ENABLED_PROPERTY_NAME).boolValue)
				{
					continue;
				}

				numberOfSensorsActive++;
			}

			return numberOfSensorsActive == 0;
		}
	}
}
