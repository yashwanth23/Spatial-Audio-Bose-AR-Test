using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Bose.Wearable.Editor
{
	internal static class BuildTools
	{
		// Shared
		private const string APP_VERSION = "1.0";

		// Wearable Demo
		private const string WEARABLE_DEMO_PRODUCT_NAME = "Wearable Demo";
		private const string WEARABLE_DEMO_APP_IDENTIFIER = "com.bose.demo.wearableunity";

		public const string WEARABLE_DEMO_ICON_GUID = "e06b243adbc49564b8ba586a2a0ed2d0";
		public const string BOSE_LOGO_ICON_GUID = "ca23c79dbd8ec42fb98cc414eb988fa8";

		private const string ROOT_SCENE_GUID = "b100476eb79061246a7d53542a204e54";
		private const string MAIN_MENU_SCENE_GUID = "814d265ed5a714b2f8a496b0e00010e1";
		private const string BASIC_DEMO_SCENE_GUID = "e822a72393d35429f941bfee942e76f4";
		private const string ADVANCED_DEMO_SCENE_GUID = "422b6c809820b4a78b2c60a058c8a7b4";
		private const string GESTURE_DEMO_SCENE_GUID = "6cb706a67df9fd948a79d1d93f05bef2";
		private const string DEBUG_DEMO_SCENE_GUID = "4a3d676024060ba4a98a63dccf9c9a93";

		public static readonly string[] WEARABLE_DEMO_SCENE_GUIDS =
		{
			ROOT_SCENE_GUID,
			MAIN_MENU_SCENE_GUID,
			BASIC_DEMO_SCENE_GUID,
			ADVANCED_DEMO_SCENE_GUID,
			GESTURE_DEMO_SCENE_GUID,
			DEBUG_DEMO_SCENE_GUID
		};

		// Build
		private const string CANNOT_BUILD_ERROR_MESSAGE = "[Bose Wearable] Cannot build the {0} for {1} as component " +
		                                               "support for that platform is not installed. Please " +
		                                               "install this component to continue, stopping build...";
		private const string CANNOT_BUILD_MISSING_SCENE_ERROR_MESSAGE = "[Bose Wearable] Could not find a scene for " +
		                                                           "the {0}, stopping build";
		private const string CANNOT_FIND_APP_ICON = "[Bose Wearable] Could not find the application icon for the Bose Wearable " +
		                                         "example content.";
		private const string BUILD_SCENES_COULD_NOT_BE_FOUND = "[Bose Wearable] Scenes could not be found for {0}, " +
		                                                  "stopping build..";
		private const string BUILD_SUCCEEDED_MESSAGE = "[Bose Wearable] {0} Build Succeeded!";
		private const string BUILD_FAILED_MESSAGE = "[Bose Wearable] {0} Build Failed! {1}";

		/// <summary>
		/// An editor-pref key for where the user last selected the build location.
		/// </summary>
		private const string BUILD_LOCATION_PREFERENCE_KEY = "bose_wearable_pref_key";

		// Folder Picker
		private const string FOLDER_PICKER_TITLE = "Build Location for {0}";

		// Unity Cloud Build
		private const string BUILD_SCENES_SET_MESSAGE = "[Bose Wearable] Build Scenes Set for {0}.";

		internal static void BuildWearableDemo()
		{
			// Check for player support
			if (!CanBuildTarget(EditorUserBuildSettings.activeBuildTarget))
			{
				Debug.LogErrorFormat(CANNOT_BUILD_ERROR_MESSAGE, WEARABLE_DEMO_PRODUCT_NAME, EditorUserBuildSettings.activeBuildTarget);
				return;
			}

			// Get folder path from the user for the build
			var folderPath = GetBuildLocation(WEARABLE_DEMO_PRODUCT_NAME);
			if (string.IsNullOrEmpty(folderPath))
			{
				return;
			}

			var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			// Cache values for the current Player Settings
			var originalProductName = PlayerSettings.productName;
			var bundleVersion = PlayerSettings.bundleVersion;
			var appId = PlayerSettings.GetApplicationIdentifier(buildTargetGroup);
			var iconGroup = PlayerSettings.GetIconsForTargetGroup(buildTargetGroup);

			var showSplash = PlayerSettings.SplashScreen.show;
			var splashStyle = PlayerSettings.SplashScreen.unityLogoStyle;
			var splashColor = PlayerSettings.SplashScreen.backgroundColor;
			var splashAnimationMode = PlayerSettings.SplashScreen.animationMode;
			var splashDrawMode = PlayerSettings.SplashScreen.drawMode;
			var splashLogos = PlayerSettings.SplashScreen.logos;

			// Override Player Settings for this build.
			EditorBuildSettingsScene[] buildScenes;
			if (SetBuildSettingsForWearableDemo(out buildScenes))
			{
				var sceneAssetPaths = buildScenes.Where(x => x.enabled).Select(x => x.path).ToArray();

				// Attempt to build the app
				var buildPlayerOptions = new BuildPlayerOptions
				{
					scenes = sceneAssetPaths,
					locationPathName = folderPath,
					target = EditorUserBuildSettings.activeBuildTarget
				};

				var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
				#if UNITY_2018_1_OR_NEWER
				if (buildReport.summary.result == BuildResult.Succeeded)
				#else
				if (string.IsNullOrEmpty(buildReport))
				#endif
				{
					Debug.LogFormat(BUILD_SUCCEEDED_MESSAGE, WEARABLE_DEMO_PRODUCT_NAME);

					if (IsAutoShowBuildOnSuccessEnabled())
					{
						ProjectTools.OpenFolder(folderPath);
					}
				}
				else
				{
					Debug.LogFormat(BUILD_FAILED_MESSAGE, WEARABLE_DEMO_PRODUCT_NAME, buildReport);
				}
			}
			else
			{
				Debug.LogErrorFormat(BUILD_SCENES_COULD_NOT_BE_FOUND, WEARABLE_DEMO_PRODUCT_NAME);
			}

			// Reset all PlayerSetting changes back to their original values.
			PlayerSettings.productName = originalProductName;
			PlayerSettings.bundleVersion = bundleVersion;
			PlayerSettings.SetApplicationIdentifier(buildTargetGroup, appId);
			PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, iconGroup);

			PlayerSettings.SplashScreen.show = showSplash;
			PlayerSettings.SplashScreen.unityLogoStyle = splashStyle;
			PlayerSettings.SplashScreen.backgroundColor = splashColor;
			PlayerSettings.SplashScreen.animationMode = splashAnimationMode;
			PlayerSettings.SplashScreen.drawMode = splashDrawMode;
			PlayerSettings.SplashScreen.logos = splashLogos;

			AssetDatabase.SaveAssets();
		}

		/// <summary>
		/// Ensure all settings for the Wearable Demo build are in place.
		/// (Parameterless version for headless build systems.)
		/// </summary>
		public static void SetBuildSettingsForWearableDemo()
		{
			EditorBuildSettingsScene[] buildScenes;
			SetBuildSettingsForWearableDemo(out buildScenes);

			#if UNITY_CLOUD_BUILD
			// Only in Unity Cloud Build do we want to override the native scene list
			EditorBuildSettings.scenes = buildScenes;
			Debug.LogFormat(BUILD_SCENES_SET_MESSAGE, WEARABLE_DEMO_PRODUCT_NAME);
			#endif
		}

		/// <summary>
		/// Ensure all settings for the Wearable Demo build are in place.
		/// </summary>
		private static bool SetBuildSettingsForWearableDemo(out EditorBuildSettingsScene[] buildScenes)
		{
			buildScenes = new EditorBuildSettingsScene[WEARABLE_DEMO_SCENE_GUIDS.Length];
			for (var i = 0; i < WEARABLE_DEMO_SCENE_GUIDS.Length; i++)
			{
				buildScenes[i] = new EditorBuildSettingsScene
				{
					path = AssetDatabase.GUIDToAssetPath(WEARABLE_DEMO_SCENE_GUIDS[i]),
					enabled = true
				};

				if (string.IsNullOrEmpty(buildScenes[i].path))
				{
					Debug.LogErrorFormat(CANNOT_BUILD_MISSING_SCENE_ERROR_MESSAGE, WEARABLE_DEMO_PRODUCT_NAME);
				}
			}

			var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

			PlayerSettings.productName = WEARABLE_DEMO_PRODUCT_NAME;
			PlayerSettings.bundleVersion = APP_VERSION;
			PlayerSettings.SetApplicationIdentifier(buildTargetGroup, WEARABLE_DEMO_APP_IDENTIFIER);
			TrySetAppIcons(WEARABLE_DEMO_ICON_GUID, buildTargetGroup);

			SetSplashSettings();

			AssetDatabase.SaveAssets();

			return buildScenes.Length > 0 &&
			       buildScenes.All(x => !string.IsNullOrEmpty(x.path) && x.enabled);
		}

		/// <summary>
		/// Returns true or false depending on whether the local Unity Editor can build
		/// the desired BuildTarget based on whether or not the player support has been
		/// installed.
		/// </summary>
		/// <param name="buildTarget"></param>
		/// <returns></returns>
		private static bool CanBuildTarget(BuildTarget buildTarget)
		{
			var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);

			#if UNITY_2018_1_OR_NEWER

			return BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget);

			#else

			try
			{
				// IsBuildTargetSupported is an internal method of BuildPipeline on 2017.4 so we must
				// use reflection in order to access it.
				const string BUILD_TARGET_SUPPORTED_METHOD_NAME = "IsBuildTargetSupported";
				var internalMethod = typeof(BuildPipeline).GetMethod(
					BUILD_TARGET_SUPPORTED_METHOD_NAME,
					BindingFlags.NonPublic | BindingFlags.Static);

				var result = internalMethod.Invoke(null, new object[] { buildTargetGroup, buildTarget });

				return (bool)result;
			}
			// Default to true if we cannot programmatically determine player support in the editor.
			catch (Exception e)
			{
				Debug.LogError(e);
				return true;
			}

			#endif
		}

		/// <summary>
		/// Get a build location from the user via a dialog box. If the path is valid, it will be saved in the
		/// user's preferences for use next time as a suggestion.
		/// </summary>
		/// <returns></returns>
		private static string GetBuildLocation(string productName)
		{
			// Get folder path from the user for the build
			var startFolder = string.Empty;
			if (EditorPrefs.HasKey(BUILD_LOCATION_PREFERENCE_KEY))
			{
				startFolder = EditorPrefs.GetString(BUILD_LOCATION_PREFERENCE_KEY);
			}

			var panelTitle = string.Format(FOLDER_PICKER_TITLE, productName);
			BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
			string folderPath;
			switch (activeTarget)
			{
				case BuildTarget.Android:
				{
					folderPath = EditorUtility.SaveFilePanel(panelTitle, startFolder, productName, "apk");
					break;
				}

				case BuildTarget.iOS:
				{
					folderPath = EditorUtility.SaveFolderPanel(panelTitle, startFolder, productName);
					break;
				}

				default:
				{
					folderPath = string.Empty;
					Debug.LogWarningFormat(WearableEditorConstants.BUILD_TOOLS_UNSUPPORTED_PLATFORM_WARNING, activeTarget);
					break;
				}
			}

			if (!string.IsNullOrEmpty(folderPath))
			{
				var directory = new DirectoryInfo(folderPath);
				if (directory.Parent != null)
				{
					var parentDirectory = directory.Parent;
					EditorPrefs.SetString(BUILD_LOCATION_PREFERENCE_KEY, parentDirectory.FullName);
				}
			}

			return folderPath;
		}

		/// <summary>
		/// Attempt to use an <see cref="Texture2D"/> identified by <see cref="string"/> <paramref name="iconGuid"/> to
		/// override the App Icon settings for <see cref="BuildTargetGroup"/> <paramref name="buildTargetGroup"/>.
		/// </summary>
		/// <param name="iconGuid"></param>
		/// <param name="buildTargetGroup"></param>
		private static void TrySetAppIcons(string iconGuid, BuildTargetGroup buildTargetGroup)
		{
			var iconPath = AssetDatabase.GUIDToAssetPath(iconGuid);
			if (string.IsNullOrEmpty(iconPath))
			{
				Debug.LogWarning(CANNOT_FIND_APP_ICON);
			}
			else
			{
				var iconSizes = PlayerSettings.GetIconSizesForTargetGroup(buildTargetGroup);
				var iconTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
				var newIconGroup = new Texture2D[iconSizes.Length];
				for (var i = 0; i < newIconGroup.Length; i++)
				{
					newIconGroup[i] = iconTexture;
				}

				PlayerSettings.SetIconsForTargetGroup(buildTargetGroup, newIconGroup);
			}
		}

		/// <summary>
		/// Sets the splash settings for the application in <see cref="PlayerSettings.SplashScreen"/>.
		/// </summary>
		private static void SetSplashSettings()
		{
			var boseLogoAssetPath = AssetDatabase.GUIDToAssetPath(BOSE_LOGO_ICON_GUID);
			var boseLogo = AssetDatabase.LoadAssetAtPath<Sprite>(boseLogoAssetPath);
			var splashScreenLogo = new PlayerSettings.SplashScreenLogo()
			{
				logo = boseLogo,
				duration = 2
			};

			var splashColor = Color.black;
			splashColor.a = 1f;

			PlayerSettings.SplashScreen.show = true;
			PlayerSettings.SplashScreen.unityLogoStyle = PlayerSettings.SplashScreen.UnityLogoStyle.LightOnDark;
			PlayerSettings.SplashScreen.backgroundColor = splashColor;
			PlayerSettings.SplashScreen.animationMode = PlayerSettings.SplashScreen.AnimationMode.Static;
			PlayerSettings.SplashScreen.drawMode = PlayerSettings.SplashScreen.DrawMode.UnityLogoBelow;
			PlayerSettings.SplashScreen.logos = new[]
			{
				splashScreenLogo
			};
		}

		/// <summary>
		/// Gets the user-preference to auto-show the demo build on success.
		/// </summary>
		/// <param name="isEnabled"></param>
		internal static bool IsAutoShowBuildOnSuccessEnabled()
		{
			return EditorPrefs.GetBool(WearableEditorConstants.SHOW_BUILD_ON_SUCCESS_KEY,
				WearableEditorConstants.DEFAULT_SHOW_BUILD_ON_SUCCESS_PREF);
		}

		/// <summary>
		/// Sets the user-preference to auto-show the demo build on success.
		/// </summary>
		/// <param name="isEnabled"></param>
		internal static void SetAutoShowBuildOnSuccessPreference(bool isEnabled)
		{
			EditorPrefs.SetBool(WearableEditorConstants.SHOW_BUILD_ON_SUCCESS_KEY, isEnabled);
		}
	}
}
