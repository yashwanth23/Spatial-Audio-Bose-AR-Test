using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Bose.Wearable
{
	internal sealed class SignalStrengthIconFactory : ScriptableObject
	{
		[Serializable]
		private class IconMapping
		{
			#pragma warning disable 0649
			public SignalStrength signalStrength;

			public Sprite sprite;
			#pragma warning restore 0649
		}

		[SerializeField]
		private List<IconMapping> _iconMappings;

		private Dictionary<SignalStrength, IconMapping> _iconMappingLookup;

		private const string MAPPING_MISSING_WARNING_FORMAT =
			"[Bose Wearable] A mapping is missing for SignalStrength [{0}] on SignalStrengthIconFactory instance.";
		private const string ICON_UNASSIGNED_WARNING_FORMAT =
			"[Bose Wearable] An icon is not assigned for SignalStrength [{0}] on SignalStrengthIconFactory instance.";
		private const string ICON_MAPPING_IS_DUPLICATED =
			"[Bose Wearable] There is more than one icon mapping for SignalStrength [{0}] on SignalStrengthIconFactory instance.";

		private void OnEnable()
		{
			_iconMappingLookup = new Dictionary<SignalStrength, IconMapping>();
			for (var i = 0; i < _iconMappings.Count; i++)
			{
				if (_iconMappingLookup.ContainsKey(_iconMappings[i].signalStrength))
				{
					continue;
				}

				_iconMappingLookup.Add(_iconMappings[i].signalStrength, _iconMappings[i]);
			}
		}

		/// <summary>
		/// Returns true if a mapping is found for <see cref="SignalStrength"/> <paramref name="signalStrength"/> to a
		/// <see cref="Sprite"/>, otherwise false.
		/// </summary>
		/// <param name="signalStrength"></param>
		/// <param name="sprite"></param>
		/// <returns></returns>
		public bool TryGetIcon(SignalStrength signalStrength, out Sprite sprite)
		{
			sprite = null;

			IconMapping iconMapping;
			if (!_iconMappingLookup.TryGetValue(signalStrength, out iconMapping))
			{
				Debug.LogWarningFormat(this, MAPPING_MISSING_WARNING_FORMAT, signalStrength);
				return false;
			}

			sprite = iconMapping.sprite;

			return true;
		}

		#if UNITY_EDITOR

		private void OnValidate()
		{
			// Iterate through all SignalStrength values and ensure there is a single icon mapping for each one.
			// Flag any duplicate icon mappings for SignalStrength.
			for (var i = 0; i < WearableConstants.SIGNAL_STRENGTHS.Length; i++)
			{
				var signalStrength = WearableConstants.SIGNAL_STRENGTHS[i];

				// If we have an existing mapping for this SignalStrength, skip it.
				if (_iconMappings.Any(x => x.signalStrength == signalStrength))
				{
					if (_iconMappings.Count(x => x.signalStrength == signalStrength) > 1)
					{
						Debug.LogWarningFormat(this, ICON_MAPPING_IS_DUPLICATED, signalStrength);
					}

					continue;
				}

				// Where we do not find a mapping for this SignalStrength, add one.
				_iconMappings.Add(new IconMapping { signalStrength = signalStrength });
			}

			// Ensure all icon mappings have a sprite assigned.
			for (var i = 0; i < _iconMappings.Count; i++)
			{
				if (_iconMappings[i].sprite != null)
				{
					continue;
				}

				Debug.LogWarningFormat(this, ICON_UNASSIGNED_WARNING_FORMAT, _iconMappings[i].signalStrength);
			}
		}

		private void Reset()
		{
			_iconMappings = new List<IconMapping>();
			for (var i = 0; i < WearableConstants.SIGNAL_STRENGTHS.Length; i++)
			{
				_iconMappings.Add(new IconMapping { signalStrength = WearableConstants.SIGNAL_STRENGTHS[i] });
			}
		}

		#endif
	}
}
