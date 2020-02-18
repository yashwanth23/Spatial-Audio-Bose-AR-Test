using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// <see cref="DeveloperAboutWindow"/> is an editor window used to show version information for the
	/// SDK and any of its dependent assemblies.
	/// </summary>
	internal class DeveloperAboutWindow : EditorWindow
	{
		// Window UI
		private const string WINDOW_TITLE = "Bose AR SDK for Unity";

		// Version UI
		private const string UNITY_SDK_VERSION_LABEL = "SDK Version:";

		#pragma warning disable 0649

		[SerializeField]
		private Texture2D _boseArSdkLogo;

		#pragma warning restore 0649

		public static void LaunchWindow()
		{
			var window = CreateInstance<DeveloperAboutWindow>();
			window.minSize = new Vector2(300f, 115f);
			window.maxSize = window.minSize;
			window.titleContent = new GUIContent(WINDOW_TITLE);
			window.position = new Rect(Screen.currentResolution.width / 2f, Screen.currentResolution.height/2f, 0f, 0f);
			window.ShowUtility();
		}

		private void OnGUI()
		{
			GUILayout.Label(_boseArSdkLogo);

			GUILayoutTools.LineSeparator();

			// Draw SDK version
			EditorGUILayout.BeginHorizontal(GUILayout.Width(300f));
			EditorGUILayout.LabelField(UNITY_SDK_VERSION_LABEL, EditorStyles.boldLabel, GUILayout.Width(200f));
			EditorGUILayout.LabelField(WearableVersion.UnitySdkVersion);
			EditorGUILayout.EndHorizontal();
		}
	}
}
