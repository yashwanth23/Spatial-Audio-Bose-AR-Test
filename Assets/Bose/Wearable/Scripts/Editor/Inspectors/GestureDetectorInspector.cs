using UnityEditor;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(GestureDetector))]
	internal sealed class GestureDetectorInspector : UnityEditor.Editor
	{
		private SerializedProperty _gestureId;
		private SerializedProperty _gestureEvent;

		private const string GESTURE_ID_FIELD = "_gesture";

		private const string GESTURE_EVENT_FIELD = "_onGestureDetected";

		private const string GESTURE_SELECT_MESSAGE = "Please select a Gesture.";

		private void OnEnable()
		{
			_gestureId = serializedObject.FindProperty(GESTURE_ID_FIELD);
			_gestureEvent = serializedObject.FindProperty(GESTURE_EVENT_FIELD);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GestureId gestureId = (GestureId)_gestureId.intValue;
			bool isGestureSelected = gestureId != (int) GestureId.None;

			EditorGUILayout.PropertyField(_gestureId, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

			if (!isGestureSelected)
			{
				EditorGUILayout.HelpBox(GESTURE_SELECT_MESSAGE, MessageType.Warning);
			}
			else if (gestureId.IsGestureDeviceSpecific())
			{
				EditorGUILayout.HelpBox(WearableEditorConstants.DEVICE_SPECIFIC_GESTURE_DISCOURAGED_WARNING, MessageType.Warning);
			}

			EditorGUILayout.PropertyField(_gestureEvent, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

			serializedObject.ApplyModifiedProperties();
		}
	}
}
