using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the connection screen UI and allows for easy connecting to devices.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup), typeof(Canvas))]
	public sealed class WearableConnectUIPanel : Singleton<WearableConnectUIPanel>, ISelectionController<Device>
	{
		#pragma warning disable 0067

		/// <summary>
		/// Invoked when this UI is closed; a status indicating the result of the user connection attempt is
		/// passed as a parameter.
		/// </summary>
		public event Action<WearableConnectUIResult> Closed;

		/// <summary>
		/// Invoked when an operating-system permission is required, but not granted by a user.
		/// </summary>
		public event Action<OSPermission> OSPermissionRequired;

		/// <summary>
		/// Invoked when an operating-system service is required, but not enabled by the user.
		/// </summary>
		public event Action<OSService> OSServiceRequired;

		/// <summary>
		/// Invoked when an operating system permission is requested, but not granted.
		/// </summary>
		public event Action<OSPermission> OSPermissionFailure;

		/// <summary>
		/// Invoked when an operating system service is requested, but not enabled.
		/// </summary>
		public event Action<OSService> OSServiceFailure;

		/// <summary>
		/// Invoked when a check for SDK-required operating system permissions and services has passed.
		/// </summary>
		public event Action OSRequirementsMet;

		/// <summary>
		/// Invoked when a device search has started.
		/// </summary>
		public event Action DeviceSearching;

		/// <summary>
		/// Invoked when a device search started locally results in devices found.
		/// </summary>
		public event Action<Device[]> DevicesFound;

		/// <summary>
		/// Invoked when an attempt has been made to connect to a device.
		/// </summary>
		public event Action DeviceConnecting;

		/// <summary>
		/// Invoked when a device has been successfully connected to.
		/// </summary>
		public event Action DeviceConnectSuccess;

		/// <summary>
		/// Invoked when a device connection attempt has failed.
		/// </summary>
		public event Action DeviceConnectFailure;

		/// <summary>
		/// Invoked when a device has become disconnected.
		/// </summary>
		public event Action<Device> DeviceDisconnected;

		/// <summary>
		/// Invoked when the application gains or loses focus. (via Unity's OnApplicationFocus)
		/// </summary>
		public event Action<bool> AppFocusChanged;

		/// <summary>
		/// Invoked when a device has successfully connected via BLE and a firmware check is needed.
		/// </summary>
		internal event Action<bool, Device, FirmwareUpdateInformation> FirmwareCheckStarted;

		/// <summary>
		/// Invoked when the device has sufficient firmware to begin secure pairing.
		/// </summary>
		internal event Action DeviceSecurePairingRequired;

		#pragma warning restore 0067

		/// <summary>
		/// Return true if the connection UI should be shown when the device is disconnected, otherwise false.
		/// </summary>
		public bool ShowPanelOnDisconnect
		{
			get { return _showOnDisconnect; }
			set { _showOnDisconnect = value; }
		}

		/// <summary>
		/// Returns true if the panel is already visible, otherwise false.
		/// </summary>
		internal bool IsVisible
		{
			get
			{
				return _isVisible;
			}
		}

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

		/// <summary>
		/// The shared text field used to provide context for the current panel.
		/// </summary>
		[SerializeField]
		private LocalizedText _messageText;

		[SerializeField]
		private Button _closeButton;

		[SerializeField]
		private Button _closeButtonImage;

		[Header("UI Settings"), Space(5)]
		[SerializeField]
		private int _sortOrder;

		[Header("Options"), Space(5)]
		[SerializeField]
		private bool _showOnStart;

		[SerializeField]
		private bool _showOnDisconnect;

		[SerializeField]
		private bool _autoReconnectOnShow;

		[SerializeField]
		[Range(0.1f, 5f)]
		private float _autoReconnectTimeout;

		#pragma warning restore 0649

		private Coroutine _checkForPermissionsAndTrySearchCoroutine;

		private bool _userDidOpen;
		private bool _isVisible;
		private Action _onClose;
		private WearableControl _wearableControl;
		private EventSystem _eventSystem;
		private bool _didAttemptReconnect;

		private const string CANNOT_FIND_EVENT_SYSTEM_WARNING =
			"[Bose Wearable] Cannot find an EventSystem. WearableConnectUIPanel will not detect any input.";

		/// <summary>
		/// Initialize any local state or listeners
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			// If we haven't prepared a singleton yet, set this one up.
			if (IsSingletonInstance)
			{
				_wearableControl = WearableControl.Instance;

				_canvas.enabled = false;
				_canvasGroup.alpha = 0f;
				_messageText.Clear();
				_closeButton.onClick.AddListener(OnCloseButtonClicked);

				DisableCloseButton();

				ToggleLockScreen(false);
			}
		}

		protected override void OnDestroy()
		{
			if (IsSingletonInstance)
			{
				base.OnDestroy();

				_closeButton.onClick.RemoveAllListeners();
			}
		}

		private void Start()
		{
			if (_showOnStart)
			{
				Show();
			}
		}

		private void OnEnable()
		{
			_wearableControl.ConnectionStatusChanged += OnConnectionStatusChanged;
			_wearableControl.AppFocusChanged += OnAppFocusChanged;
		}

		private void OnDisable()
		{
			if (IsSingletonInstance)
			{
				_wearableControl.ConnectionStatusChanged -= OnConnectionStatusChanged;
				_wearableControl.AppFocusChanged -= OnAppFocusChanged;
			}
		}

		#if UNITY_EDITOR

		private void Reset()
		{
			_autoReconnectTimeout = WearableConstants.DEFAULT_AUTO_RECONNECT_TIMEOUT;
		}

		#endif

		/// <summary>
		/// Initiates the check for firmware status to begin.
		/// </summary>
		private void OnFirmwareUpdateCheck(bool isRequired, Device device, FirmwareUpdateInformation updateInformation)
		{
			ToggleLockScreen(true);

			if (FirmwareCheckStarted != null)
			{
				FirmwareCheckStarted.Invoke(isRequired, device, updateInformation);
			}
		}

		/// <summary>
		/// Invoked when the user presses the close button
		/// </summary>
		private void OnCloseButtonClicked()
		{
			Hide();
		}

		/// <summary>
		/// On receiving new device updates, update the list of devices shown.
		/// </summary>
		/// <param name="devices"></param>
		private void OnDevicesUpdated(Device[] devices)
		{
			if (DevicesFound != null)
			{
				DevicesFound(devices);
			}
		}

		/// <summary>
		/// On receiving a connection status change from <see cref="WearableControl"/>, invoke the appropriate
		/// local event.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="device"></param>
		private void OnConnectionStatusChanged(ConnectionStatus status, Device? device)
		{
			switch (status)
			{
				case ConnectionStatus.Disconnected:
					if (DeviceDisconnected != null)
					{
						DeviceDisconnected.Invoke(device.GetValueOrDefault());
					}
					break;
				case ConnectionStatus.Searching:
					if (DeviceSearching != null)
					{
						DeviceSearching.Invoke();
					}
					break;
				case ConnectionStatus.AutoReconnect:
				case ConnectionStatus.Connecting:
					ToggleLockScreen(true);
					if (DeviceConnecting != null)
					{
						DeviceConnecting.Invoke();
					}
					break;
				case ConnectionStatus.SecurePairingRequired:
					if (DeviceSecurePairingRequired != null)
					{
						DeviceSecurePairingRequired.Invoke();
					}
					break;
				case ConnectionStatus.FirmwareUpdateAvailable:
					OnFirmwareUpdateCheck(
						false,
						device.GetValueOrDefault(),
						_wearableControl.ActiveProvider.GetFirmwareUpdateInformation());
					break;
				case ConnectionStatus.FirmwareUpdateRequired:
					OnFirmwareUpdateCheck(
						true,
						device.GetValueOrDefault(),
						_wearableControl.ActiveProvider.GetFirmwareUpdateInformation());
					break;
				case ConnectionStatus.Connected:
					ToggleLockScreen(true);
					if (DeviceConnectSuccess != null)
					{
						DeviceConnectSuccess.Invoke();
					}
					break;
				case ConnectionStatus.Failed:
					ToggleLockScreen(true);
					if (_wearableControl.LastPermissionRequestedForUser.HasValue && OSPermissionFailure != null)
					{
						OSPermissionFailure.Invoke(_wearableControl.LastPermissionRequestedForUser.Value);
					}
					else if (_wearableControl.LastServiceRequestedForUser.HasValue && OSServiceFailure != null)
					{
						OSServiceFailure.Invoke(_wearableControl.LastServiceRequestedForUser.Value);
					}
					else if (DeviceConnectFailure != null)
					{
						DeviceConnectFailure.Invoke();
					}
					break;
				case ConnectionStatus.Cancelled:
					StartSearch();
					break;
				case ConnectionStatus.PermissionRequired:
					ToggleLockScreen(true);
					if (_wearableControl.LastPermissionRequestedForUser.HasValue && OSPermissionRequired != null)
					{
						OSPermissionRequired.Invoke(_wearableControl.LastPermissionRequestedForUser.Value);
					}
					break;
				case ConnectionStatus.ServiceRequired:
					ToggleLockScreen(true);
					if (_wearableControl.LastServiceRequestedForUser.HasValue && OSServiceRequired != null)
					{
						OSServiceRequired.Invoke(_wearableControl.LastServiceRequestedForUser.Value);
					}
					break;
				case ConnectionStatus.RequirementsMet:
					ToggleLockScreen(true);
					break;
				default:
					throw new ArgumentOutOfRangeException("status", status, null);
			}
		}

		/// <summary>
		/// Serves as a relay from the WearableControl's internal AppFocusChanged which is subsequently triggered
		/// from Unity's OnApplicationFocus. This event primarily serves to handle the special case where
		/// a user updating their firmware will currently require switching to a different application to complete
		/// the firmware update. For more information on this case, please see
		/// <see cref="FirmwareWearableConnectDisplay"/>.
		/// </summary>
		/// <param name="hasFocus"></param>
		private void OnAppFocusChanged(bool hasFocus)
		{
			if (AppFocusChanged != null)
			{
				AppFocusChanged.Invoke(hasFocus);
			}
		}

		/// <summary>
		/// Show the UI and kick off the search for devices.
		/// </summary>
		public void Show()
		{
			WarnIfNoEventSystemPresent();

			// if we are not already visible, clear the auto-reconnection flag so we
			// attempt that path if the developer desires it.
			if (!_isVisible)
			{
				_didAttemptReconnect = false;
			}

			_userDidOpen = true;
			_isVisible = true;
			_canvas.sortingOrder = _sortOrder;

			StartSearch();
		}

		/// <summary>
		/// Show the Connection UI Panel without immediately searching.
		/// </summary>
		internal void ShowWithoutSearching()
		{
			ToggleLockScreen(true);

			WarnIfNoEventSystemPresent();

			_userDidOpen = true;
			_isVisible = true;
			_canvas.enabled = true;
			_canvasGroup.alpha = 1f;
		}

		/// <summary>
		/// Enables the close button, setting the button sprite to use a default color tint.
		/// </summary>
		internal void EnableCloseButton()
		{
			EnableCloseButton(Color.black);
		}

		/// <summary>
		/// Enables the close button, setting the button sprite to use a <paramref name="color"/> tint.
		/// </summary>
		internal void EnableCloseButton(Color color)
		{
			_closeButton.image.color = color;
			_closeButton.gameObject.SetActive(true);
		}

		/// <summary>
		/// Disables the close button.
		/// </summary>
		internal void DisableCloseButton()
		{
			_closeButton.gameObject.SetActive(false);
		}

		/// <summary>
		/// Enables or disables user input to this UI panel.
		/// </summary>
		/// <param name="isInteractable"></param>
		private void ToggleLockScreen(bool isInteractable)
		{
			_canvasGroup.interactable = isInteractable;
		}

		/// <summary>
		/// Hides the UI and stops the search for devices.
		/// </summary>
		public void Hide()
		{
			ToggleLockScreen(false);

			_wearableControl.StopSearchingForDevices();

			_isVisible = false;
			_canvas.enabled = false;
			_canvasGroup.alpha = 0f;
			if (_userDidOpen)
			{
				var connectionResult = _wearableControl.ConnectedDevice.HasValue
					? WearableConnectUIResult.Successful
					: WearableConnectUIResult.Cancelled;
			ToggleLockScreen(false);
				if (Closed != null)
				{
					Closed.Invoke(connectionResult);
				}
			}

			_userDidOpen = false;
		}

		/// <summary>
		/// Initiates a search for devices.
		/// </summary>
		internal void StartSearch()
		{
			_canvas.enabled = true;
			_canvasGroup.alpha = 1f;

			DisableCloseButton();

			_wearableControl.DisconnectFromDevice();

			var shouldReconnect = _autoReconnectOnShow && !_didAttemptReconnect;
			_wearableControl.SearchForDevices(OnDevicesUpdated, shouldReconnect, _autoReconnectTimeout);
			_didAttemptReconnect = true;
		}

		/// <summary>
		/// Attempt to reconnect to a Device.
		/// </summary>
		/// <param name="device"></param>
		internal void ReconnectToDevice(Device? device)
		{
			if (device.HasValue)
			{
				OnSelect(device.Value);
			}
		}

		private void WarnIfNoEventSystemPresent()
		{
			if (_eventSystem == null)
			{
				_eventSystem = FindObjectOfType<EventSystem>();

				if (_eventSystem == null)
				{
					Debug.LogWarning(CANNOT_FIND_EVENT_SYSTEM_WARNING, this);
				}
			}
		}

		#region ISelectionController

		public void OnSelect(Device value)
		{
			_wearableControl.ConnectToDevice(value);
		}
		#endregion
	}
}
