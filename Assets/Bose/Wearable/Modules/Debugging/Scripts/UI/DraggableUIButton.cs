using System;
using System.Collections;
using Bose.Wearable.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="DraggableUIButton"/> is a draggable button class that can be clicked or dragged around the
	/// screen; when released it will either snap immediately to or animate to the nearest corner of its parent
	/// <see cref="RectTransform"/>.
	/// </summary>
	internal sealed class DraggableUIButton : MonoBehaviour,
		IPointerEnterHandler,
		IPointerExitHandler,
		IPointerDownHandler,
		IPointerUpHandler,
		IDragHandler
	{
		/// <summary>
		/// Invoked when the button is clicked.
		/// </summary>
		public event Action Clicked;

		/// <summary>
		/// Invoked when dragging has started.
		/// </summary>
		public event Action DragStarted;

		/// <summary>
		/// Invoked when dragging has ended.
		/// </summary>
		public event Action DragEnded;

		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private RectTransform _rectTransform;

		[SerializeField]
		private Image _buttonImage;

		[SerializeField]
		private Image _buttonIconImage;

		[Header("UX")]
		[Range(0f, 2f)]
		[SerializeField]
		private float _dragWaitTime;

		[Range(0f, 1f)]
		[SerializeField]
		private float _swapThreshold;

		[Header("Animation"), Space(5)]
		[SerializeField]
		private Vector2 _sizeSquash;

		[SerializeField]
		private Vector2 _sizeStretch;

		[SerializeField]
		[Range(0.25f, 10f)]
		private float _animSizeSpeed;

		[SerializeField]
		[Range(0f, 1f)]
		private float _animSizeReturnTime;

		[SerializeField]
		[Range(0.25f, 10f)]
		private float _animPositionSpeed;

		[SerializeField]
		private float _animSwapTime;

		[SerializeField]
		private AnimationCurve _animSwapCurve;

		[Header("Style Data Refs")]
		[SerializeField]
		private WearableUIColorPalette _colorPalette;

		#pragma warning restore 0649

		private bool IsSwappingSides
		{
			get { return _animSwapCoroutine != null;  }
		}

		private bool _isPointerEntered;
		private bool _isPointerDown;
		private bool _isDragging;
		private RectTransform _parentRectTransform;
		private Vector2 _sizeDefault;
		private float _currentSwapValue;
		private Vector2 _lastPointerPosition;

		private Coroutine _queueDragCoroutine;
		private Coroutine _animSwapCoroutine;
		private Coroutine _animPositionCoroutine;
		private Coroutine _animSizeCoroutine;

		private WaitForEndOfFrame _waitForEndOfFrame;

		private void Awake()
		{
			_waitForEndOfFrame = new WaitForEndOfFrame();

			_parentRectTransform = _rectTransform.transform.parent.GetComponent<RectTransform>();
			_sizeDefault = _rectTransform.sizeDelta;

			// force an update on the swapValue to ensure our button has been snapped to a side.
			UpdateSwapValue();
		}

		private void OnEnable()
		{
			Reset();
		}

		private void OnValidate()
		{
			UpdateStyle();
		}

		#region Unity Pointer & Drag Events

		public void OnPointerDown(PointerEventData eventData)
		{
			_isPointerDown = true;

			UpdateSwapValue(eventData);

			if (_queueDragCoroutine == null)
			{
				_queueDragCoroutine = StartCoroutine(QueueStartDrag());
			}

			_lastPointerPosition = eventData.position;
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			_isPointerDown = false;

			if (_isDragging)
			{
				EndDrag();
			}
			else if (_isPointerEntered)
			{
				if (Clicked != null)
				{
					Clicked.Invoke();
				}
			}

			Reset();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_isPointerEntered = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			_isPointerEntered = false;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!_isDragging)
			{
				return;
			}

			UpdateSwapValue(eventData);

			_lastPointerPosition = eventData.position;
		}

		#endregion

		private void Reset()
		{
			_isDragging = false;

			if (_queueDragCoroutine != null)
			{
				StopCoroutine(_queueDragCoroutine);
				_queueDragCoroutine = null;
			}
		}

		private void UpdateStyle()
		{
			if (_colorPalette == null)
			{
				return;
			}

			var style = _colorPalette.GetCustomizedActiveStyle();
			_buttonImage.color = style.elementColor;
			_buttonIconImage.color = style.textColor;
		}

		/// <summary>
		/// Whenever the user has clicked on the button, we wait and see if they hold on the button
		/// for a given period of time.
		/// </summary>
		/// <returns></returns>
		private IEnumerator QueueStartDrag()
		{
			var holdTime = 0f;

			while (true)
			{
				// potential drags are canceled the moment the user moves outside
				// of the bounding box.
				if (!_isPointerEntered || !_isPointerDown)
				{
					break;
				}

				holdTime += Time.unscaledDeltaTime;

				if (holdTime > _dragWaitTime)
				{
					StartDrag();
					break;
				}

				yield return _waitForEndOfFrame;
			}
		}

		private void StartDrag()
		{
			_isDragging = true;

			if (DragStarted != null)
			{
				DragStarted.Invoke();
			}

			if (_animSizeCoroutine != null)
			{
				StopCoroutine(_animSizeCoroutine);
			}
			_animSizeCoroutine = StartCoroutine(AnimButtonSize());

			if (_animPositionCoroutine != null)
			{
				StopCoroutine(_animPositionCoroutine);
			}
			_animPositionCoroutine = StartCoroutine(AnimButtonPosition());
		}

		/// <summary>
		/// Updates a value that represents the normalized horizontal distance between the button's
		/// world worldPosition and the pointer.
		/// </summary>
		private void UpdateSwapValue(PointerEventData pointerData = null)
		{
			Vector2 buttonPos = _rectTransform.position;
			Vector2 pointerPos = pointerData == null ? buttonPos : pointerData.position;

			var inputPosNormalized = GetNormalizedPositionInParent(pointerPos);
			var buttonPosNormalized = GetNormalizedPositionInParent(buttonPos);
			var deltaPos = buttonPosNormalized - inputPosNormalized;

			_currentSwapValue = Mathf.Abs(deltaPos.x);

			// determine if we should be switching sides
			if (!IsSwappingSides && _currentSwapValue >= _swapThreshold)
			{
				// change anchor based on worldPosition
				if (buttonPosNormalized.x > 0.5f)
				{
					_rectTransform.SetAnchorLeftMiddle();
					_rectTransform.SetPivotLeftMiddle();
				}
				else
				{
					_rectTransform.SetAnchorRightMiddle();
					_rectTransform.SetPivotRightMiddle();
				}

				_animSwapCoroutine = StartCoroutine(AnimateSwapSides());
			}
		}

		private void EndDrag()
		{
			if (DragEnded != null)
			{
				DragEnded.Invoke();
			}
		}

		/// <summary>
		/// Returns a <see cref="Rect"/> that represents the initial size of the parent container
		/// less the current height of the button modulated by its scale.
		/// </summary>
		private Rect GetClampedParentRect()
		{
			Rect result;

			var height = _rectTransform.sizeDelta.y;
			height *= _rectTransform.localScale.y;

			result = _parentRectTransform.rect;
			result.height -= height;
			result.y += height * 0.5f;

			return result;
		}

		/// <summary>
		/// Given a world position, returns a normalized position within the clamped parent rect.
		/// </summary>
		private Vector2 GetNormalizedPositionInParent(Vector2 worldPosition)
		{
			var clampedParentRect = GetClampedParentRect();
			var localPosition = _parentRectTransform.InverseTransformPoint(worldPosition);

			return Rect.PointToNormalized(clampedParentRect, localPosition);
		}

		/// <summary>
		/// Given a normalized position within the clamped parent rect, returns a world position.
		/// </summary>
		private Vector2 GetWorldPositionFromNormalizedPosition(Vector2 normalizedPosition)
		{
			var container = _parentRectTransform.rect;
			container.height -= _rectTransform.sizeDelta.y;
			container.y += _rectTransform.sizeDelta.y * 0.5f;

			var localPosition = Rect.NormalizedToPoint(container, normalizedPosition);

			return _parentRectTransform.TransformPoint(localPosition);
		}

		/// <summary>
		/// A short, scripted animation that will return the button's anchoredPosition
		/// back to zero over a short amount of time.
		/// </summary>
		private IEnumerator AnimateSwapSides()
		{
			var time = 0f;
			var anchorPos = _rectTransform.anchoredPosition;
			var originalPosX = anchorPos.x;

			while (time < _animSwapTime)
			{
				var factor = Mathf.Clamp01(time/_animSwapTime);

				anchorPos.x = Mathf.Lerp(originalPosX, 0f, _animSwapCurve.Evaluate(factor));
				_rectTransform.anchoredPosition = anchorPos;

				time += Time.unscaledDeltaTime;
				yield return _waitForEndOfFrame;
			}

			anchorPos.x = 0f;
			_rectTransform.anchoredPosition = anchorPos;

			_animSwapCoroutine = null;
		}

		/// <summary>
		/// Once dragging begins, we constantly animate the position of the button based on
		/// the vertical height of the pointer, stored in <see cref="_lastPointerPosition"/>.
		/// </summary>
		private IEnumerator AnimButtonPosition()
		{
			Vector2 currentPosition = GetNormalizedPositionInParent(_rectTransform.position);
			Vector2 targetPosition;

			while (_isDragging)
			{
				if (!IsSwappingSides)
				{
					var pointerPos = _lastPointerPosition;
					var pointerPosWorld = new Vector2(_rectTransform.position.x, pointerPos.y);
					targetPosition = GetNormalizedPositionInParent(pointerPosWorld);

					// we move the position of the button towards the last known height of the pointer.
					// the x component is always overwritten, as swapping sides could have both the
					// target and current position conflict as to where the button should be.
					currentPosition += Vector2.ClampMagnitude((targetPosition - currentPosition) * 20f, _animPositionSpeed) * Time.unscaledDeltaTime;
					currentPosition.x = targetPosition.x;

					_rectTransform.position = GetWorldPositionFromNormalizedPosition(currentPosition);
				}

				yield return _waitForEndOfFrame;
			}
		}

		/// <summary>
		/// Once dragging begins, we constantly animate the size of the button based on
		/// the moving <see cref="_currentSwapValue"/>. When dragging ends, we animate a return
		/// to the default size.
		/// </summary>
		private IEnumerator AnimButtonSize()
		{
			var swapValue = 0f;
			var factor = 0f;
			Vector2 currentSizeDelta = _rectTransform.sizeDelta;
			Vector2 targetSizeDelta = _sizeDefault;

			// constantly update the sizeDelta of the button as we're dragging.
			while (_isDragging)
			{
				if (Mathf.Approximately(swapValue, _currentSwapValue))
				{
					swapValue = _currentSwapValue;
				}
				else
				{
					swapValue += Mathf.Clamp((_currentSwapValue - swapValue) * 20f, -_animSizeSpeed, _animSizeSpeed) * Time.unscaledDeltaTime;
				}

				factor = Mathf.Clamp(swapValue / _swapThreshold, 0.2f, 1f);

				targetSizeDelta = Vector2.Lerp(_sizeSquash, _sizeStretch, factor);
				currentSizeDelta = Vector2.Lerp(currentSizeDelta, targetSizeDelta, factor);

				_rectTransform.sizeDelta = currentSizeDelta;

				yield return _waitForEndOfFrame;
			}

			// now that we've finished dragging, return the button back to its default size.
			targetSizeDelta = _sizeDefault;
			var time = 0f;
			while (time < _animSizeReturnTime)
			{
				factor = time / _animSizeReturnTime;

				currentSizeDelta = Vector2.Lerp(currentSizeDelta, targetSizeDelta, factor);
				_rectTransform.sizeDelta = currentSizeDelta;

				time += Time.unscaledDeltaTime;
				yield return _waitForEndOfFrame;
			}

			_rectTransform.sizeDelta = targetSizeDelta;

			_animSizeCoroutine = null;
		}
	}
}
