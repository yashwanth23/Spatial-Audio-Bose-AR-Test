using Bose.Wearable.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="RotationDataToggleDebugUIControl"/> is a debug UI control that allows toggling between
	/// showing rotation data as a quaternion or as a euler vector.
	/// </summary>
	internal sealed class RotationDataToggleDebugUIControl : DebugUIControlBase, IPointerClickHandler
	{
		/// <summary>
		/// <see cref="VisualizationMode"/> describes the mode in which data is visualized
		/// </summary>
		private enum VisualizationMode
		{
			/// <summary>
			/// Data is visualized as a quaternion.
			/// </summary>
			Quaternion,

			/// <summary>
			/// Data is visualized as a euler vector.
			/// </summary>
			Euler
		}

		/// <summary>
		/// Stores which rotation source this UI corresponds to.
		/// </summary>
		public enum RotationSensorSource
		{
			SixDof,
			NineDof
		}

		public RotationSensorSource RotationSource
		{
			get { return _rotationSource; }
			set { _rotationSource = value; }
		}

		#pragma warning disable 0649

		[SerializeField]
		private RotationSensorSource _rotationSource;

		[Header("Rotation UI Refs")]
		[SerializeField]
		private Image _rotationTitleImage;

		[SerializeField]
		private Text _rotationTitleText;

		[SerializeField]
		private Text _rotationSourceText;

		[SerializeField]
		private Text _rotationUncertaintyText;

		[SerializeField]
		private Text _rotationXText;

		[SerializeField]
		private Text _rotationYText;

		[SerializeField]
		private Text _rotationZText;

		[SerializeField]
		private Text _rotationWText;

		[SerializeField]
		private Font _fontNumeric;

		[SerializeField]
		private Font _fontAlpha;

		#pragma warning restore 0649

		private VisualizationMode _vizMode;

		private List<VisualizationMode> _allVizModes;
		private Text[] _allComponentTextElements;

		private const float THIRDS_ANCHOR_INCREMENT = 0.33f;
		private const float QUARTERS_ANCHOR_INCREMENT = 0.25f;
		private const string SIX_DOF_LABEL = "SIX DOF";
		private const string NINE_DOF_LABEL = "NINE DOF";

		private void Awake()
		{
			_allVizModes =
				new List<VisualizationMode>((VisualizationMode[])Enum.GetValues(typeof(VisualizationMode)));

			_allComponentTextElements = new []
			{
				_rotationXText,
				_rotationYText,
				_rotationZText,
				_rotationWText
			};

			UpdateTextAnchors();
		}

		#region IPointerClickHandler

		public void OnPointerClick(PointerEventData eventData)
		{
			// When the user clicks, switch the visualization mode from Euler to Quaternion and vice-versa.
			var nextIndex = _allVizModes.IndexOf(_vizMode) + 1;
			if (nextIndex >= _allVizModes.Count)
			{
				nextIndex = 0;
			}

			_vizMode = _allVizModes[nextIndex];
		}

		#endregion

		/// <summary>
		/// Updates the UI based on the passed <see cref="SensorQuaternion"/> <paramref name="sensorQuaternion"/>
		/// representing a <see cref="SensorFrame.rotation"/> value.
		/// </summary>
		/// <param name="sensorQuaternion"></param>
		public void UpdateUI(SensorQuaternion sensorQuaternion)
		{
			SensorId id = _rotationSource == RotationSensorSource.NineDof ? SensorId.RotationNineDof : SensorId.RotationSixDof;
			var currentDeviceConfig = _wearableControl.CurrentDeviceConfig;
			var style = currentDeviceConfig.GetSensorConfig(id).isEnabled
				? _colorPalette.GetCustomizedActiveStyle()
				: _colorPalette.InactiveTitleElementStyle;

			var rotValue3 = _vizMode == VisualizationMode.Quaternion
				? sensorQuaternion.value.XYZ()
				: sensorQuaternion.value.eulerAngles;

			var positiveFormat = _vizMode == VisualizationMode.Quaternion
				? DebuggingConstants.QUATERNION_COMPONENT_FORMAT_POSITIVE
				: DebuggingConstants.EULER_DATA_COMPONENT_FORMAT_POSITIVE;

			var negativeFormat = _vizMode == VisualizationMode.Quaternion
				? DebuggingConstants.QUATERNION_COMPONENT_FORMAT_NEGATIVE
				: DebuggingConstants.EULER_DATA_COMPONENT_FORMAT_NEGATIVE;

			// Update Rotation
			var isRotationEnabled = currentDeviceConfig.GetSensorConfig(id).isEnabled;
			var childElementColor = isRotationEnabled
				? _colorPalette.ActiveDataTextColor
				: _colorPalette.InactiveDataTextColor;

			Color rotTitleTextColor;
			Color rotTitleElementColor;
			if (isRotationEnabled)
			{
				rotTitleTextColor = style.textColor;
				rotTitleElementColor = style.elementColor;
			}
			else
			{
				rotTitleTextColor = _colorPalette.InactiveTitleElementStyle.textColor;
				rotTitleElementColor = _colorPalette.InactiveTitleElementStyle.elementColor;
			}

			_rotationTitleText.color = rotTitleTextColor;
			_rotationTitleImage.color = rotTitleElementColor;

			_rotationXText.color = childElementColor;
			_rotationXText.text = string.Format(
				rotValue3.x >= 0
					? positiveFormat
					: negativeFormat,
				DebuggingConstants.X_FIELD,
				Mathf.Abs(rotValue3.x));

			_rotationYText.color = childElementColor;
			_rotationYText.text = string.Format(
				rotValue3.y >= 0
					? positiveFormat
					: negativeFormat,
				DebuggingConstants.Y_FIELD,
				Mathf.Abs(rotValue3.y));

			_rotationZText.color = childElementColor;
			_rotationZText.text = string.Format(
				rotValue3.z >= 0
					? positiveFormat
					: negativeFormat,
				DebuggingConstants.Z_FIELD,
				Mathf.Abs(rotValue3.z));

			_rotationWText.color = childElementColor;
			if (_vizMode == VisualizationMode.Quaternion)
			{
				_rotationWText.text = string.Format(
					sensorQuaternion.value.w >= 0
						? positiveFormat
						: negativeFormat,
				DebuggingConstants.W_FIELD,
				Mathf.Abs(sensorQuaternion.value.w));
			}
			else
			{
				_rotationWText.text = string.Empty;
			}

			if (_rotationUncertaintyText != null)
			{
				var font = float.IsInfinity(sensorQuaternion.measurementUncertainty) ? _fontAlpha : _fontNumeric;
				if (_rotationUncertaintyText.font != font)
				{
					_rotationUncertaintyText.font = font;
				}

				_rotationUncertaintyText.color = _rotationSource == RotationSensorSource.SixDof
													? _colorPalette.InactiveUncertaintyTextColor
													: rotTitleTextColor;

				_rotationUncertaintyText.text = string.Format(
					DebuggingConstants.UNCERTAINTY_FORMAT,
					sensorQuaternion.measurementUncertainty).ToUpper();
			}

			var unitsLabelValue = _vizMode == VisualizationMode.Quaternion
				? string.Empty
				: DebuggingConstants.EULER_UNITS;

			_rotationSourceText.color = rotTitleTextColor;
			_rotationSourceText.text = string.Format(
				DebuggingConstants.ROTATION_SOURCE_FORMAT,
				_rotationSource == RotationSensorSource.NineDof ? NINE_DOF_LABEL : SIX_DOF_LABEL,
				unitsLabelValue);

			UpdateTextAnchors();
		}

		private void UpdateTextAnchors()
		{
			var increment = _vizMode == VisualizationMode.Quaternion
				? QUARTERS_ANCHOR_INCREMENT
				: THIRDS_ANCHOR_INCREMENT;
			var startingAnchorValue = 0f;
			var nextAnchorValue = increment;

			var length = _vizMode == VisualizationMode.Quaternion ? 4 : 3;
			for (var i = 0; i < length; i++)
			{
				var text = _allComponentTextElements[i];
				var rt = text.rectTransform;
				rt.anchorMin = new Vector2(startingAnchorValue, 1);
				rt.anchorMax = new Vector2(nextAnchorValue, 1);

				startingAnchorValue += increment;
				nextAnchorValue += increment;
			}
		}
	}
}
