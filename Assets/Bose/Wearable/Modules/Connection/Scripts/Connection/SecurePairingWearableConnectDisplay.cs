using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	internal sealed class SecurePairingWearableConnectDisplay : WearableConnectDisplayBase
	{
		#pragma warning disable 0649

		[Header("UI Refs"), Space(5)]
		[SerializeField]
		private Image _productTourImage;

		[Header("Animation"), Space(5)]
		[SerializeField]
		private string _playStateName;

		[SerializeField]
		private Animator[] _animators;

		[SerializeField]
		private RectTransform[] _annotationImageRectTransforms;

		[Header("Data"), Space(5)]
		[SerializeField]
		private WearableProductImageFactory _productImageFactory;

		#pragma warning restore 0649

		private int _playStateHash;

		private const string CANNOT_FIND_PRODUCT_IMAGES_WARNING_FORMAT =
			"[BoseWearable] Cannot find product images for Product Type [{0}] and Variant Type [{1}].";

		protected override void Awake()
		{
			base.Awake();

			_panel.DeviceConnecting += OnDeviceConnecting;
			_panel.FirmwareCheckStarted += OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired += OnDeviceSecurePairingRequired;
			_panel.DeviceConnectSuccess += OnSecurePairingSuccess;
			_panel.DeviceConnectFailure += OnSecurePairingFailure;

			_playStateHash = Animator.StringToHash(_playStateName);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			_panel.DeviceConnecting -= OnDeviceConnecting;
			_panel.FirmwareCheckStarted -= OnFirmwareCheckStarted;
			_panel.DeviceSecurePairingRequired -= OnDeviceSecurePairingRequired;
			_panel.DeviceConnectSuccess -= OnSecurePairingSuccess;
			_panel.DeviceConnectFailure -= OnSecurePairingFailure;
		}

		private void OnDeviceConnecting()
		{
			Hide();
		}

		private void OnFirmwareCheckStarted(
			bool isRequired,
			Device device,
			FirmwareUpdateInformation updateInformation)
		{
			Hide();
		}

		private void OnDeviceSecurePairingRequired()
		{
			var deviceConnectionInfo = _wearableControl.ActiveProvider.GetDeviceConnectionInfo();
			var productType = deviceConnectionInfo.GetProductType();
			var variantType = deviceConnectionInfo.GetVariantType();

			WearableProductImageFactory.WearableProductImage productImage;
			if (_productImageFactory.TryGetProductImages(productType, variantType, out productImage))
			{
				_productTourImage.sprite = productImage.productTourSprite;

				StartAnnotationAnimations(productImage.pointOfInterestAnchor);
			}
			else
			{
				DisableAllAnnotations();

				Debug.LogWarningFormat(CANNOT_FIND_PRODUCT_IMAGES_WARNING_FORMAT, productType, variantType);
			}

			_panel.EnableCloseButton(Color.white);

			Show();
		}

		private void OnSecurePairingFailure()
		{
			Hide();
		}

		private void OnSecurePairingSuccess()
		{
			Hide();
		}

		protected override void Hide()
		{
			base.Hide();

			StopAnnotationAnimations();
		}

		private void StartAnnotationAnimations(Vector2 anchor)
		{
			for (var i = 0; i < _annotationImageRectTransforms.Length; i++)
			{
				var rt = _annotationImageRectTransforms[i];
				rt.gameObject.SetActive(true);
				rt.anchorMin = anchor;
				rt.anchorMax = anchor;
				rt.anchoredPosition = Vector2.zero;
			}

			for (var i = 0; i < _animators.Length; i++)
			{
				_animators[i].enabled = true;
				_animators[i].Play(_playStateHash);
			}
		}

		private void DisableAllAnnotations()
		{
			for (var i = 0; i < _annotationImageRectTransforms.Length; i++)
			{
				var rt = _annotationImageRectTransforms[i];
				rt.gameObject.SetActive(false);
			}
		}

		private void StopAnnotationAnimations()
		{
			DisableAllAnnotations();

			for (var i = 0; i < _animators.Length; i++)
			{
				_animators[i].enabled = false;
			}
		}
	}
}
