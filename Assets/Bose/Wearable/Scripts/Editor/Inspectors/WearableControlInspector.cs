using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bose.Wearable.Extensions;
using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(WearableControl))]
	internal sealed class WearableControlInspector : UnityEditor.Editor
	{
		private SerializedProperty _editorProvider;
		private SerializedProperty _runtimeProvider;

		private Dictionary<string, ProviderId> _editorProviderMap;
		private string[] _editorProviderOptions;
		private int _editorProviderIndex;

		private Dictionary<string, ProviderId> _runtimeProviderMap;
		private string[] _runtimeProviderOptions;
		private int _runtimeProviderIndex;

		private const string OVERRIDE_CONFIG_PRESENT_WARNING =
			"The device config is currently overridden and does not reflect the normal device config present " +
			"during normal runtime usage.";
		private const string SPECIFY_INTENT_WARNING =
			"Specify an intent profile to enable intent validation.";
		private const string INTENT_VALIDATION_IN_PROGRESS = "App intent validation in progress...";
		private const string INTENT_VALIDATION_SUCCEEDED = "Current app intent is valid.";
		private const string INTENT_VALIDATION_FAILED =
			"Current app intent is invalid! Some sensors, gestures, or update intervals might not be available on " +
			"this hardware or firmware version.";
		private const string INTENT_PROFILE_CHANGED =
			"The active intent profile has changed since last validation. Consider re-validating.";

		private const string EDITOR_DEFAULT_TITLE = "Editor Default";
		private const string RUNTIME_DEFAULT_TITLE = "Runtime Default";
		private const string RESOLVED_DEVICE_CONFIG_TITLE = "Resolved Device Config";
		private const string OVERRIDE_DEVICE_CONFIG_TITLE = "Override Device Config";
		private const string VALIDATE_INTENTS_TITLE = "Validate Intents";
		private const string TITLE_SEPARATOR = " - ";

		private const string EDITOR_DEFAULT_PROVIDER_FIELD = "_editorDefaultProvider";
		private const string RUNTIME_DEFAULT_PROVIDER_FIELD = "_runtimeDefaultProvider";
		private const string UPDATE_MODE_FIELD = "_updateMode";
		private const string DEBUG_PROVIDER_FIELD = "_debugProvider";
		private const string BLUETOOTH_PROVIDER_FIELD = "_bluetoothProvider";
		private const string USB_PROVIDER_FIELD = "_usbProvider";
		private const string INTENT_PROFILE_FIELD = "_activeAppIntentProfile";

		private const string FINAL_WEARABLE_DEVICE_CONFIG_FIELD = "_finalWearableDeviceConfig";
		private const string OVERRIDE_WEARABLE_DEVICE_CONFIG_FIELD = "_overrideDeviceConfig";

		private WearableControl _wearableControl;

		private void OnEnable()
		{
			_editorProvider = serializedObject.FindProperty(EDITOR_DEFAULT_PROVIDER_FIELD);
			_runtimeProvider = serializedObject.FindProperty(RUNTIME_DEFAULT_PROVIDER_FIELD);

			_editorProviderMap = GetProviderMap(WearableConstants.DISALLOWED_EDITOR_PROVIDERS);
			_editorProviderOptions = _editorProviderMap.Keys.ToArray();

			_runtimeProviderMap = GetProviderMap(WearableConstants.DISALLOWED_RUNTIME_PROVIDERS);
			_runtimeProviderOptions = _runtimeProviderMap.Keys.ToArray();

			_wearableControl = (WearableControl)target;
		}

		private void DrawProviderBox(string field, ProviderId provider)
		{
			bool isEditorDefault = _editorProvider.enumValueIndex == (int) provider;
			bool isRuntimeDefault = _runtimeProvider.enumValueIndex == (int) provider;

			if (isEditorDefault || isRuntimeDefault)
			{
				GUILayoutTools.LineSeparator();

				StringBuilder titleBuilder = new StringBuilder();
				titleBuilder.Append(Enum.GetName(typeof(ProviderId), provider));

				if (isEditorDefault)
				{
					titleBuilder.Append(TITLE_SEPARATOR);
					titleBuilder.Append(EDITOR_DEFAULT_TITLE);
				}

				if (isRuntimeDefault)
				{
					titleBuilder.Append(TITLE_SEPARATOR);
					titleBuilder.Append(RUNTIME_DEFAULT_TITLE);
				}

				EditorGUILayout.LabelField(titleBuilder.ToString(), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

				EditorGUILayout.PropertyField(serializedObject.FindProperty(field), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			}
		}

		private void DrawIntentsSection()
		{
			bool isDeviceConnected = _wearableControl.ConnectedDevice.HasValue;

			var status = _wearableControl.GetIntentValidationStatus();

			var profileProperty = serializedObject.FindProperty(INTENT_PROFILE_FIELD);
			AppIntentProfile oldProfile = profileProperty.objectReferenceValue as AppIntentProfile;
			EditorGUILayout.ObjectField(profileProperty, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
			AppIntentProfile newProfile = profileProperty.objectReferenceValue as AppIntentProfile;
			if (newProfile == null)
			{
				EditorGUILayout.HelpBox(SPECIFY_INTENT_WARNING, MessageType.Warning);
				return;
			}

			// If the profile changed at runtime, and there's a device connected, check intents again.
			if (oldProfile != newProfile && isDeviceConnected)
			{
				_wearableControl.SetIntentProfile(newProfile);
			}

			// Profile description
			EditorGUILayout.HelpBox(newProfile.ToString(), MessageType.None);

			if (!Application.isPlaying || !isDeviceConnected)
			{
				return;
			}

			// Re-validate warning
			if (status == IntentValidationStatus.Unknown)
			{
				EditorGUILayout.HelpBox(INTENT_PROFILE_CHANGED, MessageType.Warning);

				bool validateAgain = GUILayout.Button(VALIDATE_INTENTS_TITLE, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
				if (validateAgain)
				{
					_wearableControl.SetIntentProfile(newProfile);
					_wearableControl.ValidateIntentProfile();
				}
			}
			else
			{
				// Status box
				switch (status)
				{
					// "Unknown" is checked above, so no need to check it here.
					case IntentValidationStatus.Validating:
						EditorGUILayout.HelpBox(INTENT_VALIDATION_IN_PROGRESS, MessageType.Info);
						break;
					case IntentValidationStatus.Success:
						EditorGUILayout.HelpBox(INTENT_VALIDATION_SUCCEEDED, MessageType.Info);
						break;
					case IntentValidationStatus.Failure:
						EditorGUILayout.HelpBox(INTENT_VALIDATION_FAILED, MessageType.Error);
						break;
					case IntentValidationStatus.Disabled:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			// Update mode
			EditorGUILayout.PropertyField(serializedObject.FindProperty(UPDATE_MODE_FIELD), WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

			// Provider defaults
			using (new EditorGUI.DisabledScope(Application.isPlaying))
			{
				// Editor Default Provider
				string editorProviderName = GetNameForProvider((ProviderId)_editorProvider.intValue);
				int optionIndex = Array.IndexOf(_editorProviderOptions, editorProviderName);
				_editorProviderIndex = EditorGUILayout.Popup(
					ObjectNames.NicifyVariableName(EDITOR_DEFAULT_PROVIDER_FIELD),
					optionIndex >= 0 ? optionIndex : (int)WearableConstants.EDITOR_DEFAULT_PROVIDER,
					_editorProviderOptions
				);

				_editorProvider.intValue = (int)_editorProviderMap[_editorProviderOptions[_editorProviderIndex]];

				// Runtime Default Provider
				string runtimeProviderName = GetNameForProvider((ProviderId)_runtimeProvider.intValue);
				optionIndex = Array.IndexOf(_runtimeProviderOptions, runtimeProviderName);
				_runtimeProviderIndex = EditorGUILayout.Popup(
					ObjectNames.NicifyVariableName(RUNTIME_DEFAULT_PROVIDER_FIELD),
					optionIndex >= 0 ? optionIndex : (int)WearableConstants.RUNTIME_DEFAULT_PROVIDER,
					_runtimeProviderOptions
				);

				_runtimeProvider.intValue = (int)_runtimeProviderMap[_runtimeProviderOptions[_runtimeProviderIndex]];
			}

			// Intent profile
			GUILayoutTools.LineSeparator();
			DrawIntentsSection();

			// Providers
			DrawProviderBox(DEBUG_PROVIDER_FIELD, ProviderId.DebugProvider);
			DrawProviderBox(USB_PROVIDER_FIELD, ProviderId.USBProvider);
			DrawProviderBox(BLUETOOTH_PROVIDER_FIELD, ProviderId.BluetoothProvider);

			if (Application.isPlaying)
			{
				GUILayoutTools.LineSeparator();

				if (_wearableControl.IsOverridingDeviceConfig)
				{
					EditorGUILayout.LabelField(OVERRIDE_DEVICE_CONFIG_TITLE, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					EditorGUILayout.HelpBox(OVERRIDE_CONFIG_PRESENT_WARNING, MessageType.Warning);
					using (new EditorGUI.DisabledScope(true))
					{
						EditorGUILayout.PropertyField(
							serializedObject.FindProperty(OVERRIDE_WEARABLE_DEVICE_CONFIG_FIELD),
							WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					}
				}
				else
				{
					EditorGUILayout.LabelField(RESOLVED_DEVICE_CONFIG_TITLE, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					using (new EditorGUI.DisabledScope(true))
					{
						EditorGUILayout.PropertyField(
							serializedObject.FindProperty(FINAL_WEARABLE_DEVICE_CONFIG_FIELD),
							WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);
					}
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		public override bool RequiresConstantRepaint()
		{
			return Application.isPlaying;
		}

		private Dictionary<string, ProviderId> GetProviderMap(ProviderId[] disallowedProviders)
		{
			var providerNames = new Dictionary<string, ProviderId>();
			var providers = (ProviderId[])Enum.GetValues(typeof(ProviderId));
			for (int i = 0; i < providers.Length; ++i)
			{
				var providerId = providers[i];
				if (disallowedProviders.Contains(providerId))
				{
					continue;
				}

				var providerName = GetNameForProvider(providerId);
				providerNames.Add(providerName, providerId);
			}

			return providerNames;
		}

		private string GetNameForProvider(ProviderId providerId)
		{
			var result = Enum.GetName(typeof(ProviderId), providerId);
			result = result == null ? string.Empty : result.Nicify();

			return result;
		}
	}
}
