#if UNITY_IOS

using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	internal sealed class XcodePreBuildProcessor
		#if UNITY_2018_1_OR_NEWER
		: IPreprocessBuildWithReport
		#else
        : IPreprocessBuild
        #endif
	{
		/// <summary>
		/// The architecture of the build.
		/// </summary>
		private enum Architecture
		{
			ARMv7,
			ARM64,
			Universal
		}

		public int callbackOrder
		{
			get { return WearableEditorConstants.XCODE_PRE_BUILD_PROCESSOR_ORDER; }
		}

		#if UNITY_2018_1_OR_NEWER
		public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
		{
			Process();
		}
		#else
		public void OnPreprocessBuild(BuildTarget target, string path)
		{
			Process();
		}
        #endif

		private void Process()
		{
			// Check the architecture and if not Arm64 log an error to the console.
			var arch = (Architecture)PlayerSettings.GetArchitecture(BuildTargetGroup.iOS);
			if (arch != Architecture.ARM64)
			{
				// Set the Project to ARM64 for iOS.
				PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, (int)Architecture.ARM64);

				Debug.LogWarningFormat(WearableEditorConstants.ARCHITECTURE_ALTERATION_WARNING_WITH_MESSAGE, arch);
			}

			// Make sure the target iOS version is at or above the minimum.
			float targetOSVersion;
			if (float.TryParse(PlayerSettings.iOS.targetOSVersionString, out targetOSVersion))
			{
				if (targetOSVersion < WearableConstants.MINIMUM_COMPILABLE_IOS_VERSION)
				{
					Debug.LogWarningFormat(
						WearableEditorConstants.OS_VERSION_ALTERATION_WARNING_WITH_MESSAGE,
						WearableConstants.MINIMUM_COMPILABLE_IOS_VERSION,
						targetOSVersion.ToString("0.0"));

					targetOSVersion = WearableConstants.MINIMUM_COMPILABLE_IOS_VERSION;

					PlayerSettings.iOS.targetOSVersionString = targetOSVersion.ToString("0.0");
				}

				// If the target iOS version is in the minimum compilable version, warn the user.
				if (targetOSVersion >= WearableConstants.MINIMUM_COMPILABLE_IOS_VERSION &&
				   targetOSVersion < WearableConstants.MINIMUM_SUPPORTED_IOS_VERSION)
				{
					Debug.LogWarningFormat(
						WearableEditorConstants.OS_VERSION_COMPILE_WARNING_WITH_MESSAGE,
						WearableConstants.MINIMUM_COMPILABLE_IOS_VERSION,
						WearableConstants.MINIMUM_SUPPORTED_IOS_VERSION);
				}
			}

			// Make sure that the app is set to use BLE accessories.
			var backgroundBehavior = PlayerSettings.iOS.appInBackgroundBehavior;
			var backgroundModes = PlayerSettings.iOS.backgroundModes;
			if (backgroundBehavior != iOSAppInBackgroundBehavior.Custom ||
				(backgroundModes & iOSBackgroundMode.BluetoothCentral) == 0)
			{
				PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
				PlayerSettings.iOS.backgroundModes |= iOSBackgroundMode.BluetoothCentral;

				Debug.LogWarning(WearableEditorConstants.OS_BLUETOOTH_ALTERATION_WARNING);
			}
		}
	}
}

#endif
