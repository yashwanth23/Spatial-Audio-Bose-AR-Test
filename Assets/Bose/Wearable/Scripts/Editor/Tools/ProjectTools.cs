using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// Helper methods for dealing with project assets
	/// </summary>
	internal static class ProjectTools
	{
		// File Verbs
		private const string FILE_VERB_OPEN = "Open";

		// Project Assets
		public const string PDF_ASSET_GUID = "9cd3f8ee5111ed349b0756c5c07e176b";
		public const string LICENSE_ASSET_GUID = "931d8b671deb54ab49add68f6ded3759";

		/// <summary>
		/// Opens the local documentation PDF if present.
		/// </summary>
		public static void OpenLocalHelpDocumentation()
		{
			var unityPdfAssetPath = AssetDatabase.GUIDToAssetPath(PDF_ASSET_GUID);
			if (string.IsNullOrEmpty(unityPdfAssetPath))
			{
				Debug.LogWarningFormat(WearableEditorConstants.LOCAL_PDF_NOT_FOUND, PDF_ASSET_GUID);
				return;
			}

			OpenFile(GetLocalDocumentationPdfFilePath());
		}

		/// <summary>
		/// Returns true if the local PDF documentation is present, otherwise false.
		/// </summary>
		public static bool IsLocalPdfDocumentationPresent()
		{
			var unityPdfAssetPath = AssetDatabase.GUIDToAssetPath(PDF_ASSET_GUID);
			return AssetDatabase.LoadMainAssetAtPath(unityPdfAssetPath) != null;
		}

		/// <summary>
		/// Returns true if the local EULA is present, otherwise false.
		/// </summary>
		public static bool IsLocalLicensePresent()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(LICENSE_ASSET_GUID);
			return AssetDatabase.LoadMainAssetAtPath(assetPath) != null;
		}

		/// <summary>
		/// Attempts to open the local license file included with the SDK.
		/// </summary>
		public static void OpenLocalLicense()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(LICENSE_ASSET_GUID);
			if (string.IsNullOrEmpty(assetPath))
			{
				Debug.LogWarningFormat(WearableEditorConstants.LOCAL_LICENSE_NOT_FOUND, LICENSE_ASSET_GUID);
				return;
			}

			OpenFile(GetLocalLicenseFilePath());
		}

		/// <summary>
		/// Opens a file using the OS default program at <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath"></param>
		public static void OpenFile(string filePath)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = filePath,
				Verb = FILE_VERB_OPEN
			});
		}

		/// <summary>
		/// Opens an OS-window to select a given file/folder.
		/// </summary>
		/// <param name="path"></param>
		public static void OpenFolder(string path)
		{
			var isDir = (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
			var targetPath = isDir ? path : Path.GetDirectoryName(path);

			Process.Start(new ProcessStartInfo
			{
				FileName = targetPath,
				Verb = FILE_VERB_OPEN
			});
		}

		/// <summary>
		/// Returns a path to the local documentation PDF.
		/// </summary>
		public static string GetLocalDocumentationPdfFilePath()
		{
			var unityPdfAssetPath = AssetDatabase.GUIDToAssetPath(PDF_ASSET_GUID);
			return Path.Combine(GetProjectPath(), unityPdfAssetPath);
		}

		/// <summary>
		/// Returns a path to the local EULA.
		/// </summary>
		public static string GetLocalLicenseFilePath()
		{
			var assetPath = AssetDatabase.GUIDToAssetPath(LICENSE_ASSET_GUID);
			return Path.Combine(GetProjectPath(), assetPath);
		}

		/// <summary>
		/// Returns a path to the Unity project folder.
		/// </summary>
		public static string GetProjectPath()
		{
			return Application.dataPath.Substring(0, Application.dataPath.Length - 6);
		}
	}
}
