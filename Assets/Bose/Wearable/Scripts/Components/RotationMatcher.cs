using Bose.Wearable.Extensions;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Automatically rotates a GameObject to match the orientation of the connected device.
	/// Provides both "absolute" and "relative" rotation modes.
	/// </summary>
	[AddComponentMenu("Bose/Wearable/RotationMatcher")]
	[DisallowMultipleComponent]
	public class RotationMatcher : MonoBehaviour
	{
		public enum RotationReference
		{
			/// <summary>
			/// In absolute mode, pointing the device such that identity rotation will orient the object's forward
			/// vector in the +Z direction.
			/// </summary>
			Absolute,

			/// <summary>
			/// In relative mode, the object is rotated with regards to a fixed reference orientation. Pointing in the
			/// direction of the reference orientation will orient the object's forward vector in the +Z direction.
			/// </summary>
			Relative
		}

		public enum RotationSensorSource
		{
			SixDof,
			NineDof
		}

		/// <summary>
		/// The rotation sensor to use when rotating the object.
		/// </summary>
		public RotationSensorSource RotationSource
		{
			get { return _rotationSource; }
			set
			{
				_rotationSource = value;
				CheckAndResolveRequirement();
			}
		}

		[SerializeField]
		private RotationSensorSource _rotationSource;

		/// <summary>
		/// The reference mode to use when rotating the object. See <see cref="RotationReference"/> for descriptions
		/// of each rotation mode.
		/// </summary>
		public RotationReference ReferenceMode
		{
			get { return _mode; }
		}

		/// <summary>
		/// Get the reference rotation. Returns <code>Quaternion.identity</code> if in "absolute" mode,
		/// and the previously-set reference if in "relative" mode.
		/// </summary>
		public Quaternion ReferenceRotation
		{
			get
			{
				if (_mode == RotationReference.Absolute)
				{
					return Quaternion.identity;
				}
				else
				{
					return Quaternion.Inverse(_inverseReference);
				}
			}
		}

		/// <summary>
		/// The minimum update interval to use.
		/// </summary>
		public SensorUpdateInterval UpdateInterval
		{
			get { return _updateInterval; }
			set
			{
				_updateInterval = value;
				CheckAndResolveRequirement();
			}
		}

		[SerializeField]
		private SensorUpdateInterval _updateInterval;

		private RotationReference _mode;
		private Quaternion _inverseReference;
		private WearableRequirement _requirement;
		private WearableControl _wearableControl;

		private void Awake()
		{
			_mode = RotationReference.Absolute;
			_inverseReference = Quaternion.identity;
			_requirement = null;
			_wearableControl = WearableControl.Instance;

			CheckAndResolveRequirement();
		}

		private void Update()
		{
			if (_wearableControl.ConnectedDevice == null)
			{
				return;
			}

			// Get a frame of sensor data. Since no integration is being performed, we can safely ignore all
			// intermediate frames and just grab the most recent.
			SensorFrame frame = _wearableControl.LastSensorFrame;

			if (_mode == RotationReference.Absolute)
			{
				// In absolute mode, match the rotation exactly.
				transform.rotation =
					(_rotationSource == RotationSensorSource.NineDof ?
						frame.rotationNineDof :
						frame.rotationSixDof);
			}
			else if (_mode == RotationReference.Relative)
			{
				// In relative mode, left-apply the inverse of the reference rotation to compute the relative change
				transform.rotation = _inverseReference *
					(_rotationSource == RotationSensorSource.NineDof ?
						frame.rotationNineDof :
						frame.rotationSixDof);
			}
		}

		private void Reset()
		{
			_updateInterval = WearableConstants.DEFAULT_UPDATE_INTERVAL;
			_rotationSource = RotationSensorSource.SixDof;
		}

		private void OnValidate()
		{
			if (Application.isPlaying && _requirement != null)
			{
				CheckAndResolveRequirement();
			}
		}

		/// <summary>
		/// Set rotation to always use the rotation from the latest <see cref="SensorFrame"/> when matching the
		/// rotation.
		/// </summary>
		public void SetAbsoluteReference()
		{
			_mode = RotationReference.Absolute;
		}

		/// <summary>
		/// Set the reference to the device's current orientation.
		/// </summary>
		public void SetRelativeReference()
		{
			_mode = RotationReference.Relative;

			if (_wearableControl.ConnectedDevice != null)
			{
				_inverseReference = Quaternion.Inverse(
					_rotationSource == RotationSensorSource.NineDof ?
						_wearableControl.LastSensorFrame.rotationNineDof :
						_wearableControl.LastSensorFrame.rotationSixDof);
			}
		}

		/// <summary>
		/// Set the <see cref="Quaternion"/> <paramref name="rotation"/> as a reference when matching the rotation.
		/// </summary>
		/// <param name="rotation"></param>
		public void SetRelativeReference(Quaternion rotation)
		{
			_mode = RotationReference.Relative;
			_inverseReference = Quaternion.Inverse(rotation);
		}

		/// <summary>
		/// Check that the required rotation sensor is enabled and rate is sufficient, resolve differences if not.
		/// Adds a <see cref="WearableRequirement"/> component on this GameObject if needed.
		/// </summary>
		private void CheckAndResolveRequirement()
		{
			bool showChangedRequirementWarnings = true;

			if (_requirement == null)
			{
				// If we don't have a requirement, look for one on the parent GameObject.
				_requirement = GetComponent<WearableRequirement>();
				if (_requirement == null)
				{
					// If there isn't one, create it.
					_requirement = gameObject.AddComponent<WearableRequirement>();
					Debug.LogFormat(WearableConstants.ROTATION_MATCHER_ADDED_REQUIREMENT_MESSAGE_FORMAT, name);

					// If it's a new requirement, suppress the alteration warnings.
					showChangedRequirementWarnings = false;
				}
			}

			// Check that the attached requirement's update interval is fast enough.
			if (_requirement.DeviceConfig.updateInterval.IsSlowerThan(_updateInterval))
			{
				_requirement.SetSensorUpdateInterval(_updateInterval);
				if (showChangedRequirementWarnings)
				{
					Debug.LogWarningFormat(
						WearableConstants.ROTATION_MATCHER_CHANGED_REQUIREMENT_UPDATE_INTERVAL_WARNING_FORMAT,
						name);
				}
			}

			// Check to ensure the proper sensor is enabled
			SensorId rotationSensorId = _rotationSource == RotationSensorSource.NineDof ?
											SensorId.RotationNineDof :
											SensorId.RotationSixDof;
			var config = _requirement.DeviceConfig.GetSensorConfig(rotationSensorId);
			if (!config.isEnabled)
			{
				// Switch to proper sensor and turn off the unused one.
				// If the user has manually enabled both sensors, this will not turn any sensors off when changing mode.
				_requirement.EnableSensor(rotationSensorId);
				_requirement.DisableSensor(
					rotationSensorId == SensorId.RotationSixDof ?
						SensorId.RotationNineDof :
						SensorId.RotationSixDof);

				if (showChangedRequirementWarnings)
				{
					Debug.LogWarningFormat(
						WearableConstants.ROTATION_MATCHER_CHANGED_REQUIREMENT_SENSORS_WARNING_FORMAT,
						name);
				}
			}
		}
	}
}
