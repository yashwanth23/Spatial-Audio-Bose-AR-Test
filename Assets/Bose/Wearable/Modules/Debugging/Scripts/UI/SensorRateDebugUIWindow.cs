using Bose.Wearable.Extensions;
using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="SensorRateDebugUIWindow"/> sets up 1:1 <see cref="SensorRateToggleDebugUIControl"/> for all
	/// available <see cref="SensorUpdateInterval"/>(s).
	/// </summary>
	internal sealed class SensorRateDebugUIWindow : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private Transform _sensorRateRootTransform;

		[Header("Prefabs"), Space(5)]
		[SerializeField]
		private SensorRateToggleDebugUIControl _sensorRateToggleDebugUIControl;

		#pragma warning restore 0649

		private void Awake()
		{
			// Clear out any items on the root transforms prior to setting it up with new prefabs
			_sensorRateRootTransform.DeleteAllChildren();

			// Instantiate 1:1 prefab for distinct sensor update interval value.
			for (var i = 0; i < WearableConstants.UPDATE_INTERVALS.Length; i++)
			{
				var sensorRateToggle = Instantiate(
					_sensorRateToggleDebugUIControl,
					_sensorRateRootTransform,
					false);

				sensorRateToggle.SetUpdateInterval(WearableConstants.UPDATE_INTERVALS[i]);
			}
		}
	}
}
