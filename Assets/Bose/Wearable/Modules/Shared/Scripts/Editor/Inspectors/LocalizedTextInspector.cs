using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(LocalizedText))]
	internal sealed class LocalizedTextInspector : UnityEditor.Editor
	{
		private LocalizedText _localizedText;

		private SerializedProperty _localeKeyProperty;

		private GUIContent _dropdownButtonGUIContent;
		private GUIContent _languagePreviewGUIContent;

		private SystemLanguage _previewSystemLanguage;

		// UI
		private const string DROPDOWN_BUTTON_TEXT = "Set Locale Key";

		private const string PREVIEW_LOCALE_LABEL = "Test Preferred Language";

		private const string COMPONENT_REFS_TITLE = "UI References";
		private const string DATA_TITLE = "Localization Data";
		private const string ACTIONS_TITLE = "Debug Tools";

		private const string LOCALE_KEY_NOT_FOUND
			= "Locale Key not found, this will result in the displayed value being set to an empty string. " +
			  "Please set to a valid Locale Key";

		// Property Names
		private const string TMP_TEXT_PROPERTY_NAME = "_tmpText";
		private const string LOCALE_KEY_PROPERTY_NAME = "_localeKey";
		private const string FONT_STYLE_ID_PROPERTY_NAME = "_fontStyleId";

		private void OnEnable()
		{
			_dropdownButtonGUIContent = new GUIContent(DROPDOWN_BUTTON_TEXT);
			_languagePreviewGUIContent = new GUIContent(PREVIEW_LOCALE_LABEL);

			_localeKeyProperty = serializedObject.FindProperty(LOCALE_KEY_PROPERTY_NAME);

			_previewSystemLanguage = LocaleCache.GetPreferredSystemLanguage();

			Undo.undoRedoPerformed += OnUndoRedoPerformed;
		}

		public override void OnInspectorGUI()
		{
			_localizedText = (LocalizedText)target;

			// Scene/Prefab References
			GUILayout.Space(5);
			GUILayout.Label(COMPONENT_REFS_TITLE, EditorStyles.boldLabel);

			var tmpTextProperty = serializedObject.FindProperty(TMP_TEXT_PROPERTY_NAME);
			using (new EditorGUILayout.HorizontalScope())
			{
				using (var changeScope = new EditorGUI.ChangeCheckScope())
				{
					EditorGUILayout.LabelField(tmpTextProperty.displayName, GUILayout.MaxWidth(100f));
					EditorGUILayout.PropertyField(tmpTextProperty, GUIContent.none);

					if (changeScope.changed)
					{
						ForceUpdate();
					}
				}
			}

			// Disable all controls if the Text component is not assigned.
			using (new EditorGUI.DisabledScope(tmpTextProperty.objectReferenceValue == null))
			{
				// Locale Data
				GUILayout.Space(5);
				GUILayout.Label(DATA_TITLE, EditorStyles.boldLabel);

				// Display a warning if the currently set locale key is not found.
				if (!LocaleCache.HasLocaleKey(_localeKeyProperty.stringValue))
				{
					EditorGUILayout.HelpBox(LOCALE_KEY_NOT_FOUND, MessageType.Error);
				}

				using (var changeScope = new EditorGUI.ChangeCheckScope())
				{
					using (new EditorGUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField(_localeKeyProperty.displayName, GUILayout.MaxWidth(100f));
						EditorGUILayout.LabelField(_localeKeyProperty.stringValue);

						var lastRect = GUILayoutUtility.GetRect(_dropdownButtonGUIContent, EditorStyles.toolbarDropDown);
						if (EditorGUI.DropdownButton(lastRect, _dropdownButtonGUIContent, FocusType.Passive))
						{
							LocaleKeyDropDownMenu.ShowAsDropdown(lastRect, _localeKeyProperty.stringValue, OnLocaleKeySelected);
						}

					}

					using (new EditorGUILayout.HorizontalScope())
					{
						var fontStyleProperty = serializedObject.FindProperty(FONT_STYLE_ID_PROPERTY_NAME);
						EditorGUILayout.LabelField(fontStyleProperty.displayName, GUILayout.MaxWidth(100f));
						EditorGUILayout.PropertyField(fontStyleProperty, GUIContent.none);
					}

					if (changeScope.changed)
					{
						ForceUpdate();
					}
				}

				// Actions
				GUILayout.Space(5);
				GUILayout.Label(ACTIONS_TITLE, EditorStyles.boldLabel);

				// If a language is set as preferred that is not supported, show a warning.
				if (!LocaleCache.IsSupportedSystemLanguage(_previewSystemLanguage))
				{
					var msg = string.Format(
						WearableEditorConstants.LANGUAGE_NOT_SUPPORTED,
						_previewSystemLanguage,
						LocaleTools.GetFallbackSystemLanguage());
					EditorGUILayout.HelpBox(msg, MessageType.Warning);
				}

				// Allow the user to select a different language to see what the resulting locale value would be.
				// If it is not a language we directly support, the fallback language will be used instead.
				using (new EditorGUILayout.HorizontalScope())
				{
					using (var changeScope = new EditorGUI.ChangeCheckScope())
					{
						_previewSystemLanguage = (SystemLanguage)EditorGUILayout.EnumPopup(
							_languagePreviewGUIContent,
							_previewSystemLanguage);

						if (changeScope.changed)
						{
							// Set our new preferred language and update the displayed text to match.
							LocaleCache.SetPreferredSystemLanguage(_previewSystemLanguage);

							ForceUpdate();
						}
					}
				}
			}
		}

		private void ForceUpdate()
		{
			serializedObject.ApplyModifiedProperties();

			// Force the LocaleCache to be rebuilt as keys may have been added or removed
			// since this was last called in the editor.
			LocaleCache.ForceSetupLocaleCache();

			var textProperty = serializedObject.FindProperty(TMP_TEXT_PROPERTY_NAME);
			if (textProperty.objectReferenceValue != null)
			{
				_localizedText.UpdateLocale(_previewSystemLanguage);
			}

			EditorUtility.SetDirty(target);
		}

		/// <summary>
		/// Invoked when a user has selected a new locale key; updates the serialized object with the new
		/// selection and updates the Text component value.
		/// </summary>
		/// <param name="value"></param>
		private void OnLocaleKeySelected(object value)
		{
			_localeKeyProperty.stringValue = value.ToString();

			ForceUpdate();
		}

		private void OnUndoRedoPerformed()
		{
			// If our undo occurred as a result of deleting this component, return early.
			if (_localizedText == null)
			{
				return;
			}

			// When an undo/redo is performed, reset the displayed localized text in case the key value changed,
			// but only if there is a text component to update.
			var textProperty = serializedObject.FindProperty(TMP_TEXT_PROPERTY_NAME);
			if (textProperty.objectReferenceValue == null)
			{
				return;
			}

			ForceUpdate();
		}
	}
}
