using Bose.Wearable.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="GestureToggleDebugUIControl"/> is a debug control for gestures.
	/// </summary>
	internal sealed class GestureToggleDebugUIControl : DebugUIControlBase, IPointerClickHandler
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Image _conflictingPipImage;

		[SerializeField]
		private Image _toggleImage;

		[SerializeField]
		private Text _labelText;

		#pragma warning restore 0649

		private GestureId _gestureId;

		private void Update()
		{
			if (_gestureId == GestureId.None)
			{
				return;
			}

			var isEnabled = _wearableControl.CurrentDeviceConfig.GetGestureConfig(_gestureId).isEnabled;
			var activeStyle = _colorPalette.GetCustomizedActiveStyle();

			Color textColor;
			Color elementColor;
			if (_wearableControl.IsOverridingDeviceConfig)
			{
				textColor = isEnabled
					? activeStyle.textColor
					: _colorPalette.InactiveOverriddenChildElementStyle.textColor;
				elementColor = isEnabled
					? activeStyle.elementColor
					: _colorPalette.InactiveOverriddenChildElementStyle.elementColor;
			}
			else
			{
				var overrideActiveStyle = _colorPalette.GetCustomizedOverrideActiveStyle();
				textColor = isEnabled
					? overrideActiveStyle.textColor
					: _colorPalette.InactiveChildElementStyle.textColor;
				elementColor = isEnabled
					? overrideActiveStyle.elementColor
					: _colorPalette.InactiveChildElementStyle.elementColor;
			}

			_labelText.color = textColor;
			_toggleImage.color = elementColor;

			var isConflicting = IsConflicting();
			_conflictingPipImage.color = isEnabled
				? _colorPalette.InactiveOverriddenChildElementStyle.elementColor
				: activeStyle.elementColor;

			_conflictingPipImage.gameObject.SetActive(isConflicting);
		}

		#region IPointerClickHandler

		public void OnPointerClick(PointerEventData eventData)
		{
			if (!_wearableControl.IsOverridingDeviceConfig)
			{
				return;
			}

			var gestureConfig = _wearableControl.OverrideDeviceConfig.GetGestureConfig(_gestureId);
			gestureConfig.isEnabled = !gestureConfig.isEnabled;

			_wearableControl.ForceDeviceStateResolution();
		}

		#endregion

		/// <summary>
		/// Sets the <see cref="GestureId"/> <paramref name="gestureId"/> for this control.
		/// </summary>
		/// <param name="gestureId"></param>
		public void SetGestureId(GestureId gestureId)
		{
			Assert.AreNotEqual(gestureId, GestureId.None);

			_gestureId = gestureId;
			_labelText.text = _gestureId.ToString().Nicify().ToUpper();
		}

		/// <summary>
		/// Returns true if the Requirements gesture config conflicts with the overriden config for this gesture.
		/// </summary>
		/// <returns></returns>
		protected override bool IsConflicting()
		{
			var reqGestureConfig = _wearableControl.FinalWearableDeviceConfig.GetGestureConfig(_gestureId);
			var overrideGestureConfig = _wearableControl.OverrideDeviceConfig.GetGestureConfig(_gestureId);

			return _wearableControl.IsOverridingDeviceConfig &&
			       reqGestureConfig.isEnabled != overrideGestureConfig.isEnabled;
		}
	}
}
