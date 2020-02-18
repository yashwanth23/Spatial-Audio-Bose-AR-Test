using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable.Examples
{
	internal sealed class BasicDemoUIPanel : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UX Refs")]
		[SerializeField]
		private Toggle _referenceToggle;

		[SerializeField]
		private Text _referenceLabel;

		[SerializeField]
		private RotationMatcher _matcher;

		#pragma warning restore 0649

		private const string CENTER_LABEL = "CENTER";
		private const string RESET_LABEL = "RESET";

		private void Awake()
		{
			// N.B. On = greyed out, the state corresponding to "reset", which happens when we are already
			// in relative mode.
			_referenceToggle.isOn = _matcher.ReferenceMode == RotationMatcher.RotationReference.Relative;
			_referenceLabel.text =
				_matcher.ReferenceMode == RotationMatcher.RotationReference.Absolute ? CENTER_LABEL : RESET_LABEL;

			// Must be registered after the above to avoid an errant call.
			_referenceToggle.onValueChanged.AddListener(OnReferenceToggleClicked);
		}

		private void OnDestroy()
		{
			_referenceToggle.onValueChanged.RemoveAllListeners();
		}

		/// <summary>
		/// Alternates which rotation mode is currently selected, and changes the text to match.
		/// </summary>
		/// <param name="isOn"></param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private void OnReferenceToggleClicked(bool isOn)
		{
			switch (_matcher.ReferenceMode)
			{
				case RotationMatcher.RotationReference.Absolute:
					// Was reset, now will center. Button allows users to reset again.
					_matcher.SetRelativeReference();
					_referenceLabel.text = RESET_LABEL;
					break;

				case RotationMatcher.RotationReference.Relative:
					// Was centered, now will reset. Button allows users to center again.
					_matcher.SetAbsoluteReference();
					_referenceLabel.text = CENTER_LABEL;
					break;

				default:
					throw new ArgumentOutOfRangeException("ReferenceMode", _matcher.ReferenceMode, null);
			}
		}
	}
}
