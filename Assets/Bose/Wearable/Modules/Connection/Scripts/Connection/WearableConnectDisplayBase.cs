using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// A base class for handling behavior for hiding and showing displays related to device connection
	/// </summary>
	internal abstract class WearableConnectDisplayBase : MonoBehaviour
	{
		[SerializeField]
		protected WearableConnectUIPanel _panel;

		[SerializeField]
		protected CanvasGroup _canvasGroup;

		[SerializeField]
		protected LocalizedText _messageText;

		protected AudioControl _audioControl;
		protected WearableControl _wearableControl;

		protected virtual void Awake()
		{
			_panel.Closed += OnConnectUIPanelClosed;
			_wearableControl = WearableControl.Instance;

			Hide();
		}

		protected virtual void OnDestroy()
		{
			_panel.Closed -= OnConnectUIPanelClosed;
		}

		protected virtual void SetupAudio()
		{
			_audioControl = AudioControl.Instance;
		}

		private void OnConnectUIPanelClosed(WearableConnectUIResult status)
		{
			Hide();
		}

		protected virtual void TeardownAudio()
		{
		}

		protected virtual void Show()
		{
			_canvasGroup.alpha = 1f;
			_canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
		}

		protected virtual void Hide()
		{
			_canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
			_canvasGroup.alpha = 0f;
		}
	}
}
