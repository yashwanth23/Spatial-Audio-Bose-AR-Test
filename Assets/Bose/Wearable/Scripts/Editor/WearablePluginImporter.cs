using System;
using System.Linq;
using UnityEditor;

// ReSharper disable InconsistentNaming

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// Responsible for setting the import properties for all plugins to their proper values.
	/// </summary>
	internal sealed class WearablePluginImporter
	{
		/// <summary>
		/// Represents the OS of a Unity Editor; this enum is made to match the platform names used by the
		/// <see cref="PluginImporter.SetEditorData(string, string)"/> and
		/// <see cref="PluginImporter.GetEditorData(string)"/> methods.
		/// </summary>
		private enum EditorOS
		{
			None,
			OSX,
			Windows,
			Linux
		}

		// Editor Data Keys
		private const string OS_KEY = "OS";
		private const string CPU_KEY = "CPU";

		// Editor Data Values
		private const string CPU_VALUE = "AnyCPU";

		/// <summary>
		/// An array of all non-obsolete build targets.
		/// </summary>
		private static readonly BuildTarget[] ALL_BUILD_TARGETS;

		static WearablePluginImporter()
		{
			var buildTargets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
			ALL_BUILD_TARGETS = buildTargets.Where(x => !ReflectionTools.IsObsolete(x) &&
			                                            x != BuildTarget.NoTarget).ToArray();

			AssemblyReloadEvents.afterAssemblyReload += ApplyPluginSettings;
		}

		/// <summary>
		/// Applies desired settings for plugins included with the SDK if it finds that any of the settings
		/// don't match the desired ones.
		/// </summary>
		[InitializeOnLoadMethod]
		private static void ApplyPluginSettings()
		{
			// Setup Android Plugins
			foreach (var pluginGUID in WearablePluginConstants.ANDROID_PLUGIN_GUIDS)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(pluginGUID);
				var pluginImporter = AssetImporter.GetAtPath(assetPath) as PluginImporter;

				if (pluginImporter != null &&
				    !HasCorrectImportSettings(
					    pluginImporter,
					    isCompatibleWithAnyPlatform:false,
					    isCompatibleWithEditor:false,
						editorPlatform:EditorOS.None,
						allowedRuntimePlatforms:BuildTarget.Android))
				{
					ExcludeFromAllBuildTargets(pluginImporter);

					pluginImporter.SetCompatibleWithEditor(false);
					pluginImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
					pluginImporter.SaveAndReimport();
				}
			}

			// Setup iOS Plugins
			foreach (var pluginGUID in WearablePluginConstants.IOS_PLUGIN_GUIDS)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(pluginGUID);
				var pluginImporter = AssetImporter.GetAtPath(assetPath) as PluginImporter;

				if (pluginImporter != null &&
				    !HasCorrectImportSettings(
					    pluginImporter,
					    isCompatibleWithAnyPlatform: false,
					    isCompatibleWithEditor: false,
					    editorPlatform: EditorOS.None,
						allowedRuntimePlatforms: BuildTarget.iOS))
				{
					ExcludeFromAllBuildTargets(pluginImporter);

					pluginImporter.SetCompatibleWithEditor(false);
					pluginImporter.SetCompatibleWithPlatform(BuildTarget.iOS, true);
					pluginImporter.SaveAndReimport();
				}
			}

			// Setup macOS Plugins
			foreach (var pluginGUID in WearablePluginConstants.MAC_PLUGIN_GUIDS)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(pluginGUID);
				var pluginImporter = AssetImporter.GetAtPath(assetPath) as PluginImporter;

				if (pluginImporter != null &&
				    !HasCorrectImportSettings(
					    pluginImporter,
					    isCompatibleWithAnyPlatform: false,
					    isCompatibleWithEditor: true,
					    editorPlatform: EditorOS.OSX))
				{
					ExcludeFromAllBuildTargets(pluginImporter);

					pluginImporter.SetCompatibleWithEditor(true);
					pluginImporter.SetEditorData(OS_KEY, EditorOS.OSX.ToString());
					pluginImporter.SetEditorData(CPU_KEY, CPU_VALUE);
					pluginImporter.SaveAndReimport();
				}
			}

			// Setup Window Plugins
			foreach (var pluginGUID in WearablePluginConstants.WINDOWS_PLUGIN_GUIDS)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(pluginGUID);
				var pluginImporter = AssetImporter.GetAtPath(assetPath) as PluginImporter;

				if (pluginImporter != null &&
				    !HasCorrectImportSettings(
					    pluginImporter,
					    isCompatibleWithAnyPlatform: false,
					    isCompatibleWithEditor: true,
					    editorPlatform: EditorOS.Windows))
				{
					ExcludeFromAllBuildTargets(pluginImporter);

					pluginImporter.SetCompatibleWithEditor(true);
					pluginImporter.SetEditorData(OS_KEY, EditorOS.Windows.ToString());
					pluginImporter.SetEditorData(CPU_KEY, CPU_VALUE);
					pluginImporter.SaveAndReimport();
				}
			}
		}

		/// <summary>
		/// Returns true if the plugin importer already has the correct settings, otherwise returns false. If
		/// false is returned, the plugin settings should be reset to their expected values.
		/// </summary>
		/// <param name="pluginImporter"></param>
		/// <param name="isCompatibleWithAnyPlatform"></param>
		/// <param name="isCompatibleWithEditor"></param>
		/// <param name="editorPlatform"></param>
		/// <param name="allowedRuntimePlatforms"></param>
		/// <returns></returns>
		private static bool HasCorrectImportSettings(
			PluginImporter pluginImporter,
			bool isCompatibleWithAnyPlatform,
			bool isCompatibleWithEditor,
			EditorOS editorPlatform,
			params BuildTarget[] allowedRuntimePlatforms)
		{
			// Return false if our platform compatibility doesn't match
			if (pluginImporter.GetCompatibleWithAnyPlatform() != isCompatibleWithAnyPlatform)
			{
				return false;
			}

			// Return false if our editor compatibility doesn't match
			if (pluginImporter.GetCompatibleWithEditor() != isCompatibleWithEditor)
			{
				return false;
			}

			// If the plugin is compatible with the editor and not set to the proper platform or to the proper
			// CPU level, return false.
			if (pluginImporter.GetCompatibleWithEditor() && isCompatibleWithEditor)
			{
				var editorPlatformValue = pluginImporter.GetEditorData(OS_KEY);
				var editorOS = (EditorOS)Enum.Parse(typeof(EditorOS), editorPlatformValue);
				if (editorOS != editorPlatform)
				{
					return false;
				}

				if (pluginImporter.GetEditorData(CPU_KEY) != CPU_VALUE)
				{
					return false;
				}
			}

			// Check compatibility with each platform and verify that we have correct setting for each.
			foreach (var buildTarget in ALL_BUILD_TARGETS)
			{
				var shouldBeCompatible = allowedRuntimePlatforms.Contains(buildTarget);
				var isCompatible = pluginImporter.GetCompatibleWithPlatform(buildTarget);

				if (shouldBeCompatible && !isCompatible)
				{
					return false;
				}
				else if (!shouldBeCompatible && isCompatible)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Update the <see cref="PluginImporter"/> <paramref name="pluginImporter"/> to be explicitly
		/// incompatible with any build target.
		/// </summary>
		/// <param name="pluginImporter"></param>
		private static void ExcludeFromAllBuildTargets(PluginImporter pluginImporter)
		{
			pluginImporter.SetCompatibleWithAnyPlatform(false);
			foreach (var buildTarget in ALL_BUILD_TARGETS)
			{
				pluginImporter.SetCompatibleWithPlatform(buildTarget, false);
			}
		}
	}
}
