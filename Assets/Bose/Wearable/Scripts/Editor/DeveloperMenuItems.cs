using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// Menu items for developers for Bose Wearable resources
	/// </summary>
	internal static class DeveloperMenuItems
	{
		// Build Menu Items
		private const string BUILD_WEARABLE_DEMO_MENU_ITEM = "Tools/Bose Wearable/Build Wearable Demo";

		// Developer Help Menu Items
		private const string DEVELOPER_PORTAL_MENU_ITEM = "Tools/Bose Wearable/Help/Developer Portal";
		private const string DEVELOPER_FORUMS_MENU_ITEM = "Tools/Bose Wearable/Help/Forums";
		private const string DEVELOPER_DOCUMENTATION_MENU_ITEM = "Tools/Bose Wearable/Help/Documentation";
		private const string DEVELOPER_REPORT_BUG_MENU_ITEM = "Tools/Bose Wearable/Help/Report a Bug";
		private const string DEVELOPER_ABOUT_MENU_ITEM = "Tools/Bose Wearable/Help/About";
		private const string DEVELOPER_LICENSE_MENU_ITEM = "Tools/Bose Wearable/Help/License";

		// Menu Item Priority
		private const int BUILD_MENU_ITEM_PRIORITY = 100;
		private const int HELP_MENU_ITEM_PRIORITY = 110;

		// Links
		private const string FORUM_LINK = "https://bosedevs.bose.com/categories/bose-ar-unity-plugin";
		private const string DOCUMENTATION_LINK =
			"https://developer.bose.com/guides/bose-ar/getting-started-unity";
		private const string PORTAL_LINK = "https://developer.bose.com/bose-ar";
		private const string REPORT_A_BUG_LINK =
			"mailto:help@bosear.zendesk.com?subject=Bose%20AR%20Unity%20SDK%20Bug%20Report";
		private const string LICENSE_LINK = 
			"https://developer.bose.com/bose-ar-developer-beta-sdk-license-agreement";

		[MenuItem(BUILD_WEARABLE_DEMO_MENU_ITEM, priority = BUILD_MENU_ITEM_PRIORITY)]
		private static void BuildWearableDemo()
		{
			BuildTools.BuildWearableDemo();
		}

		[MenuItem(BUILD_WEARABLE_DEMO_MENU_ITEM, validate = true)]
		private static bool IsSupportedPlatformForWearableDemo()
		{
			BuildTarget activeTarget = EditorUserBuildSettings.activeBuildTarget;
			return activeTarget == BuildTarget.iOS || activeTarget == BuildTarget.Android;
		}

		[MenuItem(DEVELOPER_PORTAL_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchBoseWearablePortal()
		{
			Application.OpenURL(PORTAL_LINK);
		}

		[MenuItem(DEVELOPER_DOCUMENTATION_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchBoseWearableDocumentation()
		{
			if (ProjectTools.IsLocalPdfDocumentationPresent())
			{
				ProjectTools.OpenLocalHelpDocumentation();
			}
			else
			{
				Application.OpenURL(DOCUMENTATION_LINK);
			}
		}

		[MenuItem(DEVELOPER_FORUMS_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchBoseWearableForum()
		{
			Application.OpenURL(FORUM_LINK);
		}

		[MenuItem(DEVELOPER_REPORT_BUG_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchBoseWearableReportABug()
		{
			Application.OpenURL(REPORT_A_BUG_LINK);
		}

		[MenuItem(DEVELOPER_ABOUT_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchAboutWindow()
		{
			DeveloperAboutWindow.LaunchWindow();
		}

		[MenuItem(DEVELOPER_LICENSE_MENU_ITEM, priority = HELP_MENU_ITEM_PRIORITY)]
		private static void LaunchBoseWearableLicense()
		{
			if (ProjectTools.IsLocalLicensePresent())
			{
				ProjectTools.OpenLocalLicense();
			}
			else
			{
				Application.OpenURL(LICENSE_LINK);
			}
		}
	}
}
