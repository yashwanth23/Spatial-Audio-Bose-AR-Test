using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(WearableUSBProvider))]
	internal sealed class WearableUSBProviderDrawer : PropertyDrawer
	{
		private const string DESCRIPTION_BOX =
			"A provider that lets the Unity editor attach to a device connected by USB.";
		private const string DEBUG_LOGGING_FIELD = "_debugLogging";

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUILayout.HelpBox(DESCRIPTION_BOX, MessageType.None);
			EditorGUILayout.Space();

			GUI.changed = false;

			var debugLoggingProp = property.FindPropertyRelative(DEBUG_LOGGING_FIELD);
			EditorGUILayout.PropertyField(debugLoggingProp, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

			if (Application.isPlaying && GUI.changed)
			{
				property.serializedObject.ApplyModifiedProperties();
				var activeProvider = WearableControl.Instance.ActiveProvider;
				activeProvider.ConfigureDebugLogging();
			}
		}
	}
}
