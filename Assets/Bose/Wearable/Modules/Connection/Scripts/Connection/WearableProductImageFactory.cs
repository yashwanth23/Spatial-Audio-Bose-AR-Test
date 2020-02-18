using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace Bose.Wearable
{
	internal sealed class WearableProductImageFactory : ScriptableObject
	{
		[Serializable]
		internal class WearableProductImage
		{
			#pragma warning disable 0649
			public Sprite productTourSprite;
			public Vector2 pointOfInterestAnchor;
			#pragma warning disable 0649

			public WearableProductImage()
			{
				pointOfInterestAnchor = new Vector2(0.5f, 0.5f);
			}
		}

		[Serializable]
		private class WearableProductImageMapping : WearableProductImage
		{
			#pragma warning disable 0649
			public ProductType productType;
			#pragma warning disable 0649
		}

		[Serializable]
		private class WearableVariantImageMapping : WearableProductImage
		{
			#pragma warning disable 0649
			public VariantType variantType;
			#pragma warning disable 0649
		}

		[Header("Data")]
		[SerializeField]
		private WearableProductImage _fallbackBackgroundImage;

		[SerializeField]
		private WearableProductImageMapping[] _productMappings;

		[SerializeField]
		private WearableVariantImageMapping[] _variantMappings;

		private bool _isSetup;

		private Dictionary<ProductType, WearableProductImageMapping> _productImageLookup;
		private Dictionary<VariantType, WearableVariantImageMapping> _variantImageLookup;

		private const string DUPLICATE_PRODUCT_MAPPING_WARNING =
			"[BoseWearable] Product Image Mapping for ProductType [{0}] will not be added as it already" +
			" exists (duplicate).";

		private const string DUPLICATE_VARIANT_MAPPING_WARNING =
			"[BoseWearable] Product Image Mapping for VariantType [{0}] will not be added as it already" +
			" exists (duplicate).";

		/// <summary>
		/// Returns true if a background and model sprite was found for the passed
		/// <paramref name="productType"/> and <paramref name="variantType"/>, otherwise false.
		/// </summary>
		public bool TryGetProductImages(
			ProductType productType,
			VariantType variantType,
			out WearableProductImage productImage)
		{
			if (!_isSetup)
			{
				Setup();
			}

			productImage = _fallbackBackgroundImage;

			WearableVariantImageMapping variantMapping;
			WearableProductImageMapping productMapping;
			if (_variantImageLookup.TryGetValue(variantType, out variantMapping))
			{
				productImage = variantMapping;
			}
			else if (_productImageLookup.TryGetValue(productType, out productMapping))
			{
				productImage = productMapping;
			}

			return productImage != null;
		}

		private void Setup()
		{
			_productImageLookup = new Dictionary<ProductType, WearableProductImageMapping>();
			_variantImageLookup = new Dictionary<VariantType, WearableVariantImageMapping>();

			for (var i = 0; i < _productMappings.Length; i++)
			{
				if (!_productImageLookup.ContainsKey(_productMappings[i].productType))
				{
					_productImageLookup.Add(_productMappings[i].productType, _productMappings[i]);
				}
				else
				{
					Debug.LogWarningFormat(DUPLICATE_PRODUCT_MAPPING_WARNING, _productMappings[i].productType);
				}
			}

			for (var i = 0; i < _variantMappings.Length; i++)
			{
				if (!_variantImageLookup.ContainsKey(_variantMappings[i].variantType))
				{
					_variantImageLookup.Add(_variantMappings[i].variantType, _variantMappings[i]);
				}
				else
				{
					Debug.LogWarningFormat(DUPLICATE_VARIANT_MAPPING_WARNING, _variantMappings[i].variantType);
				}
			}
		}

		#if UNITY_EDITOR

		private void OnValidate()
		{
			for (var i = 0; i < _productMappings.Length; i++)
			{
				Assert.IsNotNull(_productMappings[i].productTourSprite);
			}

			for (var i = 0; i < _variantMappings.Length; i++)
			{
				Assert.IsNotNull(_variantMappings[i].productTourSprite);
			}
		}

		#endif
	}
}
