using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when a device connection attempt is made
	/// </summary>
	internal sealed class ConnectingWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("Connecting UI Refs")]
		[SerializeField]
		private Button _cancelConnectionButton;

		#pragma warning restore 0649

		protected override void Awake()
		{
			SetupAudio();

			_cancelConnectionButton.onClick.AddListener(OnCancelDeviceConnection);

			_panel.DeviceSearching += OnDeviceSearching;
			_panel.DeviceConnecting += OnDeviceConnecting;
			_panel.FirmwareCheckStarted += OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired += OnDeviceConnectEnded;
			_panel.DeviceConnectFailure += OnDeviceConnectEnded;
			_panel.DeviceConnectSuccess += OnDeviceConnectEnded;

			base.Awake();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_cancelConnectionButton.onClick.RemoveAllListeners();

			_panel.DeviceSearching -= OnDeviceSearching;
			_panel.DeviceConnecting -= OnDeviceConnecting;
			_panel.FirmwareCheckStarted -= OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired -= OnDeviceConnectEnded;
			_panel.DeviceConnectFailure -= OnDeviceConnectEnded;
			_panel.DeviceConnectSuccess -= OnDeviceConnectEnded;

			TeardownAudio();
		}

		private void OnCancelDeviceConnection()
		{
			Hide();

			_wearableControl.CancelDeviceConnection();
		}

		private void OnDeviceSearching()
		{
			Hide();
		}

		private void OnDeviceConnecting()
		{
			Show();
		}

		private void OnDeviceConnectEnded()
		{
			Hide();
		}

		private void OnFirmwareCheckStarted(
			bool isRequired,
			Device device,
			FirmwareUpdateInformation updateInformation)
		{
			Hide();
		}

		protected override void Show()
		{
			_messageText.SetLocaleKey(LocaleConstants.BOSE_AR_UNITY_SDK_CONNECTING_MESSAGE);
			_panel.DisableCloseButton();

			base.Show();
		}
	}
}
