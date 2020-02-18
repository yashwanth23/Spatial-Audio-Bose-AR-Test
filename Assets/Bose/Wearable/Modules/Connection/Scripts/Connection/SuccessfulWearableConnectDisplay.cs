using System.Collections;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when a device connection attempt has succeeded
	/// </summary>
	internal sealed class SuccessfulWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("Sound Clips")]
		[SerializeField]
		private AudioClip _sfxSuccess;

		[Tooltip("The amount of time to show the success state before hiding this display.")]
		[Range(0, 3f)]
		[SerializeField]
		private float _showSuccessDelay;

		#pragma warning restore 0649

		private WaitForSecondsRealtimeCacheable _wait;

		private bool _isVisible;

		protected override void Awake()
		{
			SetupAudio();

			_wait = new WaitForSecondsRealtimeCacheable(_showSuccessDelay);

			base.Awake();
		}

		private void OnEnable()
		{
			_panel.DeviceConnectSuccess += OnDeviceConnectionSuccess;
			_panel.DeviceDisconnected += OnDeviceDisconnected;
		}

		private void OnDisable()
		{
			_panel.DeviceConnectSuccess -= OnDeviceConnectionSuccess;
			_panel.DeviceDisconnected -= OnDeviceDisconnected;
		}

		private void OnDeviceConnectionSuccess()
		{
			StartCoroutine(ShowSuccess());
		}

		private void OnDeviceDisconnected(Device device)
		{
			// If the device was disconnected while this UI is shown, restart the device search and hide this UI.
			if (_isVisible)
			{
				_panel.StartSearch();

				// Set this to false before hiding so the WearableConnectUIPanel is not hidden.
				_isVisible = false;

				Hide();
			}
		}

		private IEnumerator ShowSuccess()
		{
			PlaySuccessSting();

			Show();

			yield return _wait.Restart();

			Hide();
		}

		protected override void Show()
		{
			_messageText.SetLocaleKey(LocaleConstants.BOSE_AR_UNITY_SDK_CONNECTION_SUCCESS);
			_isVisible = true;
			_panel.DisableCloseButton();

			base.Show();
		}

		protected override void Hide()
		{
			base.Hide();

			// Only hide the WearableConnectUIPanel if this display and the connect UI was previously visible.
			if (_isVisible && _panel.IsVisible)
			{
				_panel.Hide();
			}

			_isVisible = false;
			_messageText.Clear();
		}

		private void PlaySuccessSting()
		{
			_audioControl.PlayOneShot(_sfxSuccess);
		}
	}
}
