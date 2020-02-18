using Bose.Wearable.Examples;
using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(GestureIconFactory))]
	internal sealed class GestureIconFactoryInspector : UnityEditor.Editor
	{
		private const string LIST_PROPERTY_NAME = "_gestureToIcons";
		private const string GESTURE_ID_PROPERTY_NAME = "gestureId";
		private const string GESTURE_ICON_PROPERTY_NAME = "gestureSprite";

		public override void OnInspectorGUI()
		{
			var listProp = serializedObject.FindProperty(LIST_PROPERTY_NAME);
			var originalIndentLevel = EditorGUI.indentLevel;

			GUI.changed = false;
			EditorGUI.indentLevel = 0;
			for (var i = 0; i < listProp.arraySize; i++)
			{
				var entryProp = listProp.GetArrayElementAtIndex(i);
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.PropertyField(entryProp.FindPropertyRelative(GESTURE_ID_PROPERTY_NAME));
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.PropertyField(entryProp.FindPropertyRelative(GESTURE_ICON_PROPERTY_NAME));
				GUILayoutTools.LineSeparator();
			}
			EditorGUI.indentLevel = originalIndentLevel;

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}
