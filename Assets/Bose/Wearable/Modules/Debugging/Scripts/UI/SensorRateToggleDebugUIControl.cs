using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="SensorRateToggleDebugUIControl"/> is a debug UI control for showing whether or not a specific
	/// <see cref="SensorUpdateInterval"/> is in use or not.
	/// </summary>
	internal sealed class SensorRateToggleDebugUIControl : DebugUIControlBase, IPointerClickHandler
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Image _conflictingPipImage;

		[SerializeField]
		private Image _toggleImage;

		[SerializeField]
		private Text _labelText;

		[Header("Data"), Space(5)]
		[SerializeField]
		private SensorUpdateIntervalLabelFactory _labelFactory;

		[SerializeField]
		private SensorUpdateInterval _sensorUpdateInterval;

		#pragma warning restore 0649

		private void Update()
		{
			var isEnabled = _wearableControl.CurrentDeviceConfig.updateInterval == _sensorUpdateInterval;
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
			if (!_wearableControl.IsOverridingDeviceConfig ||
			    _wearableControl.OverrideDeviceConfig.updateInterval == _sensorUpdateInterval)
			{
				return;
			}

			_wearableControl.OverrideDeviceConfig.updateInterval = _sensorUpdateInterval;
			_wearableControl.ForceDeviceStateResolution();
		}

		#endregion

		/// <summary>
		/// Sets a <see cref="SensorUpdateInterval"/> <paramref name="sensorUpdateInterval"/> for this control.
		/// </summary>
		/// <param name="sensorUpdateInterval"></param>
		public void SetUpdateInterval(SensorUpdateInterval sensorUpdateInterval)
		{
			_sensorUpdateInterval = sensorUpdateInterval;
			_labelText.text = _labelFactory.GetLabel(sensorUpdateInterval);
		}

		/// <summary>
		/// Is the <see cref="SensorUpdateInterval"/> of the Requirements device config conflicting with the
		/// overriden config?
		/// </summary>
		/// <returns></returns>
		protected override bool IsConflicting()
		{
			var reqUpdateInterval = _wearableControl.FinalWearableDeviceConfig.updateInterval;
			var overrideInterval = _wearableControl.OverrideDeviceConfig.updateInterval;

			return _wearableControl.IsOverridingDeviceConfig &&
			       overrideInterval != reqUpdateInterval &&
			       (overrideInterval == _sensorUpdateInterval || reqUpdateInterval == _sensorUpdateInterval);
		}
	}
}
