using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="SensorToggleDebugUIControl"/> is a debug control for sensors.
	/// </summary>
	internal sealed class SensorToggleDebugUIControl : DebugUIControlBase, IPointerClickHandler
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Image _conflictingPipImage;

		[SerializeField]
		private Image _toggleImage;

		[SerializeField]
		private Text _labelText;

		[Header("Sensor Data Refs")]
		[SerializeField]
		private SensorId _sensorId;

		#pragma warning restore 0649

		private void Update()
		{
			var isEnabled = _wearableControl.CurrentDeviceConfig.GetSensorConfig(_sensorId).isEnabled;
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

			var sensorConfig = _wearableControl.OverrideDeviceConfig.GetSensorConfig(_sensorId);
			sensorConfig.isEnabled = !sensorConfig.isEnabled;

			_wearableControl.ForceDeviceStateResolution();
		}

		#endregion

		/// <summary>
		/// Returns true if the Requirements sensor config conflicts with the override sensor config,
		/// otherwise false.
		/// </summary>
		/// <returns></returns>
		protected override bool IsConflicting()
		{
			var reqSensorConfig = _wearableControl.FinalWearableDeviceConfig.GetSensorConfig(_sensorId);
			var overrideSensorConfig = _wearableControl.OverrideDeviceConfig.GetSensorConfig(_sensorId);

			return _wearableControl.IsOverridingDeviceConfig &&
			       reqSensorConfig.isEnabled != overrideSensorConfig.isEnabled;
		}
	}
}
