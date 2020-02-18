using System;
using Bose.Wearable.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="DeviceInfoDebugUIControl"/> shows information about the device itself and allows for
	/// searching for a device to connect to as well as disconnecting from a currently connected device.
	/// </summary>
	internal sealed class DeviceInfoDebugUIControl : DebugUIControlBase
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private GameObject _deviceInfoPanel;

		[SerializeField]
		private GameObject _noDeviceInfoPanel;

		[SerializeField]
		private Text _deviceNameText;

		[SerializeField]
		private Text _deviceDescriptionText;

		[SerializeField]
		private Text _deviceUidText;

		[SerializeField]
		private Button _connectButton;

		[SerializeField]
		private Image _connectButtonImage;

		[SerializeField]
		private Button _disconnectButton;

		[SerializeField]
		private Image _disconnectButtonImage;

		#pragma warning restore 0649

		protected override void Start()
		{
			base.Start();

			UpdateUI(isConnected: _wearableControl.ConnectedDevice.HasValue);

			_wearableControl.ConnectionStatusChanged += OnConnectionStatusChanged;

			_connectButton.onClick.AddListener(OnConnectButtonClicked);
			_disconnectButton.onClick.AddListener(OnDisconnectButtonClicked);
		}

		private void OnDestroy()
		{
			_wearableControl.ConnectionStatusChanged -= OnConnectionStatusChanged;

			_connectButton.onClick.RemoveAllListeners();
			_disconnectButton.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			UpdateColorStyle();
		}

		private void OnConnectionStatusChanged(ConnectionStatus status, Device? device)
		{
			switch (status)
			{
				case ConnectionStatus.Connected:
					UpdateUI(isConnected: true);
					break;
				case ConnectionStatus.Disconnected:
					UpdateUI(isConnected: false);
					break;
			}
		}

		private void OnDisconnectButtonClicked()
		{
			_wearableControl.DisconnectFromDevice();
		}

		private void OnConnectButtonClicked()
		{
			var wearableConnectUIPanel = FindObjectOfType<WearableConnectUIPanel>();
			if (wearableConnectUIPanel != null)
			{
				wearableConnectUIPanel.Show();
			}
		}

		private void UpdateUI(bool isConnected)
		{
			_deviceInfoPanel.gameObject.SetActive(isConnected);
			_noDeviceInfoPanel.gameObject.SetActive(!isConnected);

			_disconnectButton.gameObject.SetActive(isConnected);
			_connectButton.gameObject.SetActive(!isConnected);

			UpdateColorStyle();

			if (!isConnected || !_wearableControl.ConnectedDevice.HasValue)
			{
				return;
			}

			var device = _wearableControl.ConnectedDevice.Value;

			_deviceNameText.text = device.name;
			_deviceDescriptionText.text =
				Enum.GetName(typeof(VariantType), device.GetVariantType()).Nicify().ToUpper();

			_deviceUidText.text = device.firmwareVersion;
		}

		private void UpdateColorStyle()
		{
			var style = _colorPalette.GetCustomizedActiveStyle();

			_disconnectButtonImage.color = style.elementColor;
		}
	}
}
