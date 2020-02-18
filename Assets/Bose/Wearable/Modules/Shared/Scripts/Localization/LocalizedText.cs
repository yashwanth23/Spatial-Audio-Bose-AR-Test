using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="LocalizedText"/> aims to make localization of specific <see cref="Text"/> components easier
	/// by allowing a developer to select the locale key for a given key/value pair and preview the value in
	/// the editor for any supported language. In addition, at runtime the locale value will attempted to be
	/// retrieved for the users preferred <see cref="SystemLanguage"/>.
	/// </summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(TMP_Text))]
	public sealed class LocalizedText : MonoBehaviour
	{
		/// <summary>
		/// The locale key that will be used.
		/// </summary>
		public string LocaleKey
		{
			get { return _localeKey; }
		}

		internal TMP_Text TMPText
		{
			get { return _tmpText; }
			set { _tmpText = value; }
		}

		// UI
		[SerializeField]
		private TMP_Text _tmpText;

		// Localization
		[SerializeField]
		private string _localeKey;

		[SerializeField]
		private FontStyleId _fontStyleId;

		private string[] _localeKeyArguments;
		private object[] _localeKeyValues;

		private void Awake()
		{
			if (_tmpText == null)
			{
				_tmpText = GetComponent<TMP_Text>();
			}

			UpdateLocale();
		}

		private void OnEnable()
		{
			LocaleCache.LanguagePreferenceChanged += OnLanguagePreferenceChanged;
		}

		private void OnDisable()
		{
			LocaleCache.LanguagePreferenceChanged -= OnLanguagePreferenceChanged;
		}

		/// <summary>
		/// On language preference changed, update the displayed locale value text.
		/// </summary>
		private void OnLanguagePreferenceChanged()
		{
			UpdateLocale();
		}

		/// <summary>
		/// Sets the locale key and then updates the displayed locale. If any values are passed for
		/// <paramref name="localeKeyArguments"/>, these will be supplied to the primary locale key as format string
		/// arguments.
		/// </summary>
		/// <param name="localeKey">The locale key to use as the primary format string.</param>
		/// <param name="localeKeyArguments">If the locale value for <paramref name="localeKey"/> is a format string,
		/// supply additional locale keys for values to be used as format string arguments.</param>
		public void SetLocaleKey(string localeKey, params string[] localeKeyArguments)
		{
			_localeKey = localeKey;
			_localeKeyArguments = localeKeyArguments;

			// If there are any supplied locale key format string arguments, initialize a new array for the
			// cached locale values.
			if (_localeKeyArguments == null || _localeKeyArguments.Length == 0)
			{
				_localeKeyValues = null;
			}
			else
			{
				_localeKeyValues = new object[localeKeyArguments.Length];
			}

			UpdateLocale();
		}

		/// <summary>
		/// Updates the displayed text using the preferred system language and local locale key.
		/// </summary>
		public void UpdateLocale()
		{
			UpdateLocale(LocaleCache.GetPreferredSystemLanguage());
		}

		/// <summary>
		/// Updates the displayed text using the passed <paramref name="language"/> and locale key.
		/// </summary>
		public void UpdateLocale(SystemLanguage language)
		{
			Assert.IsNotNull(_tmpText);

			// Attempt to set the font first before any locale value in case a specific font is needed.
			TMP_FontAsset fontAsset;
			if (LocaleCache.TryGetFont(language, _fontStyleId, out fontAsset))
			{
				_tmpText.font = fontAsset;
			}

			// Attempt to set the locale key value
			string localeValue;
			if (LocaleCache.TryGetLocaleValue(language, _localeKey, out localeValue))
			{
				// If there are no substitution arguments to pass to the format string, set it directly.
				if (_localeKeyArguments == null || _localeKeyArguments.Length == 0)
				{
					_tmpText.text = localeValue;
				}
				// Otherwise grab all of the localization values for the string format arguments and then set
				// the value as a format string with the additional arguments.
				else
				{
					for (var i = 0; i < _localeKeyArguments.Length; i++)
					{
						string argKeyValue;
						if (!LocaleCache.TryGetLocaleValue(_localeKeyArguments[i], out argKeyValue))
						{
							Debug.LogFormat(WearableConstants.LOCALE_KEY_ARGUMENT_NOT_FOUND_FORMAT,
								_localeKey,
								_localeKeyArguments[i]);
						}

						_localeKeyValues[i] = argKeyValue;
					}

					_tmpText.text = string.Format(localeValue, _localeKeyValues);
				}
			}
			else if(!string.IsNullOrEmpty(_localeKey))
			{
				Debug.LogWarningFormat(this, WearableConstants.LOCALE_KEY_NOT_FOUND_FORMAT, _localeKey, language);
			}
		}

		/// <summary>
		/// Clears the cached localization key and  sets the displayed text to an empty string.
		/// </summary>
		public void Clear()
		{
			_localeKeyArguments = null;
			_localeKeyValues = null;
			_localeKey = string.Empty;
			_tmpText.text = string.Empty;
		}

		#if UNITY_EDITOR

		private void Reset()
		{
			_fontStyleId = null;
			_localeKey = string.Empty;
			_tmpText = GetComponent<TMP_Text>();
		}

		#endif
	}
}
