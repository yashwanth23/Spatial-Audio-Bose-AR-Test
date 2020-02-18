#if UNITY_IOS

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// XcodeBuildProcessor links all of the necessary binaries and frameworks, sets search paths, and otherwise helps to
	/// automate setting up the Unity-generated Xcode project to be able to build to device without additional customization.
	/// </summary>
	internal sealed class XcodePostBuildProcessor
		#if UNITY_2018_1_OR_NEWER
        : IPostprocessBuildWithReport
		#else
		: IPostprocessBuild
		#endif
	{
		public int callbackOrder
		{
			get { return WearableEditorConstants.XCODE_POST_BUILD_PROCESSOR_ORDER; }
		}

		private PBXProject _project;
		private string _appGuid;

		#pragma warning disable 0414
		private string _unityFrameworkGuid;
		#pragma warning restore 0414

		#if UNITY_2018_1_OR_NEWER
        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Process(report.summary.outputPath);
        }
		#else
		public void OnPostprocessBuild(UnityEditor.BuildTarget target, string path)
		{
			Process(path);
		}
		#endif

		private void Process(string path)
		{
			// Read the project contents from file
			var pbxProjectPath = Path.Combine(path, WearableEditorConstants.XCODE_PROJECT_NAME);
			_project = new PBXProject();
			_project.ReadFromFile(pbxProjectPath);

			#if UNITY_2019_3_OR_NEWER
            _appGuid = _project.GetUnityMainTargetGuid();
            _unityFrameworkGuid = _project.GetUnityFrameworkTargetGuid();
			#else
			_appGuid = _project.TargetGuidByName(PBXProject.GetUnityTargetName());
			_unityFrameworkGuid = string.Empty;
			#endif

			// Link Frameworks
			AddFrameworksToEmbeddedBinaries();

			// Add Empty Swift File
			EnableEmbeddedSwift();

			// Ensure Info.plist contains message for Bluetooth usage.
			AddBluetoothMessageToInfoPlist(path);

			// Finalize the changes by writing them back to file.
			_project.WriteToFile(pbxProjectPath);
		}

		/// <summary>
		/// For each framework, get the filename and add that framework to embedded binaries section.
		/// </summary>
		private void AddFrameworksToEmbeddedBinaries()
		{
			var frameworkRelativePath =
				AssetDatabase.GUIDToAssetPath(WearableEditorConstants.IOS_NATIVE_ARTIFACTS_GUID);

			const string ASSETS_ROOT = "Assets/";

			var frameworkRelativePathWithoutAssets = frameworkRelativePath.Substring(
				ASSETS_ROOT.Length,
				frameworkRelativePath.Length - ASSETS_ROOT.Length);

			var frameworkFullPath = Path.Combine(ProjectTools.GetProjectPath(), frameworkRelativePath);
			var frameworks = Directory.GetDirectories(frameworkFullPath, WearableEditorConstants.FRAMEWORK_FILE_FILTER)
				.Select(Path.GetFileName)
				.ToArray();

			for (var i = 0; i < frameworks.Length; i++)
			{
				AddFrameworkToEmbeddedBinaries(frameworkRelativePathWithoutAssets, frameworks[i]);
			}
		}

		/// <summary>
		/// Add framework to the embedded binaries section.
		/// </summary>
		/// <param name="frameworkPath"></param>
		/// <param name="frameworkName"></param>
		private void AddFrameworkToEmbeddedBinaries(string frameworkPath, string frameworkName)
		{
			// Framework path was broken in 2018.3.0->2018.3.3
			// From 2018.3.4 Release notes:
			// *  iOS: Fixed iOS Frameworks location is ignored when building Xcode project. (1108970)
			#if UNITY_2018_3_0 || UNITY_2018_3_1 || UNITY_2018_3_2 || UNITY_2018_3_3

			var projectFrameworkPath = Path.Combine(WearableEditorConstants.XCODE_PROJECT_FRAMEWORKS_PATH, frameworkName);

			#else

			var projectFrameworkPath = Path.Combine(WearableEditorConstants.XCODE_PROJECT_FRAMEWORKS_PATH,
													Path.Combine(frameworkPath, frameworkName));
			#endif

			// Get the GUID of the framework that Unity will automatically add to the xcode project
			var frameworkGuid = _project.FindFileGuidByProjectPath(projectFrameworkPath);

			// Add framework as embedded binary
			_project.AddFileToEmbedFrameworks(_appGuid, frameworkGuid);
		}

		/// <summary>
		/// Enables the compilation of Swift in embedded code by setting several build properties.
		/// </summary>
		private void EnableEmbeddedSwift()
		{
			// Add several build properties that help
			_project.SetBuildProperty(
				_appGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_MODULES_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_ENABLE_VALUE);

			_project.AddBuildProperty(
				_appGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SEARCH_PATHS_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SEARCH_PATHS_VALUE);


			_project.SetBuildProperty(
				_appGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_VERSION_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_VERSION_VALUE);

			_project.SetBuildProperty(
				_appGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_OPTIMIZATION_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_OPTIMIZATION_VALUE);

			#if UNITY_2019_3_OR_NEWER
			_project.SetBuildProperty(
				_unityFrameworkGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_MODULES_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_ENABLE_VALUE);

			_project.SetBuildProperty(
				_unityFrameworkGuid,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_VERSION_KEY,
				WearableEditorConstants.XCODE_BUILD_PROPERTY_SWIFT_VERSION_VALUE);

			#endif
		}

		/// <summary>
		/// Ensures that the project's Info.plist contains a message that describes the Bluetooth usage for app submission.
		/// </summary>
		private void AddBluetoothMessageToInfoPlist(string projectPath)
		{
			string pListPath =
				Path.GetFullPath(Path.Combine(projectPath, WearableEditorConstants.XCODE_INFO_PLIST_RELATIVE_PATH));

			PlistDocument infoPlist = new PlistDocument();
			infoPlist.ReadFromFile(pListPath);

			PlistElementDict infoDict = infoPlist.root;
			// Set a valid description for the use case of the Bluetooth devices if none is set. Otherwise we assume
			// the user has set one and we don't want to overwrite it. Without this message, Apple may reject your
			// app submission.
			bool updatedPlist = false;

			if (!infoDict.values.ContainsKey(WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_PERIPHERAL_KEY))
			{
				infoDict.SetString(
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_PERIPHERAL_KEY,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_MESSAGE);

				Debug.LogWarningFormat(
					WearableEditorConstants.XCODE_INFO_PLIST_ALTERATION_WARNING_WITH_MESSAGE,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_PERIPHERAL_KEY,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_MESSAGE);

				updatedPlist = true;
			}

			if (!infoDict.values.ContainsKey(WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_ALWAYS_KEY))
			{
				infoDict.SetString(
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_ALWAYS_KEY,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_MESSAGE);

				Debug.LogWarningFormat(
					WearableEditorConstants.XCODE_INFO_PLIST_ALTERATION_WARNING_WITH_MESSAGE,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_ALWAYS_KEY,
					WearableEditorConstants.XCODE_INFO_PLIST_BLUETOOTH_MESSAGE);

				updatedPlist = true;
			}

			if (updatedPlist)
			{
				infoPlist.WriteToFile(pListPath);
			}
		}
	}
}
#endif
