using System;
using System.Collections;
using Bose.Wearable.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="GestureDataDebugUIControl"/> is a debug UI control that shows the players data about
	/// gesture events.
	/// </summary>
	internal sealed class GestureDataDebugUIControl : DebugUIControlBase
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Image _titleImage;

		[SerializeField]
		private Text _titleText;

		[SerializeField]
		private Text _currentGestureText;

		[Header("UI Refs"), Space(5)]
		[Range(0f, 100f)]
		[SerializeField]
		private float _duration;

		[SerializeField]
		private AnimationCurve _animationCurve;

		#pragma warning restore 0649

		private Coroutine _gestureAnimationCoroutine;

		private Color _appearColor;
		private Color _disappearColor;

		protected override void Start()
		{
			base.Start();

			_wearableControl.GestureDetected += GestureDetected;

			_appearColor = _currentGestureText.color;
			_appearColor.a = 1f;
			_disappearColor = _appearColor;
			_disappearColor.a = 0f;

			_currentGestureText.color = _disappearColor;
		}

		private void OnDestroy()
		{
			_wearableControl.GestureDetected -= GestureDetected;
		}

		private void Update()
		{
			UpdateUI();
		}

		private void UpdateUI()
		{
			var currentDeviceConfig = _wearableControl.CurrentDeviceConfig;
			var isEnabled = currentDeviceConfig.AreAnyGesturesEnabled();

			var style = isEnabled
				? _colorPalette.GetCustomizedActiveStyle()
				: _colorPalette.InactiveTitleElementStyle;

			Color titleTextColor;
			Color titleElementColor;
			if (isEnabled)
			{
				titleTextColor = style.textColor;
				titleElementColor = style.elementColor;
			}
			else
			{
				titleTextColor = _colorPalette.InactiveTitleElementStyle.textColor;
				titleElementColor = _colorPalette.InactiveTitleElementStyle.elementColor;
			}

			_titleText.color = titleTextColor;
			_titleImage.color = titleElementColor;
		}

		private void GestureDetected(GestureId gestureId)
		{
			if (_gestureAnimationCoroutine != null)
			{
				StopCoroutine(_gestureAnimationCoroutine);
			}

			_gestureAnimationCoroutine = StartCoroutine(AnimateGestureEvent(gestureId));
		}

		private IEnumerator AnimateGestureEvent(GestureId gestureId)
		{
			_currentGestureText.text = Enum.GetName(typeof(GestureId), gestureId).Nicify().ToUpper();

			_currentGestureText.color = _appearColor;

			var currentTime = 0f;
			while (currentTime <= _duration)
			{
				currentTime += Time.unscaledDeltaTime;
				var normalizedProgress = Mathf.Clamp01(currentTime / _duration);
				var animColor = new Color(
					_currentGestureText.color.r,
					_currentGestureText.color.g,
					_currentGestureText.color.b,
					_animationCurve.Evaluate(normalizedProgress));

				_currentGestureText.color = animColor;
				yield return null;
			}

			_currentGestureText.color = _disappearColor;
		}
	}
}
