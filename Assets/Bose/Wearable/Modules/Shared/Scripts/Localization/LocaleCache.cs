using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bose.Wearable
{
	/// <summary>
	/// A global cache for localization data loaded at runtime.
	/// </summary>
	public static class LocaleCache
	{
		/// <summary>
		/// Invoked when the system language has been changed.
		/// </summary>
		public static event Action LanguagePreferenceChanged;

		/// <summary>
		/// A lazy-loaded lookup of <see cref="LocaleData"/> instances.
		/// </summary>
		internal static Dictionary<SystemLanguage, LocaleData> LanguageToLocaleLookup
		{
			get
			{
				SetupLocaleCache();

				return _languageToLocaleLookup;
			}
		}

		private static bool _isSetup;
		private static Dictionary<SystemLanguage, LocaleData> _languageToLocaleLookup;
		private static List<SystemLanguage> _supportedLanguages;

		/// <summary>
		/// Forces the cache to be flushed and setup again.
		/// </summary>
		internal static void ForceSetupLocaleCache()
		{
			_isSetup = false;
			SetupLocaleCache();
		}

		/// <summary>
		/// Initializes the global cache of <see cref="LocaleData"/> instances.
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		internal static void SetupLocaleCache()
		{
			if (_isSetup)
			{
				return;
			}

			if (_languageToLocaleLookup == null)
			{
				_languageToLocaleLookup = new Dictionary<SystemLanguage, LocaleData>();
			}
			else
			{
				_languageToLocaleLookup.Clear();
			}

			// Get all LocaleData instances, set them up, and add them to a lookup based on SystemLanguage.
			var localeData = LocaleTools.GetAllLocaleData();
			for (var i = 0; i < localeData.Length; i++)
			{
				// Force setup each instance as subsequent play mode test runs and play mode runs in the editor
				// will not cleanup local non-serialized fields in ScriptableObjects
				var ld = localeData[i];
				ld.Setup(forceSetup:true);

				for (var j = 0; j < ld.SupportedLanguages.Length; j++)
				{
					if (_languageToLocaleLookup.ContainsKey(ld.SupportedLanguages[j]))
					{
						Debug.LogWarningFormat(WearableConstants.LOCALE_LANGUAGE_ALREADY_SUPPORTED_FORMAT,
							ld.name,
							ld.SupportedLanguages[j],
							_languageToLocaleLookup[ld.SupportedLanguages[j]].name);
						continue;
					}

					_languageToLocaleLookup.Add(ld.SupportedLanguages[j], ld);
				}
			}

			_isSetup = true;
		}

		/// <summary>
		/// Adds a new locale key/value pair to the appropriate <see cref="LocaleData"/> instance. Only works for
		/// already existing, supported languages. If the key already exists, this resolves to a no-op and a
		/// warning is logged.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		internal static void AddLocaleKey(SystemLanguage language, string key, string value)
		{
			Assert.IsTrue(IsSupportedSystemLanguage(language));

			var localeData = LanguageToLocaleLookup[language];
			localeData.AddKVP(key, value);
		}

		/// <summary>
		/// Returns true if a locale key/value pair was found for the preferred system language and key value,
		/// otherwise false. If the preferred system language isn't supported, the fallback language will be
		/// used instead.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool HasLocaleKey(string key)
		{
			var systemLanguage = GetPreferredSystemLanguage();
			if (!IsSupportedSystemLanguage(systemLanguage))
			{
				systemLanguage = LocaleTools.GetFallbackSystemLanguage();
			}

			return HasLocaleKey(systemLanguage, key);
		}

		/// <summary>
		/// Returns true if a locale key/value pair was found for a specific language and key value,
		/// otherwise false.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool HasLocaleKey(SystemLanguage language, string key)
		{
			if (!LanguageToLocaleLookup.ContainsKey(language))
			{
				return false;
			}

			return LanguageToLocaleLookup[language].HasKVP(key);
		}

		/// <summary>
		/// Returns true if a font was found for a given <see cref="SystemLanguage"/> and
		/// <see cref="FontStyleId"/>, otherwise false. If true, <paramref name="fontAsset"/> will be
		/// initialized with the appropriate value.
		/// </summary>
		/// <param name="language"></param>
		/// <param name="fontStyleId"></param>
		/// <param name="fontAsset"></param>
		/// <returns></returns>
		internal static bool TryGetFont(SystemLanguage language, FontStyleId fontStyleId, out TMP_FontAsset fontAsset)
		{
			fontAsset = null;

			LocaleData ld;
			if (!LanguageToLocaleLookup.TryGetValue(language, out ld))
			{
				return false;
			}

			return ld.TryGetFont(fontStyleId, out fontAsset);
		}

		/// <summary>
		/// Returns true if a locale value was found for <paramref name="key"/> for the preferred system
		/// language, otherwise false. If true, <paramref name="value"/> will be populated with the found value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetLocaleValue(string key, out string value)
		{
			var systemLanguage = GetPreferredSystemLanguage();
			return TryGetLocaleValue(systemLanguage, key, out value);
		}

		/// <summary>
		/// Returns true if a locale value was found for <paramref name="key"/> for <paramref name="systemLanguage"/>
		/// language or fallback language, otherwise false. If true, <paramref name="value"/> will be populated
		/// with the found value.
		/// </summary>
		/// <param name="systemLanguage"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetLocaleValue(SystemLanguage systemLanguage, string key, out string value)
		{
			value = string.Empty;

			LocaleData localeData;
			if (!LanguageToLocaleLookup.TryGetValue(systemLanguage, out localeData))
			{
				var fallbackLanguage = LocaleTools.GetFallbackSystemLanguage();
				localeData = LanguageToLocaleLookup[fallbackLanguage];
			}

			return localeData.TryGetLocaleValue(key, out value);
		}

		/// <summary>
		/// Returns true if the passed <paramref name="systemLanguage"/> has dedicated locale support,
		/// otherwise returns false.
		/// </summary>
		/// <param name="systemLanguage"></param>
		/// <returns></returns>
		public static bool IsSupportedSystemLanguage(SystemLanguage systemLanguage)
		{
			return LanguageToLocaleLookup.ContainsKey(systemLanguage);
		}

		/// <summary>
		/// Returns the preferred <see cref="SystemLanguage"/> if found, or the OS-set value if a preferred
		/// value isn't set and it's supported, or a fallback language if not supported.
		/// </summary>
		/// <returns></returns>
		public static SystemLanguage GetPreferredSystemLanguage()
		{
			// If the user/developer has set a preference for a different Bose AR system language, use that
			// instead. Otherwise return the OS-set system language.
			if (PlayerPrefs.HasKey(WearableConstants.PREF_PREFERRED_SYSTEM_LANGUAGE))
			{
				return (SystemLanguage)PlayerPrefs.GetInt(WearableConstants.PREF_PREFERRED_SYSTEM_LANGUAGE);
			}

			var systemLanguage = Application.systemLanguage;
			if (!IsSupportedSystemLanguage(systemLanguage))
			{
				systemLanguage = LocaleTools.GetFallbackSystemLanguage();
			}

			return systemLanguage;
		}

		/// <summary>
		/// Sets the preferred <see cref="SystemLanguage"/> used by the Bose AR SDK. If different from the
		/// current preferred language <seealso cref="LanguagePreferenceChanged"/> will be invoked.
		/// </summary>
		/// <param name="preferredLanguage"></param>
		public static void SetPreferredSystemLanguage(SystemLanguage preferredLanguage)
		{
			var currentLanguage = GetPreferredSystemLanguage();
			if (currentLanguage != preferredLanguage)
			{
				PlayerPrefs.SetInt(WearableConstants.PREF_PREFERRED_SYSTEM_LANGUAGE, (int)preferredLanguage);

				if (LanguagePreferenceChanged != null)
				{
					LanguagePreferenceChanged.Invoke();
				}
			}
		}

		/// <summary>
		/// Clears the preferred system language
		/// </summary>
		public static void ClearSystemLanguagePreference()
		{
			var currentLanguage = GetPreferredSystemLanguage();
			if (PlayerPrefs.HasKey(WearableConstants.PREF_PREFERRED_SYSTEM_LANGUAGE))
			{
				PlayerPrefs.DeleteKey(WearableConstants.PREF_PREFERRED_SYSTEM_LANGUAGE);
			}

			// If our new preferred language is different from our last, invoke the proper event.
			var newCurrentLanguage = GetPreferredSystemLanguage();
			if (currentLanguage != newCurrentLanguage &&
			   LanguagePreferenceChanged != null)
			{
				LanguagePreferenceChanged.Invoke();
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerable{T}"/> over a collection of supported languages for localization.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<SystemLanguage> GetSupportedLanguages()
		{
			// Lazy-load the supported languages from our internal lookup
			if (_supportedLanguages == null)
			{
				_supportedLanguages = new List<SystemLanguage>();
				foreach (var kvp in LanguageToLocaleLookup)
				{
					if (_supportedLanguages.Contains(kvp.Key))
					{
						continue;
					}

					_supportedLanguages.Add(kvp.Key);
				}
			}

			return _supportedLanguages;
		}
	}
}
