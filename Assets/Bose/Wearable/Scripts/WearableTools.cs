using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Provides general-use utilities for working with the WearablePlugin.
	/// </summary>
	public static class WearableTools
	{
		// mapping from device-agnostic gestures to device-specific gestures, by product.
		// For example, on Bose Frames, a head nod means affirmative.
		private static Dictionary<ProductType, Dictionary<GestureId, GestureId>> _agnosticToSpecificGesture;

		private static List<ActiveNoiseReductionMode> _anrModeList;

		/// <summary>
		/// Static constructor.
		/// </summary>
		static WearableTools()
		{
			_agnosticToSpecificGesture = new Dictionary<ProductType, Dictionary<GestureId, GestureId>>()
		   {
				// mappings for frames
				{
					ProductType.Frames, new Dictionary<GestureId, GestureId>()
					{
						{ GestureId.Input, GestureId.DoubleTap },
						{ GestureId.Affirmative, GestureId.HeadNod },
						{ GestureId.Negative, GestureId.HeadShake }
					}
				},
				// mappings for QC35II headphones
				{
					ProductType.QuietComfort35Two, new Dictionary<GestureId, GestureId>()
					{
						{ GestureId.Input, GestureId.DoubleTap },
						{ GestureId.Affirmative, GestureId.HeadNod },
						{ GestureId.Negative, GestureId.HeadShake }
					}
				},
			   // mappings for NC700 headphones
			   {
				   ProductType.NoiseCancellingHeadphones700, new Dictionary<GestureId, GestureId>()
				   {
					   { GestureId.Input, GestureId.TouchAndHold },
					   { GestureId.Affirmative, GestureId.HeadNod },
					   { GestureId.Negative, GestureId.HeadShake }
				   }
			   }
		   };

			_anrModeList = new List<ActiveNoiseReductionMode>();
		}

		/// <summary>
		/// Get the number of seconds between samples for a given <see cref="SensorUpdateInterval"/>.
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public static float SensorUpdateIntervalToSeconds(SensorUpdateInterval interval)
		{
			// This is needed because the update interval enum doesn't reflect the actual sampling interval,
			// but the values of the underlying SDK enum.
			switch (interval)
			{
				case SensorUpdateInterval.TwentyMs:
					return 0.020f;
				case SensorUpdateInterval.FortyMs:
					return 0.040f;
				case SensorUpdateInterval.EightyMs:
					return 0.080f;
				case SensorUpdateInterval.OneHundredSixtyMs:
					return 0.160f;
				case SensorUpdateInterval.ThreeHundredTwentyMs:
					return 0.320f;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Returns the <see cref="SensorUpdateInterval"/> with a value equal to or slower than the given sampling
		/// period in milliseconds.
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <returns></returns>
		public static SensorUpdateInterval MillisecondsToClosestSensorUpdateInterval(int milliseconds)
		{
			if (milliseconds <= 20)
			{
				return SensorUpdateInterval.TwentyMs;
			}
			else if (milliseconds <= 40)
			{
				return SensorUpdateInterval.FortyMs;
			}
			else if (milliseconds <= 80)
			{
				return SensorUpdateInterval.EightyMs;
			}
			else if (milliseconds <= 160)
			{
				return SensorUpdateInterval.OneHundredSixtyMs;
			}
			else
			{
				return SensorUpdateInterval.ThreeHundredTwentyMs;
			}
		}

		/// <summary>
		/// Get the number of milliseconds between samples for a given <see cref="SensorUpdateInterval"/>.
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public static float SensorUpdateIntervalToMilliseconds(SensorUpdateInterval interval)
		{
			return SensorUpdateIntervalToSeconds(interval) * 1000.0f;
		}

		/// <summary>
		/// Get the Bose SDK bit for a SensorUpdateInterval.
		/// </summary>
		/// <param name="interval"></param>
		/// <returns></returns>
		public static int SensorUpdateIntervalToBit(SensorUpdateInterval interval)
		{
			int bit = 0;

			switch (interval)
			{
				case SensorUpdateInterval.ThreeHundredTwentyMs:
					bit = 1;
					break;
				case SensorUpdateInterval.OneHundredSixtyMs:
					bit = 2;
					break;
				case SensorUpdateInterval.EightyMs:
					bit = 4;
					break;
				case SensorUpdateInterval.FortyMs:
					bit = 8;
					break;
				case SensorUpdateInterval.TwentyMs:
					bit = 16;
					break;
				default:
					throw new ArgumentOutOfRangeException("interval", interval, null);
			}

			return bit;
		}

		/// <summary>
		/// Returns the appropriate <see cref="SensorFlags"/> for the passed individual <see cref="SensorId"/>
		/// <paramref name="sensorId"/>.
		/// </summary>
		/// <param name="sensorId"></param>
		/// <returns></returns>
		internal static SensorFlags GetSensorFlag(SensorId sensorId)
		{
			SensorFlags flag;
			switch (sensorId)
			{
				case SensorId.Accelerometer:
					flag = SensorFlags.Accelerometer;
					break;
				case SensorId.Gyroscope:
					flag = SensorFlags.Gyroscope;
					break;
				case SensorId.RotationNineDof:
					flag = SensorFlags.RotationNineDof;
					break;
				case SensorId.RotationSixDof:
					flag = SensorFlags.RotationSixDof;
					break;
				default:
					throw new ArgumentOutOfRangeException("sensorId", sensorId, null);
			}

			return flag;
		}

		/// <summary>
		/// Returns the appropriate <see cref="GestureFlags"/> for the passed individual <see cref="GestureId"/>
		/// <paramref name="gestureId"/>.
		/// </summary>
		/// <param name="gestureId"></param>
		/// <returns></returns>
		internal static GestureFlags GetGestureFlag(GestureId gestureId)
		{
			GestureFlags flag;
			switch (gestureId)
			{
				case GestureId.DoubleTap:
					flag = GestureFlags.DoubleTap;
					break;
				case GestureId.HeadNod:
					flag = GestureFlags.HeadNod;
					break;
				case GestureId.HeadShake:
					flag = GestureFlags.HeadShake;
					break;
				case GestureId.TouchAndHold:
					flag = GestureFlags.TouchAndHold;
					break;
				case GestureId.Input:
					flag = GestureFlags.Input;
					break;
				case GestureId.Affirmative:
					flag = GestureFlags.Affirmative;
					break;
				case GestureId.Negative:
					flag = GestureFlags.Negative;
					break;
				default:
					throw new ArgumentOutOfRangeException("gestureId", gestureId, null);
			}

			return flag;
		}

		/// <summary>
		/// Returns the appropriate <see cref="OSPermissionFlags"/> for the passed individual <see cref="OSPermission"/>
		/// <paramref name="permission"/>.
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		internal static OSPermissionFlags GetOSPermissionFlag(OSPermission permission)
		{
			OSPermissionFlags flag;
			switch (permission)
			{
				case OSPermission.Bluetooth:
					flag = OSPermissionFlags.Bluetooth;
					break;
				case OSPermission.Location:
					flag = OSPermissionFlags.Location;
					break;
				default:
					throw new ArgumentOutOfRangeException("permission", permission, null);
			}

			return flag;
		}

		/// <summary>
		/// Returns the appropriate <see cref="OSServiceFlags"/> for the passed individual <see cref="OSService"/>
		/// <paramref name="service"/>.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		internal static OSServiceFlags GetOSServiceFlag(OSService service)
		{
			OSServiceFlags flag;
			switch (service)
			{
				case OSService.Bluetooth:
					flag = OSServiceFlags.Bluetooth;
					break;
				case OSService.LocationServices:
					flag = OSServiceFlags.LocationServices;
					break;
				default:
					throw new ArgumentOutOfRangeException("service", service, null);
			}

			return flag;
		}

		/// <summary>
		/// Each product has its own mapping from physical gestures to abstract ones.  If we do not know
		/// about this product or this gesture we return GestureId.None.
		/// </summary>
		public static GestureId GetDeviceSpecificGestureForProduct(GestureId deviceAgnosticGestureId, ProductType product)
		{
			GestureId abstractGesture = GestureId.None;

			if (deviceAgnosticGestureId.IsGestureDeviceAgnostic())
			{
				Debug.LogWarningFormat(WearableConstants.GESTURE_NOT_DEVICE_AGNOSTIC_WARNING, deviceAgnosticGestureId);
			}
			else
			{
				Dictionary<GestureId, GestureId> mappingForProduct = null;
				if (_agnosticToSpecificGesture.TryGetValue(product, out mappingForProduct))
				{
					mappingForProduct.TryGetValue(deviceAgnosticGestureId, out abstractGesture);
				}
			}

			return abstractGesture;
		}

		internal static ProductId GetProductId(ProductType productType)
		{
			var productIds = (ProductId[])Enum.GetValues(typeof(ProductId));
			for (int i = 0; i < productIds.Length; i++)
			{
				if (GetProductType(productIds[i]) == productType)
				{
					return productIds[i];
				}
			}

			return ProductId.Undefined;
		}

		internal static ProductType GetProductType(ProductId productId)
		{
			var productType = ProductType.Unknown;
			switch (productId)
			{
				case ProductId.Frames:
				case ProductId.Frames2:
					productType = ProductType.Frames;
					break;
				case ProductId.QuietComfort35Two:
					productType = ProductType.QuietComfort35Two;
					break;
				case ProductId.NoiseCancellingHeadphones700:
					productType = ProductType.NoiseCancellingHeadphones700;
					break;
			}

			return productType;
		}

		internal static byte GetVariantId(ProductType productType, VariantType variantType)
		{
			byte variantId = 0;

			switch (productType)
			{
				case ProductType.Frames:
					variantId = GetVariantId<FramesVariantId>(productType, variantType);
					break;
				case ProductType.QuietComfort35Two:
					variantId = GetVariantId<QuietComfort35TwoVariantId>(productType, variantType);
					break;
				case ProductType.NoiseCancellingHeadphones700:
					variantId = GetVariantId<NoiseCancellingHeadphones700VariantId>(productType, variantType);
					break;
			}

			return variantId;
		}

		private static byte GetVariantId<T>(ProductType productType, VariantType variantType)
		{
			var variantIds = (byte[])Enum.GetValues(typeof(T));
			for (int i = 0; i < variantIds.Length; i++)
			{
				if (GetVariantType(productType, variantIds[i]) == variantType)
				{
					return variantIds[i];
				}
			}

			// "undefined" variantId.
			return 0;
		}

		internal static VariantType GetVariantType(ProductType productType, byte variantId)
		{
			var variantType = VariantType.Undefined;
			switch (productType)
			{
				case ProductType.Frames:
					variantType = GetFramesVariantType(variantId);
					break;
				case ProductType.QuietComfort35Two:
					variantType = GetQuietComfort35TwoVariantType(variantId);
					break;
				case ProductType.NoiseCancellingHeadphones700:
					variantType = GetNoiseCancellingHeadphones700VariantType(variantId);
					break;
			}

			return variantType;
		}

		private static VariantType GetFramesVariantType(byte variantId)
		{
			var variantType = VariantType.Undefined;
			var specificVariant = (FramesVariantId)variantId;
			if (Enum.IsDefined(typeof(FramesVariantId), specificVariant))
			{
				switch (specificVariant)
				{
					case FramesVariantId.Alto:
						variantType = VariantType.FramesAlto;
						break;
					case FramesVariantId.Rondo:
						variantType = VariantType.FramesRondo;
						break;
				}
			}

			return variantType;
		}

		private static VariantType GetQuietComfort35TwoVariantType(byte variantId)
		{
			var variantType = VariantType.Undefined;
			var specificVariant = (QuietComfort35TwoVariantId)variantId;
			if (Enum.IsDefined(typeof(QuietComfort35TwoVariantId), specificVariant))
			{
				switch (specificVariant)
				{
					case QuietComfort35TwoVariantId.Black:
						variantType = VariantType.QuietComfort35TwoBlack;
						break;
					case QuietComfort35TwoVariantId.Silver:
						variantType = VariantType.QuietComfort35TwoSilver;
						break;
					case QuietComfort35TwoVariantId.RoseGold:
						variantType = VariantType.QuietComfort35TwoRoseGold;
						break;
				}
			}

			return variantType;
		}

		private static VariantType GetNoiseCancellingHeadphones700VariantType(byte variantId)
		{
			var variantType = VariantType.Undefined;
			var specificVariant = (NoiseCancellingHeadphones700VariantId)variantId;
			if (Enum.IsDefined(typeof(NoiseCancellingHeadphones700VariantId), specificVariant))
			{
				switch (specificVariant)
				{
					case NoiseCancellingHeadphones700VariantId.Black:
						variantType = VariantType.NoiseCancellingHeadphones700Black;
						break;
					case NoiseCancellingHeadphones700VariantId.Silver:
						variantType = VariantType.NoiseCancellingHeadphones700Silver;
						break;
				}
			}

			return variantType;
		}

		internal static string[] GetVariantNamesForProduct(ProductType productType)
		{
			string[] result = null;

			switch (productType)
			{
				case ProductType.Frames:
					result = Enum.GetNames(typeof(FramesVariantId));
					break;
				case ProductType.QuietComfort35Two:
					result = Enum.GetNames(typeof(QuietComfort35TwoVariantId));
					break;
				case ProductType.NoiseCancellingHeadphones700:
					result = Enum.GetNames(typeof(NoiseCancellingHeadphones700VariantId));
					break;
				case ProductType.Unknown:
					result = new string[] { VariantType.Undefined.ToString() };
					break;
			}

			return result;
		}

		internal static byte[] GetVariantValuesForProduct(ProductType productType)
		{
			byte[] result = null;

			switch (productType)
			{
				case ProductType.Frames:
					result = (byte[])Enum.GetValues(typeof(FramesVariantId));
					break;
				case ProductType.QuietComfort35Two:
					result = (byte[])Enum.GetValues(typeof(QuietComfort35TwoVariantId));
					break;
				case ProductType.NoiseCancellingHeadphones700:
					result = (byte[])Enum.GetValues(typeof(NoiseCancellingHeadphones700VariantId));
					break;
				case ProductType.Unknown:
					result = new byte[] { 0 };
					break;
			}

			return result;
		}

		/// <summary>
		/// Packs the supplied list of <see cref="ActiveNoiseReductionMode"/>s into an int in the same manner as the
		/// underlying SDK. Duplicated modes are transparently ignored. Supplying
		/// <see cref="ActiveNoiseReductionMode.Invalid"/> will throw an exception.
		/// </summary>
		/// <param name="modes"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		internal static int GetActiveNoiseReductionModesAsInt(ActiveNoiseReductionMode[] modes)
		{
			int result = 0;
			for (int i = 0; i < modes.Length; i++)
			{
				ActiveNoiseReductionMode mode = modes[i];
				if (mode == ActiveNoiseReductionMode.Invalid)
				{
					throw new ArgumentOutOfRangeException(WearableConstants.INVALID_IS_INVALID_ANR_MODE);
				}

				// Set the corresponding bit in the availability mask.
				result |= 1 << (int) mode;
			}

			return result;
		}

		/// <summary>
		/// Unpacks an int representing the available <see cref="ActiveNoiseReductionMode"/>s in the same manner
		/// as the underlying SDK. Neither duplicated modes nor <see cref="ActiveNoiseReductionMode.Invalid"/> will
		/// be returned in the resultant list; list order is undefined.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		internal static ActiveNoiseReductionMode[] GetActiveNoiseReductionModesAsList(int flags)
		{
			if (flags == 0)
			{
				// Feature disabled; signify with an empty list.
				return WearableConstants.EMPTY_ACTIVE_NOISE_REDUCTION_MODES;
			}

			_anrModeList.Clear();

			for (int i = 0; i < WearableConstants.ACTIVE_NOISE_REDUCTION_MODES.Length; i++)
			{
				ActiveNoiseReductionMode mode = WearableConstants.ACTIVE_NOISE_REDUCTION_MODES[i];

				if (mode == ActiveNoiseReductionMode.Invalid)
				{
					continue;
				}

				// Check if the corresponding bit is set in the availability mask.
				if (((1 << (int) mode) & flags) != 0)
				{
					_anrModeList.Add(mode);
				}
			}

			return _anrModeList.ToArray();
		}
	}
}
