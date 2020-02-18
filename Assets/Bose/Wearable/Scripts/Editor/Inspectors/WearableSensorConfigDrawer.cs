using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(WearableDeviceConfig.WearableSensorConfig))]
	internal sealed class WearableSensorConfigDrawer : PropertyDrawer
	{
		private const string ENABLED_PROPERTY_NAME = "isEnabled";
		private const float BOTTOM_PADDING = 5f;
		private const float LABEL_WIDTH = 125f;
		private const float LABEL_PADDING = 15f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			EditorGUI.BeginProperty(position, label, property);
			GUI.Box(new Rect(
				position.x,
				position.y,
				position.width,
				GetPropertyHeight(property, label) - BOTTOM_PADDING),
				GUIContent.none);

			GUI.Label(new Rect(
				position.x,
				position.y,
				LABEL_WIDTH,
				WearableEditorConstants.SINGLE_LINE_HEIGHT),
				label,
				EditorStyles.boldLabel);

			var onEnableProp = property.FindPropertyRelative(ENABLED_PROPERTY_NAME);
			var onEnableRect = new Rect(
				position.x + LABEL_WIDTH + LABEL_PADDING,
				position.y,
				position.width - LABEL_WIDTH + LABEL_PADDING,
				WearableEditorConstants.SINGLE_LINE_HEIGHT);

			EditorGUI.PropertyField(onEnableRect, onEnableProp, GUIContent.none);

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return WearableEditorConstants.SINGLE_LINE_HEIGHT + BOTTOM_PADDING;
		}
	}
}
