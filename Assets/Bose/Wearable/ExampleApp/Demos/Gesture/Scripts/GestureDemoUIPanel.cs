using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bose.Wearable.Examples
{
	/// <summary>
	/// <see cref="GestureDemoUIPanel"/> is used to represent all gesture types as displays that animate when
	/// the user makes the correlating gesture.
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	internal sealed class GestureDemoUIPanel : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UX Refs")]
		[SerializeField]
		private Transform _deviceAgnosticRootTransform;
		[SerializeField]
		private Transform _deviceSpecificRootTransform;

		[Header("Prefab/Factory Refs"), Space(5)]
		[SerializeField]
		private GestureIconFactory _gestureIconFactory;

		[SerializeField]
		private GestureIconFactory _gestureGlowIconFactory;

		[SerializeField]
		private GestureDisplay _gestureDisplay;

		#pragma warning restore 0649

		private WearableControl _wearableControl;

		private const string GESTURE_ICON_NOT_FOUND_FORMAT = "[Bose Wearable] Skipped creating a GestureDisplay " +
		                                                 "for gesture [{0}].";

		private void Start ()
		{
			var deviceAgnosticChildCount = _deviceAgnosticRootTransform.childCount;
			for (var i = deviceAgnosticChildCount - 1; i >= 0; i--)
			{
				var childGameObject = _deviceAgnosticRootTransform.GetChild(i);
				Destroy(childGameObject.gameObject);
			}

			var deviceSpecificChildCount = _deviceSpecificRootTransform.childCount;
			for (var i = deviceSpecificChildCount - 1; i >= 0; i--)
			{
				var childGameObject = _deviceSpecificRootTransform.GetChild(i);
				Destroy(childGameObject.gameObject);
			}

			_wearableControl = WearableControl.Instance;

			for (var i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				GestureId gestureId = WearableConstants.GESTURE_IDS[i];

				if (gestureId == GestureId.None)
				{
					continue;
				}

				if (_wearableControl.GetWearableGestureById(gestureId).IsAvailable == false)
				{
					continue;
				}

				Sprite sprite;
				if (!_gestureIconFactory.TryGetGestureIcon(gestureId, out sprite))
				{
					Debug.LogWarningFormat(this, GESTURE_ICON_NOT_FOUND_FORMAT, gestureId);
					continue;
				}

				Sprite glowSprite;
				if (!_gestureGlowIconFactory.TryGetGestureIcon(gestureId, out glowSprite))
				{
					Debug.LogWarningFormat(this, GESTURE_ICON_NOT_FOUND_FORMAT, gestureId);
					continue;
				}

				Transform displayRoot = (gestureId.IsGestureDeviceAgnostic())?
					_deviceAgnosticRootTransform : _deviceSpecificRootTransform;
				var gestureDisplay = Instantiate(_gestureDisplay, displayRoot, false);
				gestureDisplay.Set(gestureId, sprite, glowSprite);
				gestureDisplay.gameObject.SetActive(true);
			}
		}
	}
}
