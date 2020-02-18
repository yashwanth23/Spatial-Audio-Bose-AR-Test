using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bose.Wearable
{
	internal sealed class ButtonSizeEffect : ButtonEffectBase
	{
		#pragma warning disable 0649
		[Space(5)]
		[Header("General UX Refs")]
		[SerializeField]
		private RectTransform _buttonRectTransform;

		[Space(5)]
		[Header("Animation"), Space(5)]
		[SerializeField]
		[Range(0f, 1f)]
		private float _duration;

		[FormerlySerializedAs("_sizeDown")]
		[SerializeField]
		private Vector2 _pointerDownSize;

		[FormerlySerializedAs("_sizeUp")]
		[SerializeField]
		private Vector2 _pointerUpSize;
		#pragma warning restore 0649

		private void OnEnable()
		{
			_buttonRectTransform.sizeDelta = _pointerUpSize;
		}

		/// <summary>
		/// Reset the component to default values. Automatically called when this component is added.
		/// </summary>
		private void Reset()
		{
			_duration = 0.1f;
			_pointerDownSize = new Vector2(30f, 20f);
			_pointerUpSize = Vector2.zero;
		}

		protected override void AnimateDown()
		{
			_effectCoroutine = StartCoroutine(AnimateEffect(_buttonRectTransform, _pointerDownSize));
		}

		protected override void AnimateUp()
		{
			_effectCoroutine = StartCoroutine(AnimateEffect(_buttonRectTransform, _pointerUpSize));
		}

		private IEnumerator AnimateEffect(RectTransform rectTransform, Vector3 targetValue)
		{
			var timeLeft = _duration;
			while (timeLeft > 0f)
			{
				rectTransform.sizeDelta = Vector2.Lerp(
					rectTransform.sizeDelta,
					targetValue,
					Mathf.Clamp01(1f - (timeLeft / _duration)));

				timeLeft -= Time.unscaledDeltaTime;
				yield return Wait.ForEndOfFrame;
			}

			rectTransform.sizeDelta = targetValue;
		}
	}
}
