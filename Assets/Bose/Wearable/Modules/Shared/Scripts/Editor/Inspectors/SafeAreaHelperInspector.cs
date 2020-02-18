using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Connection.Editor
{
	[CustomEditor(typeof(SafeAreaHelper))]
	internal sealed class SafeAreaHelperInspector : UnityEditor.Editor
	{
		private const string SET_CURRENT_SAFE_AREA_BUTTON_TEXT = "Set Current Safe Area";
		private const string APPLY_SIMULATED_SAFE_AREA_BUTTON_TEXT = "Apply Simulated Safe Area";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var safeAreaHelper = (SafeAreaHelper)target;

			if (GUILayout.Button(SET_CURRENT_SAFE_AREA_BUTTON_TEXT))
			{
				safeAreaHelper.SetSafeAreaAsSimulatedSafeArea();
			}

			using (new EditorGUI.DisabledScope(!Application.isPlaying))
			{
				if (GUILayout.Button(APPLY_SIMULATED_SAFE_AREA_BUTTON_TEXT))
				{
					safeAreaHelper.ApplySimulatedSafeArea();
				}
			}
		}
	}
}
