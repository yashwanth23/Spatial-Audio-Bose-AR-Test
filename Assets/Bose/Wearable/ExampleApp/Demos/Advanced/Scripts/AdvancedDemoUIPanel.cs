using System.Collections;
using UnityEngine;

namespace Bose.Wearable.Examples
{
	internal sealed class AdvancedDemoUIPanel : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UX Refs")]
		[SerializeField]
		private CanvasGroup _hintUICanvasGroup;

		[Header("Animation"), Space(5)]
		[SerializeField]
		private float _fadeDuration = 1f;

		[Header("Control"), Space(5)]
		[SerializeField]
		private float _timeToShowHintUI = 5f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _moveDistanceToHideHintUI = .25f;

		#pragma warning restore 0649

		private float _currentHideHintTime;
		private float _screenWidth;
		private float _distanceMoved;
		private bool _doHideHintUI;
		private bool _doPermanentlyHideHintUI;
		private Coroutine _hintUICoroutine;

		private void Awake()
		{
			_doHideHintUI = true;
			_screenWidth = Screen.width;
			_hintUICanvasGroup.alpha = 0f;

			MouseButtonRecognizer.Instance.DragMoved += OnDragMoved;
			MouseButtonRecognizer.Instance.DragEnded += OnDragEnded;
		}

		private void OnDestroy()
		{
			if (MouseButtonRecognizer.Instance != null)
			{
				MouseButtonRecognizer.Instance.DragMoved -= OnDragMoved;
				MouseButtonRecognizer.Instance.DragEnded -= OnDragEnded;
			}

			if (_hintUICoroutine != null)
			{
				StopCoroutine(_hintUICoroutine);
				_hintUICoroutine = null;
			}
		}

		/// <summary>
		/// When the drag ends, stop any fading of the hint UI if the touch has moved more than a specific
		/// percentage of the screen horizontally.
		/// </summary>
		/// <param name="position"></param>
		private void OnDragEnded(Vector3 position)
		{
			if (_doHideHintUI || _doPermanentlyHideHintUI)
			{
				return;
			}

			if (_hintUICoroutine != null && _distanceMoved > _moveDistanceToHideHintUI)
			{
				HideHintUI();
			}

			_distanceMoved = 0;
		}

		/// <summary>
		/// On each drag moved, capture the distance the position moved horizontally in a screen-resolution
		/// independent way.
		/// </summary>
		/// <param name="position"></param>
		private void OnDragMoved(Vector3 deltaPosition)
		{
			if (_doHideHintUI || _doPermanentlyHideHintUI)
			{
				_currentHideHintTime = 0f;
				return;
			}

			_distanceMoved += Mathf.Abs(deltaPosition.x / _screenWidth);
		}

		private void HideHintUI()
		{
			_doHideHintUI = _doPermanentlyHideHintUI = true;

			if (_hintUICoroutine != null)
			{
				StopCoroutine(_hintUICoroutine);
				_hintUICoroutine = null;
			}

			_hintUICoroutine = StartCoroutine(StopFadeHintUI());
		}

		private IEnumerator LoopFadeHintUI()
		{
			var doFadeOut = false;
			var waitForEndOfFrame = new WaitForEndOfFrame();
			var time = 0f;
			while (true)
			{
				time += Time.unscaledDeltaTime;
				if (doFadeOut)
				{
					_hintUICanvasGroup.alpha = 1 - (time / _fadeDuration);
				}
				else
				{
					_hintUICanvasGroup.alpha = time / _fadeDuration;
				}

				if (_hintUICanvasGroup.alpha <= 0 || _hintUICanvasGroup.alpha >= 1)
				{
					doFadeOut = !doFadeOut;
					time = 0f;
				}

				yield return waitForEndOfFrame;
			}
		}

		private IEnumerator StopFadeHintUI()
		{
			var waitForEndOfFrame = new WaitForEndOfFrame();
			var time = 0f;
			while (_hintUICanvasGroup.alpha > 0)
			{
				time += Time.unscaledDeltaTime;
				_hintUICanvasGroup.alpha = 1 - (time / _fadeDuration);
				yield return waitForEndOfFrame;
			}

			_hintUICoroutine = null;
		}

		private void Update()
		{
			if (_doHideHintUI && !_doPermanentlyHideHintUI)
			{
				_currentHideHintTime += Time.unscaledDeltaTime;

				if (_currentHideHintTime >= _timeToShowHintUI)
				{
					_doHideHintUI = false;
					_hintUICoroutine = StartCoroutine(LoopFadeHintUI());
				}
			}
		}
	}
}
