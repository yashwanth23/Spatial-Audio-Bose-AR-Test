using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="WearableUIColorPalette"/> is an asset that allows for user-customization of the UI colors for
	/// several states on the <see cref="DebugUIPanel"/>'s controls.
	/// </summary>
	internal sealed class WearableUIColorPalette : ScriptableObject
	{
		[Serializable]
		internal class Style
		{
			public Color textColor;
			public Color elementColor;
		}

		[Serializable]
		internal class StyleMapping
		{
			public WearableUIColorStyleId StyleId
			{
				get { return _styleId; }
			}

			#pragma warning disable 0649

			[SerializeField]
			private WearableUIColorStyleId _styleId;

			/// <summary>
			/// The <see cref="Color"/> for an element and active.
			/// </summary>
			public Style activeStyle;

			/// <summary>
			/// The <see cref="Color"/> for an element when active and overriden.
			/// </summary>
			public Style overrideActiveStyle;

			#pragma warning restore 0649
		}

		/// <summary>
		/// The <see cref="Color"/> for child elements that are inactive.
		/// </summary>
		public Style InactiveChildElementStyle
		{
			get { return _inactiveChildElementStyle; }
		}

		/// <summary>
		/// The <see cref="Style"/> for child elements that have been overriden as inactive.
		/// </summary>
		public Style InactiveOverriddenChildElementStyle
		{
			get { return _inactiveOverridenChildElementStyle; }
		}

		/// <summary>
		/// The <see cref="Color"/> for title elements that are inactive.
		/// </summary>
		public Style InactiveTitleElementStyle
		{
			get { return _inactiveTitleElementStyle; }
		}

		/// <summary>
		/// The <see cref="Color"/> for inactive data text.
		/// </summary>
		public Color InactiveUncertaintyTextColor
		{
			get { return _inactiveUncertaintyTextColor; }
		}

		/// <summary>
		/// The <see cref="Color"/> for inactive data text.
		/// </summary>
		public Color ActiveDataTextColor
		{
			get { return _activeDataTextColor; }
		}

		/// <summary>
		/// The <see cref="Color"/> for inactive data text.
		/// </summary>
		public Color InactiveDataTextColor
		{
			get { return _inactiveDataTextColor; }
		}

		#pragma warning disable 0649

		[SerializeField]
		private WearableUIColorStyleId _styleId;

		[SerializeField]
		private List<StyleMapping> _customizedChildElementStyles;

		[SerializeField]
		private Style _inactiveChildElementStyle;

		[SerializeField]
		private Style _inactiveOverridenChildElementStyle;

		[SerializeField]
		private Style _inactiveTitleElementStyle;

		[SerializeField]
		private Color _inactiveUncertaintyTextColor;

		[SerializeField]
		private Color _inactiveDataTextColor;

		[SerializeField]
		private Color _activeDataTextColor;

		#pragma warning restore 0649

		/// <summary>
		/// Returns the current active color style for the Debug UI based on assigned <see cref="WearableUIColorStyleId"/>
		/// on this <see cref="WearableUIColorPalette"/> instance.
		/// </summary>
		/// <returns></returns>
		public Style GetCustomizedActiveStyle()
		{
			Assert.IsNotNull(_styleId);
			Assert.IsTrue(_customizedChildElementStyles != null && _customizedChildElementStyles.Count > 0);

			for (var i = 0; i < _customizedChildElementStyles.Count; i++)
			{
				if (_customizedChildElementStyles[i].StyleId != _styleId)
				{
					continue;
				}

				return _customizedChildElementStyles[i].activeStyle;
			}

			return DebuggingConstants.EMPTY_STYLE;
		}

		/// <summary>
		/// Returns the current override active color style for the Debug UI based on assigned <see cref="WearableUIColorStyleId"/>
		/// on this <see cref="WearableUIColorPalette"/> instance.
		/// </summary>
		/// <returns></returns>
		public Style GetCustomizedOverrideActiveStyle()
		{
			Assert.IsNotNull(_styleId);
			Assert.IsTrue(_customizedChildElementStyles != null && _customizedChildElementStyles.Count > 0);

			for (var i = 0; i < _customizedChildElementStyles.Count; i++)
			{
				if (_customizedChildElementStyles[i].StyleId != _styleId)
				{
					continue;
				}

				return _customizedChildElementStyles[i].overrideActiveStyle;
			}

			return DebuggingConstants.EMPTY_STYLE;
		}
	}
}
