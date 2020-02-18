using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bose.Wearable
{
	internal sealed class ButtonScaleEffect : ButtonEffectBase
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

		[FormerlySerializedAs("_scaleDown")]
		[SerializeField]
		private Vector3 _pointerDownScale;

		[FormerlySerializedAs("_scaleUp")]
		[SerializeField]
		private Vector3 _pointerUpScale;
		#pragma warning restore 0649

		private void OnEnable()
		{
			_buttonRectTransform.localScale = _pointerUpScale;
		}

		/// <summary>
		/// Reset the component to default values. Automatically called when this component is added.
		/// </summary>
		private void Reset()
		{
			_duration = 0.1f;
			_pointerDownScale = new Vector3(1.1f, 1.1f, 1.1f);
			_pointerUpScale = Vector3.one;
		}

		protected override void AnimateDown()
		{
			_effectCoroutine = StartCoroutine(AnimateEffect(_buttonRectTransform, _pointerDownScale));
		}

		protected override void AnimateUp()
		{
			_effectCoroutine = StartCoroutine(AnimateEffect(_buttonRectTransform, _pointerUpScale));
		}

		private IEnumerator AnimateEffect(RectTransform rectTransform, Vector3 targetValue)
		{
			var timeLeft = _duration;
			while (timeLeft > 0f)
			{
				rectTransform.localScale = Vector3.Lerp(
					rectTransform.localScale,
					targetValue,
					Mathf.Clamp01(1f - (timeLeft / _duration)));

				timeLeft -= Time.unscaledDeltaTime;
				yield return Wait.ForEndOfFrame;
			}

			rectTransform.localScale = targetValue;
		}
	}
}
