#if UNITY_ANDROID

using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	internal sealed class AndroidPreBuildProcessor
		#if UNITY_2018_1_OR_NEWER
		: IPreprocessBuildWithReport
		#else
        : IPreprocessBuild
        #endif
	{
		public int callbackOrder
		{
			get { return WearableEditorConstants.ANDROID_PRE_BUILD_PROCESSOR_ORDER; }
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
			// If below our minimum SDK compile version, force it to that version.
			var minSdkVersion = (int)PlayerSettings.Android.minSdkVersion;
			if (minSdkVersion < WearableConstants.MINIMUM_COMPILABLE_ANDROID_VERSION)
			{
				Debug.LogWarningFormat(
					WearableEditorConstants.ANDROID_VERSION_ALTERATION_WARNING_WITH_MESSAGE,
					WearableConstants.MINIMUM_COMPILABLE_ANDROID_VERSION,
					minSdkVersion);

				minSdkVersion = WearableConstants.MINIMUM_COMPILABLE_ANDROID_VERSION;
				PlayerSettings.Android.minSdkVersion = (AndroidSdkVersions)minSdkVersion;
			}

			// If the target Android SDK version is in the minimum compilable version, warn the user.
			if (minSdkVersion >= WearableConstants.MINIMUM_COMPILABLE_ANDROID_VERSION &&
				minSdkVersion < WearableConstants.MINIMUM_SUPPORTED_ANDROID_VERSION)
			{
				Debug.LogWarningFormat(
					WearableEditorConstants.ANDROID_VERSION_COMPILE_WARNING_WITH_MESSAGE,
					WearableConstants.MINIMUM_COMPILABLE_ANDROID_VERSION,
					WearableConstants.MINIMUM_SUPPORTED_ANDROID_VERSION);
			}
		}
	}
}

#endif
