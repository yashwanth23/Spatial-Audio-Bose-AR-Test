using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="DraggableUIButtonSnapArea"/> is a UX element that indicates to the user an area the button
	/// can be snapped to.
	/// </summary>
	internal sealed class DraggableUIButtonSnapArea : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private DraggableUIButton _draggableUIButton;

		[SerializeField]
		private Image _image;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		[Header("Animation")]
		[Range(0.01f,1f)]
		[SerializeField]
		private float _canvasFadeTime;

		[SerializeField]
		private float _colorAnimationFrequency;

		[Header("Style Data Refs"), Space(5)]
		[SerializeField]
		private WearableUIColorPalette _colorPalette;

		#pragma warning restore 0649

		private bool _isAnimated;
		private Coroutine _fadeCoroutine;

		private void Awake()
		{
			_canvasGroup.interactable = false;
			_canvasGroup.blocksRaycasts = false;
			_canvasGroup.alpha = 0f;

			Subscribe();
		}

		private void OnDestroy()
		{
			Unsubscribe();
		}

		private void Update()
		{
			var factor = 1f - (Mathf.Cos(Time.time * Mathf.PI * _colorAnimationFrequency) + 1f) * 0.5f;

			var color1 = _colorPalette.InactiveChildElementStyle.elementColor;
			var color2 = _colorPalette.GetCustomizedActiveStyle().elementColor;

			_image.color = Color.Lerp(color1, color2, factor);
		}

		private void Subscribe()
		{
			_draggableUIButton.DragStarted += OnDragStarted;
			_draggableUIButton.DragEnded += OnDragEnded;
		}

		private void Unsubscribe()
		{
			_draggableUIButton.DragStarted -= OnDragStarted;
			_draggableUIButton.DragEnded -= OnDragEnded;
		}

		private void OnDragStarted()
		{
			_isAnimated = true;

			if (_fadeCoroutine != null)
			{
				StopCoroutine(_fadeCoroutine);
			}
			_fadeCoroutine = StartCoroutine(AnimateFade());
		}

		private void OnDragEnded()
		{
			_isAnimated = false;

			if (_fadeCoroutine != null)
			{
				StopCoroutine(_fadeCoroutine);
			}
			_fadeCoroutine = StartCoroutine(AnimateFade());
		}

		private IEnumerator AnimateFade()
		{
			var fadeTarget = _isAnimated ? 1f : 0f;
			var fadeSpeed = 1f / Mathf.Max(0.01f, _canvasFadeTime);
			var fadeFactor = _canvasGroup.alpha;

			var waitForEndOfFrame = new WaitForEndOfFrame();

			while (!Mathf.Approximately(fadeFactor, fadeTarget))
			{
				fadeFactor += Mathf.Sign(fadeTarget - fadeFactor) * Time.deltaTime * fadeSpeed;
				fadeFactor = Mathf.Clamp01(fadeFactor);

				_canvasGroup.alpha = fadeFactor;

				yield return waitForEndOfFrame;
			}

			_canvasGroup.alpha = fadeTarget;
		}
	}
}
