using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when a required service needs to be enabled.
	/// </summary>
	internal sealed class ServiceRequestedConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Button _enableServiceButton;

		[SerializeField]
		private Button _continueWithoutButton;

		[SerializeField]
		private Image _warningIconImage;

		[SerializeField]
		private LocalizedText _headerText;

		[SerializeField]
		private LocalizedText _warningText;

		[Space(5)]
		[Header("Data")]
		[SerializeField]
		private Sprite _generalWarningIcon;

		[SerializeField]
		private Sprite _bluetoothWarningSprite;

		#pragma warning restore 0649

		#pragma warning disable 0414

		private OSService _requiredService;

		#pragma warning restore 0414

		protected override void Awake()
		{
			base.Awake();

			_panel.OSPermissionRequired += OnOSPermissionRequired;
			_panel.OSServiceRequired += OnOSServiceRequired;
			_panel.DeviceSearching += Hide;
			_panel.DeviceConnectFailure += Hide;
			_panel.DeviceSearching += Hide;
			_panel.OSPermissionFailure += OnOSPermissionFailure;
			_panel.OSServiceFailure += OnOSServiceFailure;

			_enableServiceButton.onClick.AddListener(OnTryAgainClicked);
			_continueWithoutButton.onClick.AddListener(OnContinueWithoutBoseARClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_panel.OSPermissionRequired -= OnOSPermissionRequired;
			_panel.OSServiceRequired -= OnOSServiceRequired;
			_panel.DeviceSearching -= Hide;
			_panel.DeviceConnectFailure -= Hide;
			_panel.DeviceSearching -= Hide;
			_panel.OSPermissionFailure -= OnOSPermissionFailure;
			_panel.OSServiceFailure -= OnOSServiceFailure;

			_enableServiceButton.onClick.RemoveListener(OnTryAgainClicked);
			_continueWithoutButton.onClick.RemoveListener(OnContinueWithoutBoseARClicked);
		}

		private void OnOSPermissionRequired(OSPermission permission)
		{
			Hide();
		}

		private void OnOSServiceRequired(OSService service)
		{
			_requiredService = service;
			_messageText.Clear();

			if (service == OSService.Bluetooth)
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_NO_BLUETOOTH_TITLE);
				_warningText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_NO_BLUETOOTH_MESSAGE);
				_warningIconImage.sprite = _bluetoothWarningSprite;
			}
			else
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_NO_LOCATION_TITLE);
				_warningText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_NO_LOCATION_MESSAGE);
				_warningIconImage.sprite = _generalWarningIcon;
			}

			Show();
		}

		private void OnTryAgainClicked()
		{
			Hide();

			_panel.StartSearch();
		}

		private void OnOSServiceFailure(OSService service)
		{
			Hide();
		}

		private void OnOSPermissionFailure(OSPermission permission)
		{
			Hide();
		}

		private void OnContinueWithoutBoseARClicked()
		{
			Hide();

			_wearableControl.DenyPermissionOrService();
		}
	}
}
