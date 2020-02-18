using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Bose.Wearable.Examples
{
	internal sealed class MainMenuUIPanel : MonoBehaviour
	{
		#pragma warning disable 0649

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[SerializeField]
		private GameObject _buttonParentGameObject;

		[SerializeField]
		private GameObject _connectParentGameObject;

		[SerializeField]
		private Button _showConnectUIButton;

		[SerializeField]
		private Button _basicDemoButton;

		[SerializeField]
		private Button _gestureDemoButton;

		[SerializeField]
		private Button _advancedDemoButton;

		[SerializeField]
		private Button _debugDemoButton;

		#pragma warning restore 0649

		private WearableControl _wearableControl;

		private void Awake()
		{
			_showConnectUIButton.onClick.AddListener(OnShowConnectUIButtonClicked);
			_basicDemoButton.onClick.AddListener(OnBasicDemoButtonClicked);
			_advancedDemoButton.onClick.AddListener(OnAdvancedDemoButtonClicked);
			_gestureDemoButton.onClick.AddListener(OnGestureDemoButtonClicked);
			_debugDemoButton.onClick.AddListener(OnDebugDemoButtonClicked);

			ToggleInteractivity(true);
		}

		private void Start()
		{
			_wearableControl = WearableControl.Instance;
			_wearableControl.DeviceConnected += OnDeviceConnected;
			_wearableControl.DeviceDisconnected += OnDeviceDisconnected;

			var deviceIsConnected = _wearableControl.ConnectedDevice.HasValue;
			_buttonParentGameObject.gameObject.SetActive(deviceIsConnected);
			_connectParentGameObject.gameObject.SetActive(!deviceIsConnected);
		}

		private void OnDestroy()
		{
			_wearableControl.DeviceConnected -= OnDeviceConnected;
			_wearableControl.DeviceDisconnected -= OnDeviceDisconnected;

			_showConnectUIButton.onClick.RemoveAllListeners();
			_basicDemoButton.onClick.RemoveAllListeners();
			_advancedDemoButton.onClick.RemoveAllListeners();
			_gestureDemoButton.onClick.RemoveAllListeners();
			_debugDemoButton.onClick.RemoveAllListeners();
		}

		private void OnShowConnectUIButtonClicked()
		{
			WearableConnectUIPanel.Instance.Show();
		}

		private void OnAdvancedDemoButtonClicked()
		{
			LoadingUIPanel.Instance.LoadScene(WearableDemoConstants.ADVANCED_DEMO_SCENE, LoadSceneMode.Single);

			ToggleInteractivity(false);
		}

		private void OnBasicDemoButtonClicked()
		{
			LoadingUIPanel.Instance.LoadScene(WearableDemoConstants.BASIC_DEMO_SCENE, LoadSceneMode.Single);

			ToggleInteractivity(false);
		}

		private void OnGestureDemoButtonClicked()
		{
			LoadingUIPanel.Instance.LoadScene(WearableDemoConstants.GESTURE_DEMO_SCENE, LoadSceneMode.Single);

			ToggleInteractivity(false);
		}

		private void OnDebugDemoButtonClicked()
		{
			LoadingUIPanel.Instance.LoadScene(WearableDemoConstants.DEBUG_DEMO_SCENE, LoadSceneMode.Single);

			ToggleInteractivity(false);
		}

		private void OnDeviceConnected(Device device)
		{
			_buttonParentGameObject.gameObject.SetActive(true);
			_connectParentGameObject.gameObject.SetActive(false);
		}

		private void OnDeviceDisconnected(Device device)
		{
			_buttonParentGameObject.gameObject.SetActive(false);
			_connectParentGameObject.gameObject.SetActive(true);
		}

		private void ToggleInteractivity(bool isOn)
		{
			_canvasGroup.interactable = isOn;
		}
	}
}
