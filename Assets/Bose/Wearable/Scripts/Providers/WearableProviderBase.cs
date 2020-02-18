using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bose.Wearable
{
	[Serializable]
	public abstract class WearableProviderBase
	{
		/// <summary>
		/// Invoked when the connection status changes, such as when a device connects, disconnects, requires a
		/// firmware update, etc. Not all statuses will provide an associated device.
		/// </summary>
		internal event Action<ConnectionStatus, Device?> ConnectionStatusChanged;

		/// <summary>
		/// Invoked when there are sensor updates from the Wearable device.
		/// </summary>
		internal event Action<SensorFrame> SensorsUpdated;

		/// <summary>
		/// Invoked when there are gesture updates from the Wearable device.
		/// </summary>
		internal event Action<GestureId> GestureDetected;

		/// <summary>
		/// Invoked when a configuration attempt was successful.
		/// </summary>
		internal event Action ConfigurationSucceeded;

		/// <summary>
		/// Invoked when a configuration attempt failed.
		/// </summary>
		internal event Action<ConfigStatus, ConfigStatus> ConfigurationFailed;

		/// <summary>
		/// Invoked when service is suspended on the device.
		/// </summary>
		internal event Action<SensorServiceSuspendedReason> SensorServiceSuspended;

		/// <summary>
		/// Invoked when service is resumed.
		/// </summary>
		internal event Action SensorServiceResumed;

		/// <summary>
		/// Invoked when the current ANR mode has changed.
		/// </summary>
		internal event Action<ActiveNoiseReductionMode> ActiveNoiseReductionModeChanged;

		/// <summary>
		/// Invoked when either the CNC level or state has changed.
		/// </summary>
		internal event Action<int, bool> ControllableNoiseCancellationInfoChanged;

		/// <summary>
		/// Whether or not the provider has been initialized
		/// </summary>
		internal bool Initialized
		{
			get { return _initialized; }
		}

		protected bool _initialized;

		/// <summary>
		/// Whether or not the provider is enabled
		/// </summary>
		internal bool Enabled
		{
			get { return _enabled; }
		}

		protected bool _enabled;

		/// <summary>
		/// The last reported value for the sensors.
		/// </summary>
		internal SensorFrame LastSensorFrame
		{
			get { return _lastSensorFrame; }
		}

		protected SensorFrame _lastSensorFrame;

		/// <summary>
		/// A list of SensorFrames returned from the plugin bridge in order from oldest to most recent.
		/// </summary>
		internal List<SensorFrame> CurrentSensorFrames
		{
			get { return _currentSensorFrames; }
		}

		protected List<SensorFrame> _currentSensorFrames;

		/// <summary>
		/// A list of GestureData returned from the bridge in order from oldest to most recent.
		/// The list is empty if no gestures were returned in the most recent update.
		/// </summary>
		internal List<GestureData> CurrentGestureData
		{
			get { return _currentGestureData; }
		}

		protected List<GestureData> _currentGestureData;

		/// <summary>
		/// The Wearable device that is currently connected in Unity.
		/// </summary>
		internal Device? ConnectedDevice
		{
			get { return _connectedDevice; }
		}

		protected Device? _connectedDevice;

		/// <summary>
		/// Whether or not the user has requested fresh device configuration, and is waiting on a response.
		/// </summary>
		protected bool WaitingForDeviceConfig
		{
			get { return _waitingForDeviceConfig; }
		}

		private bool _waitingForDeviceConfig;

		/// <summary>
		/// Currently waiting for a response to an outstanding intent validation request.
		/// </summary>
		public bool WaitingForIntentValidation
		{
			get { return _waitingForIntentValidation; }
		}

		private bool _waitingForIntentValidation;

		/// <summary>
		/// Returns true if currently waiting for a write to either the ANR or CNC feature configuration to complete, otherwise false.
		/// </summary>
		protected bool WaitingForAnrCncWriteComplete
		{
			get { return _waitingForAnrCncWriteComplete; }
		}

		private bool _waitingForAnrCncWriteComplete;

		/// <summary>
		/// If non-null, this will be initialized with the specific <see cref="OSPermission"/> value that the user
		/// was last prompted to grant and was not granted or was denied. This will only be initialized if
		/// <see cref="ConnectionStatusChanged"/> was invoked with parameter
		/// <see cref="Wearable.ConnectionStatus.PermissionRequired"/>.
		/// </summary>
		internal OSPermission? LastPermissionRequestedForUser
		{
			get
			{
				return _lastPermissionRequestedForUser;
			}
		}

		protected OSPermission? _lastPermissionRequestedForUser;

		/// <summary>
		/// Returns true if the last permission checked is granted, otherwise false.
		/// </summary>
		internal bool LastPermissionCheckedIsGranted
		{
			get { return _lastPermissionCheckedIsGranted; }
		}

		protected bool _lastPermissionCheckedIsGranted;

		/// <summary>
		/// If non-null, this will be initialized with the specific <see cref="OSService"/> value that the user
		/// was last prompted to enable and was not enable. This will only be initialized if <see cref="ConnectionStatusChanged"/>
		/// was invoked with parameter <see cref="Wearable.ConnectionStatus.ServiceRequired"/>.
		/// </summary>
		internal OSService? LastServiceRequestedForUser
		{
			get { return _lastServiceRequestedForUser; }
		}

		protected OSService? _lastServiceRequestedForUser;

		/// <summary>
		/// Returns true if the last service checked is enabled, otherwise false.
		/// </summary>
		internal bool LastServiceCheckedIsEnabled
		{
			get { return _lastServiceCheckedIsEnabled; }
		}

		protected bool _lastServiceCheckedIsEnabled;

		/// <summary>
		/// An event holding any callbacks that have subscribed to the request for new configuration.
		/// </summary>
		private event Action<WearableDeviceConfig> DeviceConfigRequestSubscribers;

		/// <summary>
		/// An event holding callbacks that have subscribed for a list of devices during searching.
		/// </summary>
		private event Action<Device[]> DeviceSearchCallback;

		/// <summary>
		/// The provider's connection status.
		/// </summary>
		public ConnectionStatus ConnectionStatus
		{
			get { return _connectionStatus; }
		}

		protected ConnectionStatus _connectionStatus;

		/// <summary>
		/// Should the provider attempt to reconnect to the last successfully connected device?
		/// </summary>
		protected bool _autoReconnect;

		/// <summary>
		/// How long should we search for the last successfully connected device before giving
		/// the ability to switch back to searching for devices.
		/// </summary>
		protected float _autoReconnectTimeout;

		/// <summary>
		/// Should the provider attempt to skip common prompts during an auto-reconnect attempt?
		/// </summary>
		protected bool _autoReconnectWithoutPrompts;

		/// <summary>
		/// The UID of the last successfully connected (including secure connection, firmware updates, and
		/// intent validation) device. Stored and accessed through PlayerPrefs.
		/// </summary>
		internal string LastConnectedDeviceUID
		{
			get { return PlayerPrefs.GetString(WearableConstants.PREF_LAST_CONNECTED_DEVICE_UID, string.Empty); }
			set { PlayerPrefs.SetString(WearableConstants.PREF_LAST_CONNECTED_DEVICE_UID, value);}
		}

		/// <summary>
		/// Enable additional debug logging for the provider.
		/// </summary>
		[SerializeField]
		protected bool _debugLogging;

		internal void ConfigureDebugLogging()
		{
			SetDebugLogging(_debugLogging ? LogLevel.Verbose : LogLevel.Error);
		}

		/// <summary>
		/// Notifies the provider to provide more verbose logging.
		/// </summary>
		/// <param name="logLevel"></param>
		internal abstract void SetDebugLogging(LogLevel logLevel);

		/// <summary>
		/// Returns the <see cref="DeviceConnectionInfo"/> for the current connection
		/// </summary>
		/// <returns></returns>
		public virtual DeviceConnectionInfo GetDeviceConnectionInfo()
		{
			return new DeviceConnectionInfo();
		}

		/// <summary>
		/// <see cref="_lastPermissionCheckedIsGranted"/> is set to true if the user has granted the <see cref="OSPermissionFlags"/> <paramref name="permission"/>,
		/// otherwise false if the user has not granted the permission. If not granted,
		/// <see cref="ConnectionStatusChanged"/> will be invoked with <seealso cref="ConnectionStatus.PermissionRequest"/>
		/// as the passed value.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		internal abstract IEnumerator ValidatePermissionIsGranted(OSPermission permission);

		/// <summary>
		/// <see name="_isServiceEnabled"/> is set to true if the user has enabled the <see cref="OSServiceFlags"/> <paramref name="service"/>,
		/// otherwise false if the user has not enabled it.If not granted,
		/// <see cref="ConnectionStatusChanged"/> will be invoked with <seealso cref="ConnectionStatus.ServiceRequest"/>
		/// as the passed value.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		internal abstract IEnumerator ValidateServiceIsEnabled(OSService service);

		/// <summary>
		/// Asynchronously requests the <see cref="OSPermission"/> <paramref name="permission"/>. This should
		/// yield until the user has confirmed or denied the permission or returned to the application.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		internal abstract IEnumerator RequestPermissionCoroutine(OSPermission permission);

		/// <summary>
		/// Invoked when a user has denied granting or enabling a permission or service.
		/// </summary>
		internal void DenyPermissionOrService()
		{
			OnConnectionStatusChanged(ConnectionStatus.Failed);
		}

		/// <summary>
		/// Changes the connection status to indicate that all OS requirements had been met.
		/// </summary>
		internal void SetOSRequirementsAsMet()
		{
			OnConnectionStatusChanged(ConnectionStatus.RequirementsMet);
		}

		/// <summary>
		/// Returns the proper text description of a permission prompt.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		internal virtual string GetPermissionRequiredText(OSPermission permission)
		{
			return string.Empty;
		}

		/// <summary>
		/// Returns the proper text description of a permission prompt.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		internal virtual string GetServiceRequiredText(OSService service)
		{
			return string.Empty;
		}

		/// <summary>
		/// Searches for all Wearable devices that can be connected to.
		/// </summary>
		/// <param name="appIntentProfile"></param>
		/// <param name="onDevicesUpdated"></param>
		/// <param name="autoReconnect"></param>
		/// <param name="autoReconnectTimeout"></param>
		internal virtual void SearchForDevices(
			AppIntentProfile appIntentProfile,
			Action<Device[]> onDevicesUpdated,
			bool autoReconnect,
			float autoReconnectTimeout)
		{
			if (onDevicesUpdated != null)
			{
				DeviceSearchCallback += onDevicesUpdated;
			}

			_autoReconnect = autoReconnect;
			_autoReconnectTimeout = Mathf.Min(Time.unscaledTime + autoReconnectTimeout, float.MaxValue);
			_autoReconnectWithoutPrompts = false;
		}

		/// <summary>
		/// Stops searching for Wearable devices that can be connected to.
		/// </summary>
		internal virtual void StopSearchingForDevices()
		{
			DeviceSearchCallback = null;
		}

		internal void OnReceivedSearchDevices(Device[] devices)
		{
			// If we are currently attempting to auto-reconnect, we will compare every series of devices received
			// against the last connected device UID. If we find a match, we immediately connect to that device.
			if (_connectionStatus == ConnectionStatus.AutoReconnect)
			{
				string uid = LastConnectedDeviceUID;

				for (int i = 0; i < devices.Length; ++i)
				{
					if (devices[i].uid == uid)
					{
						ConnectToDevice(devices[i]);
						break;
					}
				}
			}
			else
			{
				if (DeviceSearchCallback != null)
				{
					DeviceSearchCallback.Invoke(devices);
				}
			}
		}

		/// <summary>
		/// Reconnects to the device last successfully connected.
		/// </summary>
		/// <param name="appIntentProfile"></param>
		internal virtual void ReconnectToLastSuccessfulDevice(AppIntentProfile appIntentProfile)
		{
			_autoReconnectWithoutPrompts = true;
		}

		/// <summary>
		/// Cancels any currently running device connection if there is one.
		/// </summary>
		internal abstract void CancelDeviceConnection();

		/// <summary>
		/// Connects to a specified device and conveys the result via the <see cref="ConnectionStatusChanged"/>
		/// event.
		/// </summary>
		/// <param name="device"></param>
		internal abstract void ConnectToDevice(Device device);

		/// <summary>
		/// Stops all attempts to connect to or monitor a device and disconnects from a device if connected.
		/// </summary>
		internal abstract void DisconnectFromDevice();

		/// <summary>
		/// If the device needs a firmware update, grab the relevant information.
		/// </summary>
		internal abstract FirmwareUpdateInformation GetFirmwareUpdateInformation();

		/// <summary>
		/// If the device is waiting for the user to choose to update out-of-date firmware, this tells the
		/// provider which option they have selected.
		/// </summary>
		internal abstract void SelectFirmwareUpdateOption(int index);

		/// <summary>
		/// Set all relevant device information for a WearableDevice, including sensors, gestures, and update interval.
		/// </summary>
		/// <param name="config"></param>
		internal abstract void SetDeviceConfiguration(WearableDeviceConfig config);

		/// <summary>
		/// Returns a reference to the provider's cached internal configuration state. May not be up-to-date with the
		/// connected device.
		/// </summary>
		/// <returns></returns>
		internal abstract WearableDeviceConfig GetCachedDeviceConfiguration();

		/// <summary>
		/// Instructs the provider to request up-to-date configuration from the attached device. As this operation may
		/// take an indeterminate amount of time, a callback is invoked when the operation completes.
		/// Multiple calls to this method while a request is waiting for completion will not request the configuration
		/// multiple times, but will invoke all callbacks set during the request.
		/// Providers are permitted to invoke the callback from this call if they have up-to-date configuration already.
		/// </summary>
		internal void RequestDeviceConfiguration(Action<WearableDeviceConfig> callback)
		{
			if (!_connectedDevice.HasValue)
			{
				Debug.LogWarning(WearableConstants.DEVICE_IS_NOT_CURRENTLY_CONNECTED);
				_waitingForDeviceConfig = false;
				return;
			}

			DeviceConfigRequestSubscribers += callback;
			if (!_waitingForDeviceConfig)
			{
				_waitingForDeviceConfig = true;
				RequestDeviceConfigurationInternal();
			}
		}

		/// <summary>
		/// Makes the actual request for fresh device configuration. This request should call
		/// <see cref="OnReceivedDeviceConfiguration"/> on completion to trigger subscribed callbacks.
		/// </summary>
		protected abstract void RequestDeviceConfigurationInternal();

		/// <summary>
		/// Checks <see cref="DeviceStatus"/> <paramref name="deviceStatus"/> to see if the sensor service is
		/// suspended and if so invokes the <see cref="SensorServiceSuspended"/> event.
		/// </summary>
		/// <param name="deviceStatus"></param>
		protected void CheckForServiceSuspended(DeviceStatus deviceStatus)
		{
			if (deviceStatus.ServiceSuspended)
			{
				var reason = deviceStatus.GetServiceSuspendedReason();

				if (SensorServiceSuspended != null)
				{
					SensorServiceSuspended.Invoke(reason);
				}
			}
		}

		/// <summary>
		/// Set the active noise reduction mode on the attached device, if the feature is available.
		/// <paramref name="mode"/> should be one of the available modes returned by
		/// <see cref="Device.GetAvailableActiveNoiseReductionModes"/>.
		/// </summary>
		public void SetActiveNoiseReductionMode(ActiveNoiseReductionMode mode)
		{
			if (!_connectedDevice.HasValue)
			{
				Debug.LogWarning(WearableConstants.DEVICE_IS_NOT_CURRENTLY_CONNECTED);
				_waitingForAnrCncWriteComplete = false;
				return;
			}

			if (mode == ActiveNoiseReductionMode.Invalid)
			{
				Debug.LogError(WearableConstants.INVALID_IS_INVALID_ANR_MODE);
				// Exit early without setting or clearing the waiting flag
				return;
			}

			if (_waitingForAnrCncWriteComplete)
			{
				Debug.LogError(WearableConstants.ANR_CNC_WRITE_LOCK_ERROR);
				return;
			}

			_waitingForAnrCncWriteComplete = true;
			SetActiveNoiseReductionModeInternal(mode);
		}

		/// <summary>
		/// Performs the actual ANR mode write. Will not be called if any ANR or CNC writes are outstanding.
		/// Providers should invoke <see cref="OnAnrCncWriteComplete"/> when the write is finished, which is permitted
		/// to happen during this call.
		/// </summary>
		/// <param name="mode"></param>
		protected abstract void SetActiveNoiseReductionModeInternal(ActiveNoiseReductionMode mode);

		/// <summary>
		/// Set the controllable noise cancellation level on the attached device, if the feature is available.
		/// <paramref name="level"/> should be between zero and
		/// (<see cref="Device.totalControllableNoiseCancellationLevels"/> - 1).
		/// </summary>
		public void SetControllableNoiseCancellationLevel(int level, bool enabled)
		{
			if (!_connectedDevice.HasValue)
			{
				Debug.LogWarning(WearableConstants.DEVICE_IS_NOT_CURRENTLY_CONNECTED);
				_waitingForAnrCncWriteComplete = false;
				return;
			}

			if (_waitingForAnrCncWriteComplete)
			{
				Debug.LogError(WearableConstants.ANR_CNC_WRITE_LOCK_ERROR);
				return;
			}

			_waitingForAnrCncWriteComplete = true;
			SetControllableNoiseCancellationLevelInternal(level, enabled);
		}

		/// <summary>
		/// Performs the actual CNC mode write. Will not be called if any ANR or CNC writes are outstanding.
		/// Providers should invoke <see cref="OnAnrCncWriteComplete"/> when the write is finished, which is permitted
		/// to happen during this call.
		/// </summary>
		protected abstract void SetControllableNoiseCancellationLevelInternal(int level, bool enabled);

		/// <summary>
		/// Returns the device status of the connected device.
		/// </summary>
		/// <returns></returns>
		internal abstract DynamicDeviceInfo GetDynamicDeviceInfo();

		/// <summary>
		/// Triggers a request for validation of a given <see cref="AppIntentProfile"/>. This request will
		/// call the <see cref="callback"/> when the validation is complete.
		/// </summary>
		/// <param name="profile"></param>
		/// <param name="callback"></param>
		internal void RequestIntentProfileValidation(AppIntentProfile profile, Action<bool> callback)
		{
			if (!_connectedDevice.HasValue)
			{
				Debug.LogWarning(WearableConstants.DEVICE_IS_NOT_CURRENTLY_CONNECTED);
				_waitingForIntentValidation = false;
				return;
			}

			if (!_waitingForIntentValidation)
			{
				IntentValidationSubscribers += callback;
				_waitingForIntentValidation = true;
				RequestIntentProfileValidationInternal(profile);

				// Clear the dirty flag after the request has been sent out, so if any
				// change occurs after this point, we know that a re-validation would
				// be necessary.
				profile.IsDirty = false;
			}
			else
			{
				// If there is an outstanding request, generate an error and ignore this call.
				Debug.LogError(WearableConstants.TOO_MANY_VALIDATION_REQUESTS_ERROR);
			}
		}

		/// <summary>
		/// Makes the actual request to validate a device configuration. This request should call
		/// <see cref="OnReceivedIntentValidationResponse"/> on completion to trigger subscribed callbacks.
		/// </summary>
		/// <param name="appIntentProfile"></param>
		protected abstract void RequestIntentProfileValidationInternal(AppIntentProfile appIntentProfile);

		/// <summary>
		/// Informs the provider that the application focus has been changed. This is called directly
		/// from WearableControl on the active provider and the event is a relay from Unity's OnApplicationFocus
		/// event.
		/// </summary>
		/// <param name="hasFocus"></param>
		internal virtual void SetAppFocusChanged(bool hasFocus)
		{
			// no-op.
		}

		/// <summary>
		/// Called by <see cref="WearableControl"/> when the provider is first initialized.
		/// Providers must call <code>base.OnInitializeProvider()</code> when overriding to update internal state.
		/// </summary>
		internal virtual void OnInitializeProvider()
		{
			_initialized = true;
			_enabled = false;
			_waitingForDeviceConfig = false;
			_waitingForIntentValidation = false;
			DeviceConfigRequestSubscribers = null;
			_connectionStatus = ConnectionStatus.Disconnected;
		}

		/// <summary>
		/// Called by <see cref="WearableControl"/> when the provider is destroyed at application quit.
		/// Providers must call <code>base.OnDestroyProvider()</code> when overriding to update internal state.
		/// </summary>
		internal virtual void OnDestroyProvider()
		{
			_initialized = false;
			_enabled = false;
			_waitingForDeviceConfig = false;
			_waitingForIntentValidation = false;
			DeviceConfigRequestSubscribers = null;
			_connectionStatus = ConnectionStatus.Disconnected;
		}

		/// <summary>
		/// Called by <see cref="WearableControl"/> when the provider is being enabled.
		/// Automatically sets the connection status to Connected if a device is still connected.
		/// Providers must call <code>base.OnEnableProvider()</code> when overriding to update internal state.
		/// </summary>
		internal virtual void OnEnableProvider()
		{
			_enabled = true;

			if (_connectedDevice != null)
			{
				OnConnectionStatusChanged(ConnectionStatus.Connected, _connectedDevice.Value);
			}
		}

		/// <summary>
		/// Called by <see cref="WearableControl"/> when the provider is being disabled.
		/// Automatically sets the connection status to disconnected if a device is still connected.
		/// Providers must call <code>base.OnDisableProvider()</code> when overriding to update internal state.
		/// </summary>
		internal virtual void OnDisableProvider()
		{
			_enabled = false;

			StopSearchingForDevices();

			if (_connectedDevice != null)
			{
				OnConnectionStatusChanged(ConnectionStatus.Disconnected, _connectedDevice.Value);
			}
		}

		/// <summary>
		/// Called by <see cref="WearableControl"/> during the appropriate Unity update method if the provider is active.
		/// </summary>
		internal virtual void OnUpdate()
		{
			if (_connectionStatus == ConnectionStatus.AutoReconnect)
			{
				if (string.IsNullOrEmpty(LastConnectedDeviceUID) || Time.unscaledTime > _autoReconnectTimeout)
				{
					OnConnectionStatusChanged(ConnectionStatus.Searching);
				}
			}

			if (_connectedDevice != null)
			{
				MonitorDynamicDeviceInfo();
			}
		}

		/// <summary>
		/// Checks the device for dynamic data changes during a session.
		/// </summary>
		private void MonitorDynamicDeviceInfo()
		{
			if (!_connectedDevice.HasValue)
			{
				return;
			}

			Device device = _connectedDevice.Value;
			Device lastDevice = device;

			DynamicDeviceInfo dynamicDeviceInfo = GetDynamicDeviceInfo();

			// Copy dynamic info to the device struct, then update the provider's device.
			// Do this at the beginning so any callbacks invoked will have up-to-date info available.
			device.SetDynamicInfo(dynamicDeviceInfo);
			_connectedDevice = device;

			// Calculate incoming outgoing events
			DeviceStatus deviceStatus = device.deviceStatus;
			DeviceStatus risingEvents = deviceStatus.GetRisingEdges(lastDevice.deviceStatus);
			DeviceStatus fallingEvents = deviceStatus.GetFallingEdges(lastDevice.deviceStatus);

			// Service suspended/resumes
			CheckForServiceSuspended(risingEvents);

			if (fallingEvents.GetFlagValue(DeviceStatusFlags.SensorServiceSuspended))
			{
				if (SensorServiceResumed != null)
				{
					SensorServiceResumed.Invoke();
				}
			}

			// ANR state changed
			if (lastDevice.activeNoiseReductionMode != device.activeNoiseReductionMode)
			{
				if (ActiveNoiseReductionModeChanged != null)
				{
					ActiveNoiseReductionModeChanged.Invoke(device.activeNoiseReductionMode);
				}
			}

			// CNC state changed
			if ((lastDevice.controllableNoiseCancellationLevel != device.controllableNoiseCancellationLevel) ||
			    (lastDevice.controllableNoiseCancellationEnabled != device.controllableNoiseCancellationEnabled))
			{
				if (ControllableNoiseCancellationInfoChanged != null)
				{
					ControllableNoiseCancellationInfoChanged.Invoke(
						device.controllableNoiseCancellationLevel,
						device.controllableNoiseCancellationEnabled);
				}
			}
		}

		/// <summary>
		/// An event holding any callbacks that have subscribed to the request for intent validation
		/// </summary>
		private event Action<bool> IntentValidationSubscribers;

		protected WearableProviderBase()
		{
			_currentSensorFrames = new List<SensorFrame>();
			_lastSensorFrame = WearableConstants.EMPTY_FRAME;

			_currentGestureData = new List<GestureData>();
		}

		/// <summary>
		/// Invokes any callbacks registered by <see cref="RequestDeviceConfiguration"/> and marks the request as
		/// complete.
		/// </summary>
		/// <param name="config"></param>
		internal void OnReceivedDeviceConfiguration(WearableDeviceConfig config)
		{
			if (!_enabled)
			{
				return;
			}

			if (_waitingForDeviceConfig && DeviceConfigRequestSubscribers != null)
			{
				DeviceConfigRequestSubscribers.Invoke(config);
			}

			_waitingForDeviceConfig = false;
			DeviceConfigRequestSubscribers = null;
		}

		/// <summary>
		/// Invokes any callbacks registered by <see cref="RequestIntentProfileValidation"/> and marks the request as
		/// complete.
		/// </summary>
		/// <param name="valid"></param>
		protected void OnReceivedIntentValidationResponse(bool valid)
		{
			if (!_enabled)
			{
				return;
			}

			if (_waitingForIntentValidation && IntentValidationSubscribers != null)
			{
				IntentValidationSubscribers.Invoke(valid);
			}

			_waitingForIntentValidation = false;
			IntentValidationSubscribers = null;
		}

		/// <summary>
		/// Used by inheriting providers to indicate that the connection status has changed.
		/// </summary>
		/// <param name="status"></param>
		/// <param name="device"></param>
		protected void OnConnectionStatusChanged(ConnectionStatus status, Device? device = null)
		{
			// If the connection attempt has failed or the device is disconnected, clear device config and
			// app intent validation state.
			if (status == ConnectionStatus.Disconnected || status == ConnectionStatus.Failed)
			{
				_waitingForDeviceConfig = false;
				DeviceConfigRequestSubscribers = null;

				_waitingForIntentValidation = false;
				IntentValidationSubscribers = null;

				_connectedDevice = null;
			}
			else if (status == ConnectionStatus.Connected && device.HasValue)
			{
				// we store the UID of the device for auto-reconnection in the future.
				LastConnectedDeviceUID = device.Value.uid;
			}

			if (_connectionStatus == status)
			{
				return;
			}

			_connectionStatus = status;

			if (ConnectionStatusChanged != null)
			{
				ConnectionStatusChanged.Invoke(_connectionStatus, device);
			}
		}

		/// <summary>
		/// Invokes the <see cref="SensorsUpdated"/> event.
		/// </summary>
		/// <param name="frame"></param>
		protected void OnSensorsUpdated(SensorFrame frame)
		{
			if (SensorsUpdated != null)
			{
				SensorsUpdated.Invoke(frame);
			}
		}

		/// <summary>
		/// Invokes the <see cref="GestureDetected"/> event.
		/// </summary>
		/// <param name="gestureId"></param>
		protected void OnGestureDetected(GestureId gestureId)
		{
			if (GestureDetected != null)
			{
				GestureDetected.Invoke(gestureId);
			}
		}

		/// <summary>
		/// Invokes the <see cref="OnConfigurationSucceeded"/> event.
		/// </summary>
		protected void OnConfigurationSucceeded()
		{
			if (ConfigurationSucceeded != null)
			{
				ConfigurationSucceeded.Invoke();
			}
		}

		/// <summary>
		/// Invokes the <see cref="OnConfigurationFailed"/> event.
		/// </summary>
		/// <param name="sensor"></param>
		/// <param name="gesture"></param>
		internal void OnConfigurationFailed(ConfigStatus sensor, ConfigStatus gesture)
		{
			if (ConfigurationFailed != null)
			{
				ConfigurationFailed.Invoke(sensor, gesture);
			}
		}

		/// <summary>
		/// Invoked by providers when an ANR or CNC write is complete, regardless of whether it failed or succeeded.
		/// </summary>
		protected void OnAnrCncWriteComplete()
		{
			_waitingForAnrCncWriteComplete = false;
		}
	}
}
