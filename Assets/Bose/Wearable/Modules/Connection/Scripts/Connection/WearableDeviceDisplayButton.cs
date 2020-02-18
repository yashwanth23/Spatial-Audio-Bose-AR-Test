using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Displays a device info as a clickable button for a user to select.
	/// </summary>
	internal sealed class WearableDeviceDisplayButton : MonoBehaviour
	{
		#pragma warning disable 0649

		/// <summary>
		/// The label text for this device.
		/// </summary>
		[Header("UI Refs")]
		[SerializeField]
		private TMP_Text _labelText;

		/// <summary>
		/// The signal icon for this device's current rssi.
		/// </summary>
		[SerializeField]
		private Image _signalStrengthIcon;

		/// <summary>
		/// The button on this display.
		/// </summary>
		[SerializeField]
		private Button _button;

		[Header("Data"), Space(5)]
		[SerializeField]
		private SignalStrengthIconFactory _iconFactory;

		#pragma warning restore 0649

		public Device Device
		{
			get { return _device; }
		}

		private Device _device;

		/// <summary>
		/// Set local components and add any listeners
		/// </summary>
		private void Awake()
		{
			_button.onClick.AddListener(OnClick);
		}

		/// <summary>
		/// When the device display is clicked, pass the device up to the SelectionController.
		/// </summary>
		private void OnClick()
		{
			var selectionController = GetComponentInParent<ISelectionController<Device>>();
			if (selectionController == null)
			{
				return;
			}

			selectionController.OnSelect(_device);
		}

		/// <summary>
		/// Remove all listeners.
		/// </summary>
		private void OnDestroy()
		{
			_button.onClick.RemoveAllListeners();
		}

		/// <summary>
		/// Set the device on this display.
		/// </summary>
		/// <param name="deviceValue"></param>
		public void Set(Device deviceValue)
		{
			_device = deviceValue;
			_labelText.text = string.Format("{0}", deviceValue.name);

			Sprite iconSprite;
			if (_iconFactory.TryGetIcon(deviceValue.GetSignalStrength(), out iconSprite))
			{
				_signalStrengthIcon.sprite = iconSprite;
			}
		}
	}
}

