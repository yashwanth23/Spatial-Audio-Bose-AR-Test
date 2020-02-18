using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="LocaleData"/> is the container of localization key/value pairs (KVPs) for one or more
	/// locales. It contains a serialized POCO collection of KVPs that at runtime will be loaded into a
	/// Dictionary for easy access.
	/// </summary>
	internal sealed class LocaleData : ScriptableObject
	{
		/// <summary>
		/// A key/value pair of localization.
		/// </summary>
		[Serializable]
		internal sealed class LocaleKVP : IComparable
		{
			[SerializeField]
			public string key;

			[SerializeField]
			public string value;

			public int CompareTo(object obj)
			{
				// Sort in ascending order based on the key value.
				return string.Compare(key, ((LocaleKVP)obj).key, StringComparison.Ordinal);
			}
		}

		/// <summary>
		/// Represents a mapping between a unique identifier and a specific <see cref="Font"/> asset.
		/// </summary>
		[Serializable]
		internal sealed class FontStyle : IEquatable<FontStyle>
		{
			/// <summary>
			/// A unique identifier for this style instance.
			/// </summary>
			public FontStyleId Id
			{
				get { return _id; }
			}

			/// <summary>
			/// The <see cref="TMP_FontAsset"/> associated with this style instance.
			/// </summary>
			public TMP_FontAsset TMPFontAsset
			{
				get { return _tmpFontAsset; }
			}

			#pragma warning disable 0649

			[SerializeField]
			private FontStyleId _id;

			[SerializeField]
			private TMP_FontAsset _tmpFontAsset;

			#pragma warning restore 0649

			#region IEquatable

			public bool Equals(FontStyle other)
			{
				if (ReferenceEquals(null, other))
				{
					return false;
				}

				if (ReferenceEquals(this, other))
				{
					return true;
				}

				return Equals(_id, other._id);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
				{
					return false;
				}

				if (ReferenceEquals(this, obj))
				{
					return true;
				}

				return obj is FontStyle && Equals((FontStyle)obj);
			}

			public override int GetHashCode()
			{
				return (_id != null ? _id.GetHashCode() : 0);
			}

			#endregion
		}

		#region Runtime

		/// <summary>
		/// The internal cache of localization data.
		/// </summary>
		internal List<LocaleKVP> LocaleKVPs
		{
			get
			{
				if (_localeKVPs == null)
				{
					_localeKVPs = new List<LocaleKVP>();
				}

				return _localeKVPs;
			}
		}

		/// <summary>
		/// Returns the list of languages that this <see cref="LocaleData"/> instance supports.
		/// </summary>
		internal SystemLanguage[] SupportedLanguages
		{
			get { return _supportedLanguages; }
		}

		/// <summary>
		/// A <see cref="Font"/> asset to be used in place of any others for anything using this
		/// <see cref="LocaleData"/> instance.
		/// </summary>
		public TMP_FontAsset OverrideFontAsset
		{
			get { return _overrideFontAsset; }
		}

		/// <summary>
		/// Whether or not anything using this <see cref="LocaleData"/> should be using the
		/// <see cref="OverrideFontAsset"/>.
		/// </summary>
		public bool HasOverrideFontAsset
		{
			get { return _overrideFontAsset != null; }
		}

		/// <summary>
		/// Returns a list of <see cref="FontStyles"/>.
		/// </summary>
		public List<FontStyle> FontStyles
		{
			get { return _fontStyles; }
		}

		/// <summary>
		/// The internal run-time lookup of localization data. Will be lazy-loaded upon retrieval for the
		/// first time.
		/// </summary>
		private Dictionary<string, string> LocaleLookup
		{
			get
			{
				Setup();

				return _localeLookup;
			}
		}

		#pragma warning disable 0649

		/// <summary>
		/// The serialized cache of localization data that will be loaded at runtime.
		/// </summary>
		[SerializeField]
		private List<LocaleKVP> _localeKVPs;

		[SerializeField]
		private SystemLanguage[] _supportedLanguages;

		[SerializeField]
		private TMP_FontAsset _overrideFontAsset;

		[SerializeField]
		private List<FontStyle> _fontStyles;

		#pragma warning restore 0649

		/// <summary>
		/// The runtime collection of localization data
		/// </summary>
		private Dictionary<string, string> _localeLookup;

		private bool _isSetup;

		public void Setup(bool forceSetup = false)
		{
			if (_isSetup && !forceSetup)
			{
				return;
			}

			if (_localeLookup == null)
			{
				_localeLookup = new Dictionary<string, string>();
			}
			else
			{
				_localeLookup.Clear();
			}

			var localeKVPs = LocaleKVPs;
			for (var i = 0; i < localeKVPs.Count; i++)
			{
				_localeLookup.Add(localeKVPs[i].key, localeKVPs[i].value);
			}

			_isSetup = true;
		}

		/// <summary>
		/// Adds the key/value pair if the key does not already exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddKVP(string key, string value)
		{
			if (HasKVP(key))
			{
				Debug.LogWarningFormat(WearableConstants.LOCALE_KEY_ALREADY_EXISTS_FORMAT, key);
				return;
			}

			// If we're in the editor and not playing, add this to our serialized cache
			// Otherwise add it to our runtime collection.
			#if UNITY_EDITOR

			if (!Application.isPlaying)
			{
				LocaleKVPs.Add(new LocaleKVP(){key = key, value = value});
				return;
			}

			#endif

			LocaleLookup.Add(key, value);
		}

		/// <summary>
		/// Returns true if this instance contains a locale value for the passed <paramref name="key"/>.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool HasKVP(string key)
		{
			// If we're in the editor and not playing, check our serialized cache
			// Otherwise check our runtime collection.
			#if UNITY_EDITOR

			if (!Application.isPlaying)
			{
				var localeKVPs = LocaleKVPs;
				for (var i = 0; i < localeKVPs.Count; i++)
				{
					if (localeKVPs[i].key == key)
					{
						return true;
					}
				}

				return false;
			}

			#endif

			return LocaleLookup.ContainsKey(key);
		}

		/// <summary>
		/// Returns true if a KVP is found for the passed <paramref name="key"/>, otherwise false. The <paramref name="value"/>
		/// parameter will be initialized with the proper localized data if true is returned.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGetLocaleValue(string key, out string value)
		{
			value = null;

			// If we're in the editor and not playing, check our serialized cache
			// Otherwise check our runtime collection.
			#if UNITY_EDITOR

			if (!Application.isPlaying)
			{
				var localeKVPs = LocaleKVPs;
				for (var i = 0; i < localeKVPs.Count; i++)
				{
					if (localeKVPs[i].key == key)
					{
						value = localeKVPs[i].value;
						return true;
					}
				}

				return false;
			}

			#endif

			return LocaleLookup.TryGetValue(key, out value);
		}

		/// <summary>
		/// Returns true if a <see cref="Font"/> was found for the passed <see cref="FontStyleId"/> or if an
		/// <see cref="OverrideFontAsset"/> was present, otherwise false. If true, <paramref name="fontAsset"/>
		/// will have its value initialized.
		/// </summary>
		/// <param name="fontStyleId"></param>
		/// <param name="fontAsset"></param>
		/// <returns></returns>
		public bool TryGetFont(FontStyleId fontStyleId, out TMP_FontAsset fontAsset)
		{
			fontAsset = null;

			for (var i = 0; i < _fontStyles.Count; i++)
			{
				if (_fontStyles[i].Id != fontStyleId)
				{
					continue;
				}

				fontAsset = _fontStyles[i].TMPFontAsset;
				break;
			}

			if (fontAsset == null)
			{
				fontAsset = OverrideFontAsset;
			}

			return fontAsset != null;
		}

		#endregion

		#region Editor-Only
		#if UNITY_EDITOR

		/// <summary>
		/// Clears the serialized cache of locale data.
		/// </summary>
		public void Clear()
		{
			LocaleKVPs.Clear();
		}

		/// <summary>
		/// Unity-hook for resetting object instance data.
		/// </summary>
		private void Reset()
		{
			Clear();
		}

		private void OnValidate()
		{
			const string WARNING_FORMAT =
				"Each FontStyle should have a unique FontStyleId assigned. {0} is used multiple times in this " +
				"instance.";

			// Verify each FontStyle has a unique id.
			var fontStyleGroupsById = _fontStyles.GroupBy(x => x.Id);
			foreach (var fontStyleGroup in fontStyleGroupsById)
			{
				Assert.IsFalse(fontStyleGroup.Count() > 1, string.Format(WARNING_FORMAT, fontStyleGroup.Key));
			}
		}

		#endif
		#endregion
	}
}
