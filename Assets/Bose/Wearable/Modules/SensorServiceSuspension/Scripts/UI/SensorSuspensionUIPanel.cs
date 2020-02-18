using System;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	[RequireComponent(typeof(Canvas))]
	public sealed class SensorSuspensionUIPanel : Singleton<SensorSuspensionUIPanel>
	{
		#pragma warning disable 0649

		/// <summary>
		/// The Canvas on the root UI element.
		/// </summary>
		[Header("UI Refs")]
		[SerializeField]
		private Canvas _canvas;

		/// <summary>
		/// The CanvasGroup on the root UI element of this Canvas.
		/// </summary>
		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private Button _launchExternalAppButton;

		[SerializeField]
		private Button _continueWithoutBoseButton;

		[SerializeField]
		private LocalizedText _launchExternalAppButtonText;

		[SerializeField]
		private LocalizedText _warningText;

		#pragma warning restore 0649

		private WearableControl _wearableControl;
		private SensorServiceSuspendedReason _reason;

		protected override void Awake()
		{
			base.Awake();

			_wearableControl = WearableControl.Instance;
			_wearableControl.DeviceConnected += OnDeviceConnected;
			_wearableControl.DeviceDisconnected += OnDeviceDisconnected;
			_wearableControl.SensorServiceResumed += OnSensorServiceResumed;
			_wearableControl.SensorServiceSuspended += OnSensorServiceSuspended;

			_launchExternalAppButton.onClick.AddListener(LaunchExternalApp);
			_continueWithoutBoseButton.onClick.AddListener(ContinueWithoutBoseAR);

			Hide();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_wearableControl.DeviceConnected -= OnDeviceConnected;
			_wearableControl.DeviceDisconnected -= OnDeviceDisconnected;
			_wearableControl.SensorServiceResumed -= OnSensorServiceResumed;
			_wearableControl.SensorServiceSuspended -= OnSensorServiceSuspended;

			_launchExternalAppButton.onClick.RemoveListener(LaunchExternalApp);
			_continueWithoutBoseButton.onClick.RemoveListener(ContinueWithoutBoseAR);
		}

		private void Show()
		{
			_canvas.enabled = true;
			_canvasGroup.alpha = 1;
			_canvasGroup.interactable = true;

			_launchExternalAppButtonText.SetLocaleKey(
				LocaleConstants.CONNECTION_TASK_OPEN_THE_APP,
				LocaleConstants.FIRMWARE_UPDATE_APP_BOSE_CONNECT);

			// We allow the user to enter Bose Connect if we don't know for certain the user is currently in a
			// VPA suspension. Since VPA suspensions resolve on their own and often in short order, it would be
			// a worse experience for a user to enter Bose Connect in an attempt to resolve that suspension.
			bool allowBoseConnect = _reason != SensorServiceSuspendedReason.VoiceAssistantInUse;

			// If we know for certain that the user is in music sharing or multipoint, then we allow them
			// to continue without Bose AR, effectively killing their Bose AR connection.
			bool allowContinueWithout = (_reason == SensorServiceSuspendedReason.MusicSharingActive ||
			                            _reason == SensorServiceSuspendedReason.MultipointConnectionActive);

			_launchExternalAppButton.gameObject.SetActive(allowBoseConnect);
			_continueWithoutBoseButton.gameObject.SetActive(allowContinueWithout);

			_warningText.SetLocaleKey(_reason.GetLocaleKey());
		}

		private void Hide()
		{
			_canvas.enabled = false;
			_canvasGroup.alpha = 0;
			_canvasGroup.interactable = false;
		}

		private void LaunchExternalApp()
		{
			PlatformTools.LaunchBoseConnectApp();
		}

		private void ContinueWithoutBoseAR()
		{
			_wearableControl.DisconnectFromDevice();
		}

		private void OnDeviceConnected(Device device)
		{
			if (device.deviceStatus.ServiceSuspended)
			{
				_reason = device.deviceStatus.GetServiceSuspendedReason();

				Show();
			}
		}

		private void OnDeviceDisconnected(Device device)
		{
			Hide();
		}

		private void OnSensorServiceResumed()
		{
			Hide();
		}

		private void OnSensorServiceSuspended(SensorServiceSuspendedReason reason)
		{
			_reason = reason;

			Show();
		}
	}
}
