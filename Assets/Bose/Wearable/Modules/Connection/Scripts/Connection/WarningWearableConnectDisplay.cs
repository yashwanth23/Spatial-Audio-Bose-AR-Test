using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	internal sealed class WarningWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[SerializeField]
		private Button _searchButton;

		[SerializeField]
		private Button _reconnectButton;

		#pragma warning restore 0649

		private Device? _device;

		protected override void Awake()
		{
			SetupAudio();

			base.Awake();
		}

		private void OnEnable()
		{
			_panel.DeviceDisconnected += OnDeviceDisconnected;
			_panel.DeviceSearching += OnDeviceSearching;
			_panel.DeviceConnecting += OnDeviceConnecting;

			_searchButton.onClick.AddListener(OnSearchButtonClicked);
			_reconnectButton.onClick.AddListener(OnReconnectButtonClicked);
		}

		private void OnDisable()
		{
			_panel.DeviceDisconnected -= OnDeviceDisconnected;
			_panel.DeviceSearching -= OnDeviceSearching;
			_panel.DeviceConnecting -= OnDeviceConnecting;

			_searchButton.onClick.RemoveAllListeners();
			_reconnectButton.onClick.RemoveAllListeners();
		}

		private void OnDeviceDisconnected(Device device)
		{
			// If the panel is already visible, don't attempt to auto show it.
			if (_panel.IsVisible || !_panel.ShowPanelOnDisconnect)
			{
				return;
			}

			_device = device;

			_messageText.SetLocaleKey(LocaleConstants.BOSE_AR_UNITY_SDK_RECONNECTION_MESSAGE);

			_panel.ShowWithoutSearching();
			_panel.EnableCloseButton();

			Show();
		}

		private void OnDeviceSearching()
		{
			Hide();
		}

		private void OnDeviceConnecting()
		{
			Hide();
		}

		private void OnReconnectButtonClicked()
		{
			Hide();

			if (_device != null)
			{
				_panel.ReconnectToDevice(_device);
				_device = null;
			}
		}

		private void OnSearchButtonClicked()
		{
			Hide();

			_panel.StartSearch();
		}
	}
}
