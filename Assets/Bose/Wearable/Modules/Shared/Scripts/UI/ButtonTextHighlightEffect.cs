using System.Collections;
using TMPro;
using UnityEngine;

namespace Bose.Wearable
{
	internal sealed class ButtonTextHighlightEffect : ButtonEffectBase
	{
		#pragma warning disable 0649
		[Header("Text UX Refs")]
		[SerializeField]
		private TMP_Text _labelText;

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
			_originalColor = _labelText.color;
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
			var currentColor = _labelText.color;
			var timeLeft = _duration;
			while (timeLeft > 0f)
			{
				_labelText.color = Color.Lerp(
					currentColor,
					targetColor,
					Mathf.Clamp01(1f - (timeLeft / _duration)));

				timeLeft -= Time.unscaledDeltaTime;
				yield return Wait.ForEndOfFrame;
			}

			_labelText.color = targetColor;
		}

		private void Reset()
		{
			_highlightColor = Color.white;
			_highlightColor.a = 1f;
		}
	}
}
