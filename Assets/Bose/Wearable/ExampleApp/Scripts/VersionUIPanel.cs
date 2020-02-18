using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	internal sealed class VersionUIPanel : MonoBehaviour, IPointerClickHandler
	{
		#pragma warning disable 0649

		[SerializeField]
		private Text _versionText;

		#pragma warning restore 0649

		private bool _showSdkVersion;
		private string _sdkVersion;
		private string _unityVersion;

		private const string SDK_VERSION_FORMAT = "SDK v{0}";
		private const string UNITY_VERSION_FORMAT = "Unity {0}";

		private void Awake()
		{
			_showSdkVersion = true;

			_sdkVersion = string.Format(SDK_VERSION_FORMAT, WearableVersion.UnitySdkVersion);
			_unityVersion = string.Format(UNITY_VERSION_FORMAT, Application.unityVersion);

			UpdateVersionLabel();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			_showSdkVersion = !_showSdkVersion;
			UpdateVersionLabel();
		}

		private void UpdateVersionLabel()
		{
			_versionText.text = _showSdkVersion ? _sdkVersion : _unityVersion;
		}
	}
}

