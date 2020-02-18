using UnityEngine;
using UnityEngine.EventSystems;

namespace Bose.Wearable
{
	internal abstract class ButtonEffectBase : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
	{
		#pragma warning disable 0649

		[Space(5)]
		[Header("Sound"), Space(5)]
		[SerializeField]
		private AudioClip _sfxClick;

		#pragma warning restore 0649

		private AudioControl _audioControl;
		protected Coroutine _effectCoroutine;

		private void Start()
		{
			_audioControl = AudioControl.Instance;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			AnimateUpInternal();
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			AnimateDownInternal();
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			PlayClickSting();
		}

		private void AnimateUpInternal()
		{
			if (_effectCoroutine != null)
			{
				StopCoroutine(_effectCoroutine);
				_effectCoroutine = null;
			}

			AnimateUp();
		}

		protected abstract void AnimateUp();

		private void AnimateDownInternal()
		{
			if (_effectCoroutine != null)
			{
				StopCoroutine(_effectCoroutine);
				_effectCoroutine = null;
			}

			AnimateDown();
		}

		protected abstract void AnimateDown();

		private void PlayClickSting()
		{
			if (_sfxClick != null)
			{
				_audioControl.PlayOneShot(_sfxClick);
			}
		}
	}
}
