using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Editor
{
	/// <summary>
	/// A helper class for showing the locale keys in the project from <see cref="LocaleData"/> instances for
	/// user selection.
	/// </summary>
	internal static class LocaleKeyDropDownMenu
	{
		/// <summary>
		/// Lazy-loaded <see cref="LocaleKeyCache"/>; upon access, the latest <see cref="LocaleData"/> instances
		/// will be populated ensuring the most up-to-date key/value pairs.
		/// </summary>
		private static LocaleKeyCache LocaleKeyCache
		{
			get
			{
				if (_localeKeyCache == null)
				{
					_localeKeyCache = new LocaleKeyCache();
				}

				var localeData = LocaleTools.GetAllLocaleData();
				_localeKeyCache.Set(localeData);

				return _localeKeyCache;
			}
		}

		private static LocaleKeyCache _localeKeyCache;

		public static void ShowAsDropdown(
			Rect position,
			string currentValue,
			GenericMenu.MenuFunction2 onLocaleKeySelected)
		{
			var menu = new GenericMenu();
			var uniqueKeys = LocaleKeyCache.UniqueKeys;
			for (var i = 0; i < uniqueKeys.Count; i++)
			{
				menu.AddItem(new GUIContent(uniqueKeys[i]), uniqueKeys[i] == currentValue, onLocaleKeySelected, uniqueKeys[i]);
			}
			menu.DropDown(position);
		}
	}
}
