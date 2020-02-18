using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	internal sealed class ButtonImageHighlightEffect : ButtonEffectBase
	{
		#pragma warning disable 0649
		[Header("Image UX Refs")]
		[SerializeField]
		private Image _image;

		[Space(5)]
		[Header("Animation"), Space(5)]
		[SerializeField]
		[Range(0f, 1f)]
		private float _duration;

		[Space(5)]
		[Header("Color"), Space(5)]
		[SerializeField]
		private Color _highlightColor;
		#pragma warning restore 0649

		private Color _originalColor;

		private void Awake()
		{
			_originalColor = _image.color;
		}

		protected override void AnimateUp()
		{
			StopEffectCoroutine();

			_effectCoroutine = StartCoroutine(AnimateEffect(_originalColor));
		}

		protected override void AnimateDown()
		{
			StopEffectCoroutine();

			_effectCoroutine = StartCoroutine(AnimateEffect(_highlightColor));
		}

		private void StopEffectCoroutine()
		{
			if (_effectCoroutine != null)
			{
				StopCoroutine(_effectCoroutine);
				_effectCoroutine = null;
			}
		}

		private IEnumerator AnimateEffect(Color targetColor)
		{
			var currentColor = _image.color;
			var timeLeft = _duration;
			while (timeLeft > 0f)
			{
				_image.color = Color.Lerp(
					currentColor,
					targetColor,
					Mathf.Clamp01(1f - (timeLeft / _duration)));

				timeLeft -= Time.unscaledDeltaTime;
				yield return Wait.ForEndOfFrame;
			}

			_image.color = targetColor;
		}

		private void Reset()
		{
			_highlightColor = Color.white;
			_highlightColor.a = 1f;
		}
	}
}
