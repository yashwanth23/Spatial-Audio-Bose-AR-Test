using System;
using System.Collections;
using System.Collections.Generic;
using Bose.Wearable.Extensions;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Shown when searching for devices
	/// </summary>
	internal sealed class SearchingWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		/// <summary>
		/// The RectTransform that ConnectionDeviceDisplays will be instantiated underneath.
		/// </summary>
		[Header("UX Refs")]
		[SerializeField]
		private RectTransform _displayRootRectTransform;

		[SerializeField]
		private RectTransform _animatedDividerTransform;

		[SerializeField]
		private CanvasGroup _animatedDividerCanvasGroup;

		[SerializeField]
		private CanvasGroup _buttonDisplayCanvasGroup;

		[Header("Animation")]
		[SerializeField]
		private float _animationDuration = 5;

		[SerializeField]
		private float _animationIntervalDelay = .5f;

		[SerializeField]
		private AnimationCurve _animationWidthCurve;

		[SerializeField]
		private AnimationCurve _animationAlphaCurve;

		/// <summary>
		/// The ConnectionDeviceDisplay prefab that will be used.
		/// </summary>
		[Header("Prefabs"), Space(5)]
		[SerializeField]
		private WearableDeviceDisplayButton _displayButtonPrefab;

		[Header("Sound Clips"), Space(5)]
		[SerializeField]
		private AudioClip _sfxSearching;

		#pragma warning restore 0649

		private const string DEVICE_NAME_UID_SORT_FORMAT = "{0}_{1}";

		// Device Tracking & Button Pool
		private List<Device> _devicesCurrent;
		private List<Device> _devicesAddedOnDiscovery;
		private List<Device> _devicesRemovedOnDiscovery;

		private const int INIT_POOL_SIZE = 5;
		private List<WearableDeviceDisplayButton> _buttonPool;
		private List<WearableDeviceDisplayButton> _deviceButtons;

		// Audio
		private AudioSource _srcSearching;
		private Coroutine _searchAudioRepeatCoroutine;
		private WaitForSecondsRealtimeCacheable _repeatAudioDelay;
		private WaitForSecondsRealtimeCacheable _repeatAudioIntervalDelay;
		private const float REPEAT_INTERVAL = 5f;
		private const float TIME_BACKGROUND_FADE = 0.5f;

		private Coroutine _searchSpinner;
		private WaitForSecondsRealtimeCacheable _waitAnimationDelay;

		private RectTransform _parentRectTransform;
		private float _animationProgress;

		protected override void Awake()
		{
			_devicesCurrent = new List<Device>();
			_devicesAddedOnDiscovery = new List<Device>();
			_devicesRemovedOnDiscovery = new List<Device>();

			SetupButtonPool();
			SetupAudio();

			// Cache animation state
			_animatedDividerTransform.SetPivotCenterMiddle();
			_animatedDividerTransform.SetAnchorCenterMiddle();

			_parentRectTransform = ((RectTransform)_animatedDividerTransform.parent);
			_waitAnimationDelay = new WaitForSecondsRealtimeCacheable(_animationIntervalDelay);
			_repeatAudioIntervalDelay = new WaitForSecondsRealtimeCacheable(REPEAT_INTERVAL);
			_repeatAudioDelay = new WaitForSecondsRealtimeCacheable(_sfxSearching.length);

			_panel.DeviceSearching += OnDeviceSearching;
			_panel.DeviceConnecting += OnDeviceConnecting;
			_panel.DeviceConnectFailure += OnDeviceConnectEnd;
			_panel.DeviceConnectSuccess += OnDeviceConnectEnd;
			_panel.DevicesFound += OnDevicesFound;
			_panel.Closed += OnPanelClosed;

			base.Awake();
		}

		protected override void SetupAudio()
		{
			base.SetupAudio();

			_srcSearching = _audioControl.GetSource(true);
			_srcSearching.clip = _sfxSearching;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_panel.DeviceSearching -= OnDeviceSearching;
			_panel.DeviceConnecting -= OnDeviceConnecting;
			_panel.DeviceConnectFailure -= OnDeviceConnectEnd;
			_panel.DeviceConnectSuccess -= OnDeviceConnectEnd;
			_panel.DevicesFound -= OnDevicesFound;
			_panel.Closed -= OnPanelClosed;

			TeardownButtonPool();
			TeardownAudio();
		}

		private void OnDisable()
		{
			TeardownButtonPool();
			TeardownAudio();
		}

		private void OnDeviceSearching()
		{
			Show();
		}

		private void OnDeviceConnecting()
		{
			Hide();
		}

		private void OnDeviceConnectEnd()
		{
			Hide();
		}

		protected override void Show()
		{
			_messageText.Clear();
			_panel.EnableCloseButton();

			if (_searchSpinner == null)
			{
				_searchSpinner = StartCoroutine(AnimateIconSpinner());
			}

			_buttonDisplayCanvasGroup.alpha = 0f;

			StartRepeatingSearchAudio();

			base.Show();
		}

		private IEnumerator AnimateIconSpinner()
		{
			// Set initial animation state
			_animationProgress = 0f;
			_animatedDividerTransform.sizeDelta = new Vector2(0, _animatedDividerTransform.sizeDelta.y);
			_animatedDividerTransform.gameObject.SetActive(true);
			_animatedDividerCanvasGroup.alpha = 1f;

			while (true)
			{
				_animationProgress += Time.unscaledDeltaTime;
				if(_animationProgress > _animationDuration)
				{
					_animationProgress -= _animationDuration;
					yield return _waitAnimationDelay.Restart();
				}

				var animationWidthProgress = _animationWidthCurve.Evaluate(_animationProgress / _animationDuration);
				var animationScaleProgress = _animationAlphaCurve.Evaluate(_animationProgress / _animationDuration);

				var newWidth = _parentRectTransform.rect.width * animationWidthProgress;
				var newAlpha = (1f - animationScaleProgress) * 1f;

				_animatedDividerTransform.sizeDelta = new Vector2(newWidth, _parentRectTransform.rect.height);
				_animatedDividerCanvasGroup.alpha = newAlpha;

				yield return Wait.ForEndOfFrame;
			}
		}

		protected override void Hide()
		{
			base.Hide();

			_devicesCurrent.Clear();

			if (_searchSpinner != null)
			{
				StopCoroutine(_searchSpinner);
				_searchSpinner = null;

				// Cleanup animation state and visuals
				_animatedDividerCanvasGroup.alpha = 0f;
			}

			StopRepeatingSearchAudio();

			ReclaimAllButtons();
		}

		private void OnDevicesFound(Device[] devices)
		{
			DetermineRemovedDevices(devices);
			DetermineAddedDevices(devices);
			ResolveDeviceChanges();

			if (_devicesAddedOnDiscovery.Count > 0)
			{
				_buttonDisplayCanvasGroup.alpha = 1f;
			}
			else if (_devicesCurrent.Count > 0)
			{
				_buttonDisplayCanvasGroup.alpha = 1f;
			}
			else if (_devicesCurrent.Count == 0)
			{
				_buttonDisplayCanvasGroup.alpha = 0f;
			}
		}

		private void OnPanelClosed(WearableConnectUIResult value)
		{
			Hide();
		}

		/// <summary>
		/// Create a list of all devices that are currently being tracked that are no longer reported in the
		/// most recent list of discovered devices.
		/// </summary>
		private void DetermineRemovedDevices(Device[] devices)
		{
			_devicesRemovedOnDiscovery.Clear();

			for (int i = 0; i < _devicesCurrent.Count; ++i)
			{
				var device = _devicesCurrent[i];

				if (Array.IndexOf(devices, device) >= 0)
				{
					continue;
				}

				_devicesRemovedOnDiscovery.Add(device);
			}
		}

		/// <summary>
		/// Create a list of new devices that do not exist in our list of current devices.
		/// </summary>
		private void DetermineAddedDevices(Device[] devices)
		{
			_devicesAddedOnDiscovery.Clear();

			for (int i = 0; i < devices.Length; ++i)
			{
				// If we already have the unique device locally cached by way of uid, updates its value
				// as it may have changed.
				var device = devices[i];
				var deviceIndex = _devicesCurrent.IndexOf(device);
				if (deviceIndex != -1)
				{
					_devicesCurrent[deviceIndex] = device;
					continue;
				}

				_devicesAddedOnDiscovery.Add(device);
			}
		}

		/// <summary>
		/// Ensure that _devicesCurrent is up to date with only the known devices, and that the active buttons
		/// reflect that collection and its ordering.
		/// </summary>
		private void ResolveDeviceChanges()
		{
			// resolve the deltas to the current devices, and re-sort based on the name of the devices.
			_devicesCurrent.RemoveAll(_devicesRemovedOnDiscovery.Contains);
			_devicesCurrent.AddRange(_devicesAddedOnDiscovery);
			_devicesCurrent.Sort(CompareDevicesBasedOnName);

			// reclaim all devices we want to remove.
			for (int i = 0; i < _devicesRemovedOnDiscovery.Count; ++i)
			{
				ReclaimButtonWithDevice(_devicesRemovedOnDiscovery[i]);
			}

			// go through the current devices and either move the ones that already exist in the current list
			// or pull one from the pool to insert into the current space.
			for (int i = 0; i < _devicesCurrent.Count; ++i)
			{
				var device = _devicesCurrent[i];
				var deviceButton = _deviceButtons.Find(button => button.Device.uid == device.uid);
				if (deviceButton == null)
				{
					deviceButton = CreateButton(device);
				}

				deviceButton.Set(device);
				deviceButton.transform.SetSiblingIndex(i);
			}
		}

		/// <summary>
		/// Compare <see cref="Device"/> <paramref name="x"/> with <see cref="Device"/> <paramref name="y"/>
		/// based on combination of <seealso cref="Device.name"/> and <seealso cref="Device.uid"/>.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private static int CompareDevicesBasedOnName(Device x, Device y)
		{
			return string.Compare(
				string.Format(DEVICE_NAME_UID_SORT_FORMAT, x.name, x.uid),
				string.Format(DEVICE_NAME_UID_SORT_FORMAT, y.name, y.uid),
				StringComparison.Ordinal);
		}

		/// <summary>
		/// Sets up the button pool with a constant amount of buttons.
		/// </summary>
		private void SetupButtonPool()
		{
			_buttonPool = new List<WearableDeviceDisplayButton>();
			_deviceButtons = new List<WearableDeviceDisplayButton>();

			for (int i = 0; i < INIT_POOL_SIZE; ++i)
			{
				CreateButton();
			}
		}

		/// <summary>
		/// Either instantiates or pulls a button from the pool based on availability. If a
		/// <paramref name="device"/> is provided, the button will made visible immediately.
		/// </summary>
		/// <returns>The button.</returns>
		/// <param name="device">Device.</param>
		private WearableDeviceDisplayButton CreateButton(Device? device = null)
		{
			WearableDeviceDisplayButton button = null;

			if (_buttonPool.Count > 0)
			{
				var idx = _buttonPool.Count - 1;

				button = _buttonPool[idx];
				_buttonPool.RemoveAt(idx);
			}
			else
			{
				button = Instantiate(_displayButtonPrefab, _displayRootRectTransform, false);
			}

			if (device.HasValue)
			{
				button.gameObject.SetActive(true);
				_deviceButtons.Add(button);
			}
			else
			{
				button.gameObject.SetActive(false);
				_buttonPool.Add(button);
			}

			return button;
		}

		/// <summary>
		/// Reclaims the button with device.
		/// </summary>
		/// <param name="device">Device.</param>
		private void ReclaimButtonWithDevice(Device device)
		{
			var idxButton = _deviceButtons.FindIndex(button => button.Device.uid == device.uid);

			if (idxButton >= 0)
			{
				var deviceButton = _deviceButtons[idxButton];
				_deviceButtons.RemoveAt(idxButton);
				_buttonPool.Add(deviceButton);

				deviceButton.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Forcibly reclaims all buttons.
		/// </summary>
		private void ReclaimAllButtons()
		{
			for (int i = 0; i < _deviceButtons.Count; ++i)
			{
				var button = _deviceButtons[i];

				_buttonPool.Add(button);
				button.gameObject.SetActive(false);
			}

			_devicesCurrent.Clear();
			_devicesAddedOnDiscovery.Clear();
			_devicesRemovedOnDiscovery.Clear();
			_deviceButtons.Clear();
		}

		/// <summary>
		/// Removes all display children from the root rect transform.
		/// </summary>
		private void TeardownButtonPool()
		{
			var childCount = _displayRootRectTransform.childCount;

			if (childCount == 0)
			{
				return;
			}

			for (var i = childCount - 1; i >= 0; i--)
			{
				var child = _displayRootRectTransform.GetChild(i);
				Destroy(child.gameObject);
			}

			if (_deviceButtons != null)
			{
				_deviceButtons.Clear();
			}

			if (_buttonPool != null)
			{
				_buttonPool.Clear();
			}
		}

		private void StartRepeatingSearchAudio()
		{
			if (_searchAudioRepeatCoroutine != null)
			{
				StopCoroutine(_searchAudioRepeatCoroutine);
				_searchAudioRepeatCoroutine = null;
			}

			_searchAudioRepeatCoroutine = StartCoroutine(RepeatSearchAudio());
		}

		private void StopRepeatingSearchAudio()
		{
			if (_searchAudioRepeatCoroutine != null)
			{
				StopCoroutine(_searchAudioRepeatCoroutine);
				_searchAudioRepeatCoroutine = null;
			}

			if (_srcSearching.isPlaying)
			{
				_audioControl.FadeOut(_srcSearching, TIME_BACKGROUND_FADE);
			}
		}

		private IEnumerator RepeatSearchAudio()
		{
			while(true)
			{
				_audioControl.FadeIn(_srcSearching, TIME_BACKGROUND_FADE);

				yield return _repeatAudioDelay.Restart();

				_audioControl.FadeIn(_srcSearching, TIME_BACKGROUND_FADE);

				yield return _repeatAudioDelay.Restart();
				yield return _repeatAudioIntervalDelay.Restart();
			}
		}

		protected override void TeardownAudio()
		{
			if (_srcSearching != null)
			{
				Destroy(_srcSearching.gameObject);
			}
		}
	}
}
