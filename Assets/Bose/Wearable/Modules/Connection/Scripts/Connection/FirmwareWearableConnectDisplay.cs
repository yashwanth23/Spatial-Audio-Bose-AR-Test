using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	internal sealed class FirmwareWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("UI Refs"), Space(5)]
		[SerializeField]
		private Image _appGroupIcon;

		[SerializeField]
		private LocalizedText _headerText;

		[SerializeField]
		private LocalizedText _scrollText;

		[SerializeField]
		private Button _updateButton;

		[SerializeField]
		private LocalizedText _updateButtonText;

		[SerializeField]
		private Button _continueButton;

		[SerializeField]
		private LocalizedText _continueButtonText;

		[Header("Data"), Space(5)]
		[SerializeField]
		private Sprite _boseConnectIcon;

		[SerializeField]
		private Sprite _boseMusicIcon;

		#pragma warning restore 0649

		private bool _isVisible;
		private bool _clickedUpdateButton;
		private FirmwareUpdateInformation _updateInformation;

		protected override void Awake()
		{
			base.Awake();

			_panel.DeviceSearching += OnDeviceSearching;
			_panel.FirmwareCheckStarted += OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired += OnDeviceSecurePairingRequired;
			_panel.DeviceConnectFailure += OnDeviceConnectFailure;

			_updateButton.onClick.AddListener(OnPrimaryButtonClick);
			_continueButton.onClick.AddListener(OnContinueButtonClick);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_panel.DeviceSearching -= OnDeviceSearching;
			_panel.FirmwareCheckStarted -= OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired -= OnDeviceSecurePairingRequired;
			_panel.DeviceConnectFailure -= OnDeviceConnectFailure;

			_updateButton.onClick.RemoveAllListeners();
			_continueButton.onClick.RemoveAllListeners();
		}

		protected override void Show()
		{
			base.Show();

			_clickedUpdateButton = false;
			_isVisible = true;
		}

		protected override void Hide()
		{
			base.Hide();

			_isVisible = false;
		}

		private void OnDeviceSearching()
		{
			Hide();
		}

		private void OnFirmwareCheckStarted(
			bool isRequired,
			Device device,
			FirmwareUpdateInformation updateInformation)
		{
			_messageText.Clear();

			_updateInformation = updateInformation;

			var appLocaleKey = GetAppSpecificLocaleKey(updateInformation.icon);
			if (isRequired)
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_UPDATE_REQUIRED_TITLE);
				_scrollText.SetLocaleKey(
					LocaleConstants.CONNECTION_TASK_UPDATE_REQUIRED_MESSAGE,
					appLocaleKey);
				_updateButtonText.SetLocaleKey(
					LocaleConstants.CONNECTION_TASK_OPEN_THE_APP,
					appLocaleKey);
				_continueButtonText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_CONTINUE_WITHOUT_BOSE_AR);
			}
			else
			{
				_headerText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_UPDATE_AVAILABLE_TITLE);
				_scrollText.SetLocaleKey(
					LocaleConstants.CONNECTION_TASK_UPDATE_AVAILABLE_MESSAGE,
					appLocaleKey);
				_updateButtonText.SetLocaleKey(
					LocaleConstants.CONNECTION_TASK_OPEN_THE_APP,
					appLocaleKey);
				_continueButtonText.SetLocaleKey(LocaleConstants.CONNECTION_TASK_CONTINUE_WITHOUT_UPDATING_TITLE);
			}

			switch (updateInformation.icon)
			{
				case BoseUpdateIcon.Connect:
					_appGroupIcon.sprite = _boseConnectIcon;
					break;
				case BoseUpdateIcon.Music:
					_appGroupIcon.sprite = _boseMusicIcon;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Show();
		}

		private void OnDeviceSecurePairingRequired()
		{
			Hide();
		}

		private void OnDeviceConnectFailure()
		{
			Hide();
		}

		private void OnPrimaryButtonClick()
		{
			_clickedUpdateButton = true;

			_wearableControl.ActiveProvider.SelectFirmwareUpdateOption(GetIndex(AlertStyle.Affirmative));
			_wearableControl.DisconnectFromDevice();
		}

		private void OnContinueButtonClick()
		{
			_wearableControl.ActiveProvider.SelectFirmwareUpdateOption(GetIndex(AlertStyle.Negative));

			Hide();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			// If we've gained back app focus and this panel is the last thing the user saw before they left the app
			if (hasFocus && _isVisible && _clickedUpdateButton)
			{
				Hide();

				_panel.StartSearch();
			}
		}

		private int GetIndex(AlertStyle style)
		{
			var index = 0;
			for (var i = 0; i < _updateInformation.options.Length; i++)
			{
				if (_updateInformation.options[i].style == style)
				{
					index = i;
				}
			}

			return index;
		}

		/// <summary>
		/// Returns the appropriate localization key to use for the app based on the intended update icon.
		/// </summary>
		/// <param name="updateIcon"></param>
		/// <returns></returns>
		private static string GetAppSpecificLocaleKey(BoseUpdateIcon updateIcon)
		{
			return updateIcon == BoseUpdateIcon.Connect
				? LocaleConstants.FIRMWARE_UPDATE_APP_BOSE_CONNECT
				: LocaleConstants.FIRMWARE_UPDATE_APP_BOSE_MUSIC;
		}
	}
}
