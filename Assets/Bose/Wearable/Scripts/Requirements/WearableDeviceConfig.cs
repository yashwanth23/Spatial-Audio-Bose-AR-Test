using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the state of a WearableDevice including its sensors and gestures.
	/// </summary>
	[Serializable]
	internal class WearableDeviceConfig
	{
		/// <summary>
		/// Represents the state of a WearableDevice's gesture
		/// </summary>
		[Serializable]
		internal class WearableGestureConfig
		{
			public bool isEnabled;

			/// <summary>
			/// Returns a deep-copy of this <see cref="WearableGestureConfig"/> instance.
			/// </summary>
			/// <returns></returns>
			public WearableGestureConfig Clone()
			{
				return new WearableGestureConfig { isEnabled = isEnabled };
			}
		}

		/// <summary>
		/// Represents the state of a WearableDevice's sensor
		/// </summary>
		[Serializable]
		internal class WearableSensorConfig
		{
			public bool isEnabled;

			/// <summary>
			/// Returns a deep-copy of this <see cref="WearableSensorConfig"/> instance.
			/// </summary>
			/// <returns></returns>
			public WearableSensorConfig Clone()
			{
				return new WearableSensorConfig {isEnabled = isEnabled};
			}
		}

		public WearableSensorConfig accelerometer;

		public WearableSensorConfig gyroscope;

		public WearableSensorConfig rotationNineDof;

		public WearableSensorConfig rotationSixDof;

		// device-specific gestures
		[FormerlySerializedAs("doubleTap")]
		public WearableGestureConfig doubleTapGesture;
		[FormerlySerializedAs("headNod")]
		public WearableGestureConfig headNodGesture;
		[FormerlySerializedAs("headShake")]
		public WearableGestureConfig headShakeGesture;
		public WearableGestureConfig touchAndHoldGesture;

		// Device-agnostic gestures
		public WearableGestureConfig inputGesture;
		public WearableGestureConfig affirmativeGesture;
		public WearableGestureConfig negativeGesture;

		public SensorUpdateInterval updateInterval;

		public WearableDeviceConfig()
		{
			accelerometer = new WearableSensorConfig();
			gyroscope = new WearableSensorConfig();
			rotationNineDof = new WearableSensorConfig();
			rotationSixDof = new WearableSensorConfig();

			doubleTapGesture = new WearableGestureConfig();
			headNodGesture = new WearableGestureConfig();
			headShakeGesture = new WearableGestureConfig();
			touchAndHoldGesture = new WearableGestureConfig();

			inputGesture = new WearableGestureConfig();
			affirmativeGesture = new WearableGestureConfig();
			negativeGesture = new WearableGestureConfig();

			updateInterval = WearableConstants.DEFAULT_UPDATE_INTERVAL;
		}

		/// <summary>
		/// Sets <see cref="WearableSensorConfig"/> <paramref name="config"/> for the appropriate sensor
		/// based on the passed <see cref="SensorId"/> <paramref name="sensorId"/>.
		/// </summary>
		/// <param name="sensorId"></param>
		/// <param name="config"></param>
		public void SetSensorConfig(SensorId sensorId, WearableSensorConfig config)
		{
			switch (sensorId)
			{
				case SensorId.Accelerometer:
					accelerometer = config;
					break;
				case SensorId.Gyroscope:
					gyroscope = config;
					break;
				case SensorId.RotationNineDof:
					rotationNineDof = config;
					break;
				case SensorId.RotationSixDof:
					rotationSixDof = config;
					break;
				default:
					throw new ArgumentOutOfRangeException("sensorId", sensorId, null);
			}
		}

		/// <summary>
		/// Returns an appropriate <see cref="WearableSensorConfig"/> for the passed <see cref="SensorId"/>
		/// <paramref name="sensorId"/>
		/// </summary>
		/// <param name="sensorId"></param>
		/// <returns></returns>
		public WearableSensorConfig GetSensorConfig(SensorId sensorId)
		{
			WearableSensorConfig config;
			switch (sensorId)
			{
				case SensorId.Accelerometer:
					config = accelerometer;
					break;
				case SensorId.Gyroscope:
					config = gyroscope;
					break;
				case SensorId.RotationNineDof:
					config = rotationNineDof;
					break;
				case SensorId.RotationSixDof:
					config = rotationSixDof;
					break;
				default:
					throw new ArgumentOutOfRangeException("sensorId", sensorId, null);
			}

			return config;
		}

		/// <summary>
		/// Sets <see cref="WearableGestureConfig"/> <paramref name="config"/> for the appropriate sensor
		/// based on the passed <see cref="GestureId"/> <paramref name="gestureId"/>.
		/// </summary>
		/// <param name="gestureId"></param>
		/// <param name="config"></param>
		public void SetGestureConfig(GestureId gestureId, WearableGestureConfig config)
		{
			switch (gestureId)
			{
				case GestureId.DoubleTap:
					doubleTapGesture = config;
					break;
				case GestureId.HeadNod:
					headNodGesture = config;
					break;
				case GestureId.HeadShake:
					headShakeGesture = config;
					break;
				case GestureId.None:
				default:
					throw new ArgumentOutOfRangeException("gestureId", gestureId, null);
			}
		}

		/// <summary>
		/// Returns an appropriate <see cref="WearableGestureConfig"/> for the passed <see cref="GestureId"/>
		/// <paramref name="gestureId"/>
		/// </summary>
		/// <param name="gestureId"></param>
		/// <returns></returns>
		public WearableGestureConfig GetGestureConfig(GestureId gestureId)
		{
			WearableGestureConfig config;
			switch (gestureId)
			{
				case GestureId.DoubleTap:
					config = doubleTapGesture;
					break;
				case GestureId.HeadNod:
					config = headNodGesture;
					break;
				case GestureId.HeadShake:
					config = headShakeGesture;
					break;
				case GestureId.TouchAndHold:
					config = touchAndHoldGesture;
					break;
				case GestureId.Input:
					config = inputGesture;
					break;
				case GestureId.Affirmative:
					config = affirmativeGesture;
					break;
				case GestureId.Negative:
					config = negativeGesture;
					break;
				case GestureId.None:
				default:
					throw new ArgumentOutOfRangeException("gestureId", gestureId, null);
			}

			return config;
		}

		/// <summary>
		/// Disables all sensors on this configuration.
		/// </summary>
		public void DisableAllSensors()
		{
			for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
			{
				GetSensorConfig(WearableConstants.SENSOR_IDS[i]).isEnabled = false;
			}
		}

		/// <summary>
		/// Disables all gestures on this configuration.
		/// </summary>
		public void DisableAllGestures()
		{
			for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				if (WearableConstants.GESTURE_IDS[i] == GestureId.None)
				{
					continue;
				}

				GetGestureConfig(WearableConstants.GESTURE_IDS[i]).isEnabled = false;
			}
		}

		/// <summary>
		/// Copy only the sensor configuration from another <see cref="WearableDeviceConfig"/>. Includes the sensor
		/// update interval.
		/// </summary>
		/// <param name="config"></param>
		public void CopySensorConfigFrom(WearableDeviceConfig config)
		{
			for (int i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
			{
				SensorId sensorId = WearableConstants.SENSOR_IDS[i];
				GetSensorConfig(sensorId).isEnabled = config.GetSensorConfig(sensorId).isEnabled;
			}
			updateInterval = config.updateInterval;
		}

		/// <summary>
		/// Copy only the gesture configuration from another <see cref="WearableDeviceConfig"/>.
		/// </summary>
		/// <param name="config"></param>
		public void CopyGestureConfigFrom(WearableDeviceConfig config)
		{
			for (int i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				GestureId gestureId = WearableConstants.GESTURE_IDS[i];

				if (gestureId == GestureId.None)
				{
					continue;
				}

				GetGestureConfig(gestureId).isEnabled = config.GetGestureConfig(gestureId).isEnabled;
			}
		}

		/// <summary>
		/// Copy all the configuration values from the specified configuration.
		/// </summary>
		/// <param name="config"></param>
		public void CopyValuesFrom(WearableDeviceConfig config)
		{
			CopySensorConfigFrom(config);
			CopyGestureConfigFrom(config);
		}

		/// <summary>
		/// Returns a deep copy of this configuration.
		/// </summary>
		/// <returns></returns>
		public WearableDeviceConfig Clone()
		{
			var result = new WearableDeviceConfig();
			result.CopyValuesFrom(this);
			return result;
		}

		/// <summary>
		/// Returns true if any sensors are enabled.
		/// </summary>
		/// <returns></returns>
		internal bool HasAnySensorsEnabled()
		{
			return GetNumberOfEnabledSensors() > 0;
		}

		/// <summary>
		/// Returns true if three or more sensors are enabled.
		/// </summary>
		/// <returns></returns>
		internal bool HasThreeOrMoreSensorsEnabled()
		{
			return GetNumberOfEnabledSensors() >= 3;
		}

		/// <summary>
		/// Returns true if any gestures are enabled, otherwise false.
		/// </summary>
		/// <returns></returns>
		internal bool AreAnyGesturesEnabled()
		{
			var result = false;
			for (var i = 0; i < WearableConstants.GESTURE_IDS.Length; i++)
			{
				var gestureId = WearableConstants.GESTURE_IDS[i];
				if (gestureId == GestureId.None)
				{
					continue;
				}

				result |= GetGestureConfig(gestureId).isEnabled;
			}

			return result;
		}

		/// <summary>
		/// Returns the number of sensor configs that are enabled.
		/// </summary>
		/// <returns></returns>
		private int GetNumberOfEnabledSensors()
		{
			var wearableControl = Application.isPlaying && WearableControl.Exists
				? WearableControl.Instance
				: null;

			var numberOfSensorsActive = 0;
			for (var i = 0; i < WearableConstants.SENSOR_IDS.Length; i++)
			{
				var sensorId = WearableConstants.SENSOR_IDS[i];
				var sensor = GetSensorConfig(sensorId);
				if (!sensor.isEnabled ||
				    wearableControl != null && !wearableControl.GetWearableSensorById(sensorId).IsAvailable)
				{
					continue;
				}

				numberOfSensorsActive++;
			}

			return numberOfSensorsActive;
		}
	}
}
