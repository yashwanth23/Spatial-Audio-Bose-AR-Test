using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bose.Wearable.Examples
{
	internal sealed class ReturnToMainMenuOnDisconnect : MonoBehaviour
	{
		private WearableControl _wearableControl;

		private void Start()
		{
			_wearableControl = WearableControl.Instance;
			_wearableControl.DeviceDisconnected += OnDeviceDisconnected;

			if (!_wearableControl.ConnectedDevice.HasValue)
			{
				ReturnToMainMenu();
			}
		}

		private void OnDestroy()
		{
			_wearableControl.DeviceDisconnected -= OnDeviceDisconnected;
		}

		private void OnDeviceDisconnected(Device device)
		{
			ReturnToMainMenu();
		}

		private void ReturnToMainMenu()
		{
			if (LoadingUIPanel.Exists)
			{
				LoadingUIPanel.Instance.LoadScene(WearableDemoConstants.MAIN_MENU_SCENE, LoadSceneMode.Single);
			}
		}
	}
}
