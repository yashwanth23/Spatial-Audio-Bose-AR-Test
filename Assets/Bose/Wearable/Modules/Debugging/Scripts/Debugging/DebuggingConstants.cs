using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Constant fields for the debug panel.
	/// </summary>
	internal static class DebuggingConstants
	{
		// Format strings
		public const string FRAMES_PER_SECOND_FORMAT = "{0:0.0}";
		public const string DATA_COMPONENT_FORMAT_POSITIVE = "<color=#999999>{0}</color>{1: 000.000}";
		public const string DATA_COMPONENT_FORMAT_NEGATIVE = "<color=#999999>{0}</color>{1:-000.000}";
		public const string EULER_DATA_COMPONENT_FORMAT_POSITIVE = "<color=#999999>{0}</color>{1: 000.00°}";
		public const string EULER_DATA_COMPONENT_FORMAT_NEGATIVE = "<color=#999999>{0}</color>{1:-000.00°}";
		public const string QUATERNION_COMPONENT_FORMAT_POSITIVE = "<color=#999999>{0}</color>{1: 0.00}";
		public const string QUATERNION_COMPONENT_FORMAT_NEGATIVE = "<color=#999999>{0}</color>{1:-0.00}";
		public const string UNCERTAINTY_FORMAT = "± {0:00.00}°";
		public const string ROTATION_SOURCE_FORMAT = "{0} {1}";
		public const string SECONDS_FORMAT = "{0:0.000}s";
		public const string EULER_UNITS = "| deg";

		public const string X_FIELD = "X";
		public const string Y_FIELD = "Y";
		public const string Z_FIELD = "Z";
		public const string W_FIELD = "W";

		// UI
		public const string RESET_OVERRIDE_BUTTON_TEXT = "RESET OVERRIDE";
		public const string OVERRIDE_CONFIG_BUTTON_TEXT = "OVERRIDE CONFIG";

		public const string RESET_OVERRIDE_CONFIG_ON_HIDE_TOOLTIP =
			"When this is true, closing the DebugUIPanel will automatically stop overriding the devie config. " +
			"Otherwise the user will need to click the \"" + RESET_OVERRIDE_BUTTON_TEXT + "\" button to do " +
			"so.";

		public static readonly WearableUIColorPalette.Style EMPTY_STYLE;

		static DebuggingConstants()
		{
			EMPTY_STYLE = new WearableUIColorPalette.Style()
			{
				textColor = Color.magenta,
				elementColor = Color.magenta
			};
		}
	}
}
