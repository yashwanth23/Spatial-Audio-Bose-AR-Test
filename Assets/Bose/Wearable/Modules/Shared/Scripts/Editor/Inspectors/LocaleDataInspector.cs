using UnityEditor;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(LocaleData))]
	internal sealed class LocaleDataInspector : UnityEditor.Editor
	{
		// General UI
		private const string KVP_TITLE = "Key/Value Pairs";
		private const string LANGUAGES_TITLE = "Languages";
		private const string FONT_STYLE_TITLES = "Font Styling";

		// Property Names
		private const string LIST_PROPERTY_NAME = "_localeKVPs";
		private const string SUPPORTED_LANGUAGES_PROPERTY_NAME = "_supportedLanguages";
		private const string FONT_PROPERTY_NAME = "_overrideFontAsset";
		private const string FONT_STYLES_PROPERTY_NAME = "_fontStyles";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			using (var changeScope = new EditorGUI.ChangeCheckScope())
			{
				// Supported Languages
				EditorGUILayout.LabelField(LANGUAGES_TITLE, EditorStyles.boldLabel);
				EditorGUILayout.PropertyField(
					serializedObject.FindProperty(SUPPORTED_LANGUAGES_PROPERTY_NAME),
					true);

				// Font Styling
				EditorGUILayout.Space();
				EditorGUILayout.LabelField(FONT_STYLE_TITLES, EditorStyles.boldLabel);

				EditorGUILayout.PropertyField(serializedObject.FindProperty(FONT_PROPERTY_NAME));

				EditorGUILayout.PropertyField(
					serializedObject.FindProperty(FONT_STYLES_PROPERTY_NAME),
					true);

				if (changeScope.changed)
				{
					serializedObject.ApplyModifiedProperties();
					serializedObject.Update();

					EditorUtility.SetDirty(target);
				}
			}

			// Key/Value Pairs
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(KVP_TITLE, EditorStyles.boldLabel);
			var listProp = serializedObject.FindProperty(LIST_PROPERTY_NAME);
			for (var i = 0; i < listProp.arraySize; i++)
			{
				var elementProp = listProp.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(elementProp, true);
			}
		}
	}
}
