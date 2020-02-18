using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomPropertyDrawer(typeof(LocaleData.LocaleKVP))]
	internal sealed class LocaleKVPDrawer : PropertyDrawer
	{
		// UI Spacing
		private const float BOTTOM_PADDING = 5f;
		private const float LABEL_WIDTH = 50f;

		// General UI Text
		private const string KEY_LABEL = "Key:";
		private const string VALUE_LABEL = "Value:";

		// Property Names
		private const string KEY_PROPERTY_NAME = "key";
		private const string VALUE_PROPERTY_NAME = "value";

		private Rect _position;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_position = position;

			var indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Draw box background for entire object in inspector
			GUI.Box(new Rect(
					_position.x,
					_position.y,
					_position.width,
					GetPropertyHeight(property, label) - BOTTOM_PADDING),
				GUIContent.none);

			EditorGUI.BeginProperty(_position, label, property);

			// Draw Key label and entry
			GUI.Label(new Rect(
					_position.x,
					_position.y,
					LABEL_WIDTH,
					WearableEditorConstants.SINGLE_LINE_HEIGHT),
				KEY_LABEL,
				EditorStyles.boldLabel);

			GUI.Label(new Rect(
					_position.x + LABEL_WIDTH,
					_position.y,
					_position.width - LABEL_WIDTH,
					WearableEditorConstants.SINGLE_LINE_HEIGHT),
				property.FindPropertyRelative(KEY_PROPERTY_NAME).stringValue);

			// Draw Value label and entry
			GUI.Label(new Rect(
					_position.x,
					_position.y + WearableEditorConstants.SINGLE_LINE_HEIGHT,
					LABEL_WIDTH,
					WearableEditorConstants.SINGLE_LINE_HEIGHT),
				VALUE_LABEL,
				EditorStyles.boldLabel);

			GUI.TextArea(new Rect(
					_position.x + LABEL_WIDTH,
					_position.y + WearableEditorConstants.SINGLE_LINE_HEIGHT,
					_position.width - LABEL_WIDTH,
					GetValuePropHeight()),
				property.FindPropertyRelative(VALUE_PROPERTY_NAME).stringValue,
				EditorStyles.textArea);

			EditorGUI.indentLevel = indent;
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var baseHeight = WearableEditorConstants.SINGLE_LINE_HEIGHT + BOTTOM_PADDING;
			var valueHeight = GetValuePropHeight();
			return baseHeight + valueHeight;
		}

		private float GetValuePropHeight()
		{
			// Hard-coded for now to essentially three lines of text for the text-area because of extreme issues
			// ascertaining GUI.TextArea height. It should be as simple as calling GUIStyle.CalcHeight with the
			// appropriate width, but it's not.
			return WearableEditorConstants.SINGLE_LINE_HEIGHT * 2.5f;
		}
	}
}
