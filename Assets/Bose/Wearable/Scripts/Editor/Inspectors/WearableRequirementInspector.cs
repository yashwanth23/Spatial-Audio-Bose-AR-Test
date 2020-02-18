using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	[CustomEditor(typeof(WearableRequirement))]
	internal sealed class WearableRequirementInspector : UnityEditor.Editor
	{
		private const string DEVICE_CONFIG_PROPERTY_NAME = "_wearableDeviceConfig";

		public override void OnInspectorGUI()
		{
			GUI.changed = false;
			var property = serializedObject.FindProperty(DEVICE_CONFIG_PROPERTY_NAME);
			EditorGUILayout.PropertyField(property, WearableEditorConstants.EMPTY_LAYOUT_OPTIONS);

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();

				if (Application.isPlaying)
				{
					var requirement = (WearableRequirement)target;
					requirement.SetDirty();
				}
			}
		}
	}
}
