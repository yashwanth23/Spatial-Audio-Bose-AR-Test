using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when a device connection attempt has failed
	/// </summary>
	internal sealed class FailedWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[SerializeField]
		private Button _searchButton;

		#pragma warning restore 0649
		protected override void Awake()
		{
			SetupAudio();

			base.Awake();
		}

		private void OnEnable()
		{
			_panel.DeviceSearching += OnDeviceSearching;
			_panel.DeviceConnectFailure += OnDeviceConnectFailure;
			_panel.OSPermissionFailure += OnOSPermissionFailure;
			_panel.OSServiceFailure += OnOSServiceFailure;
			_panel.Closed += OnPanelClosed;

			_searchButton.onClick.AddListener(OnSearchButtonClicked);
		}

		private void OnDisable()
		{
			_panel.DeviceSearching -= OnDeviceSearching;
			_panel.DeviceConnectFailure -= OnDeviceConnectFailure;
			_panel.OSPermissionFailure -= OnOSPermissionFailure;
			_panel.OSServiceFailure -= OnOSServiceFailure;
			_panel.Closed -= OnPanelClosed;

			_searchButton.onClick.RemoveAllListeners();
		}

		private void OnDeviceSearching()
		{
			Hide();
		}

		private void OnDeviceConnectFailure()
		{
			if (!_panel.IsVisible)
			{
				return;
			}

			_messageText.SetLocaleKey(LocaleConstants.BOSE_AR_UNITY_SDK_CONNECTION_FAILURE);

			Show();
		}

		private void OnOSServiceFailure(OSService service)
		{
			_messageText.SetLocaleKey(
				LocaleConstants.BOSE_AR_UNITY_SDK_SERVICE_FAILURE_MESSAGE,
				service.GetLocaleKey());

			Show();
		}

		private void OnOSPermissionFailure(OSPermission permission)
		{
			_messageText.SetLocaleKey(
				LocaleConstants.BOSE_AR_UNITY_SDK_PERMISSION_FAILURE_MESSAGE,
				permission.GetLocaleKey());

			Show();
		}

		private void OnPanelClosed(WearableConnectUIResult value)
		{
			Hide();
		}

		private void OnSearchButtonClicked()
		{
			Hide();

			_panel.StartSearch();
		}

		protected override void Show()
		{
			_panel.EnableCloseButton();

			base.Show();
		}
	}
}
