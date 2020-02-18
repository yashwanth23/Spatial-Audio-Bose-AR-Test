using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when a required permission is not granted by the user.
	/// </summary>
	internal sealed class PermissionRequestedConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Button _grantPermissionButton;

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

		private OSPermission _requiredPermission;

		protected override void Awake()
		{
			base.Awake();

			_panel.OSPermissionRequired += OnOSPermissionRequired;
			_panel.OSServiceRequired += OnOSServiceRequired;
			_panel.DeviceSearching += Hide;
			_panel.DeviceConnectFailure += Hide;
			_panel.DeviceDisconnected += OnDeviceDisconnected;
			_panel.OSPermissionFailure += OnOSPermissionFailure;
			_panel.OSServiceFailure += OnOSServiceFailure;

			_grantPermissionButton.onClick.AddListener(OnGrantPermissionClicked);
			_continueWithoutButton.onClick.AddListener(OnContinueWithoutBoseARClicked);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_panel.OSPermissionRequired -= OnOSPermissionRequired;
			_panel.OSServiceRequired -= OnOSServiceRequired;
			_panel.DeviceSearching -= Hide;
			_panel.DeviceConnectFailure -= Hide;
			_panel.DeviceDisconnected -= OnDeviceDisconnected;
			_panel.OSPermissionFailure -= OnOSPermissionFailure;
			_panel.OSServiceFailure -= OnOSServiceFailure;

			_grantPermissionButton.onClick.RemoveListener(OnGrantPermissionClicked);
			_continueWithoutButton.onClick.RemoveListener(OnContinueWithoutBoseARClicked);
		}

		private void OnOSServiceRequired(OSService service)
		{
			Hide();
		}

		private void OnOSPermissionRequired(OSPermission permission)
		{
			_requiredPermission = permission;
			_messageText.Clear();

			if (permission == OSPermission.Bluetooth)
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_AUTHORIZATION_REQUIRED_TITLE);
				_warningText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_AUTHORIZATION_REQUIRED_MESSAGE);
				_warningIconImage.sprite = _bluetoothWarningSprite;
			}
			else
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_LOCATION_AUTHORIZATION_REQUIRED_TITLE);
				_warningText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_LOCATION_AUTHORIZATION_REQUIRED_MESSAGE);
				_warningIconImage.sprite = _generalWarningIcon;
			}

			Show();
		}

		private void OnDeviceDisconnected(Device obj)
		{
			Hide();
		}

		private void OnOSServiceFailure(OSService service)
		{
			Hide();
		}

		private void OnOSPermissionFailure(OSPermission permission)
		{
			Hide();
		}

		private void OnGrantPermissionClicked()
		{
			Hide();

			_wearableControl.RequestPermission(_requiredPermission);
		}

		private void OnContinueWithoutBoseARClicked()
		{
			Hide();

			_wearableControl.DenyPermissionOrService();
		}
	}
}
