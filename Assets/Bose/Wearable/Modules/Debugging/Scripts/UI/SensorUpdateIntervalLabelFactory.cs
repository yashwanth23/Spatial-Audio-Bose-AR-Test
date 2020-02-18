using System;
using UnityEngine;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using System.Linq;
#endif

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="SensorUpdateIntervalLabelFactory"/> is a factory for mapping and returning labels for the
	/// appropriate <see cref="SensorUpdateInterval"/>.
	/// </summary>
	internal sealed class SensorUpdateIntervalLabelFactory : ScriptableObject
	{
		[Serializable]
		private class UpdateIntervalToLabel
		{
			#pragma warning disable 0649

			public SensorUpdateInterval updateInterval;
			public string label;

			#pragma warning restore 0649
		}

		[SerializeField]
		private UpdateIntervalToLabel[] _updateIntervalToLabels;

		/// <summary>
		/// Returns a label for the passed <see cref="SensorUpdateInterval"/> <paramref name="updateInterval"/>.
		/// </summary>
		/// <param name="updateInterval"></param>
		/// <returns></returns>
		public string GetLabel(SensorUpdateInterval updateInterval)
		{
			for (var i = 0; i < _updateIntervalToLabels.Length; i++)
			{
				if (_updateIntervalToLabels[i].updateInterval != updateInterval)
				{
					continue;
				}

				return _updateIntervalToLabels[i].label;
			}

			return string.Empty;
		}

		#if UNITY_EDITOR

		private void Reset()
		{
			_updateIntervalToLabels = new UpdateIntervalToLabel[WearableConstants.UPDATE_INTERVALS.Length];

			for (var i = 0; i < _updateIntervalToLabels.Length; i++)
			{
				_updateIntervalToLabels[i].updateInterval = WearableConstants.UPDATE_INTERVALS[i];
			}
		}

		private void OnValidate()
		{
			for (var i = 0; i < _updateIntervalToLabels.Length; i++)
			{
				Assert.IsFalse(string.IsNullOrEmpty(_updateIntervalToLabels[i].label));
			}

			// Validate that there is a 1:1 mapping for update interval to label and that the label is not blank.
			for (var i = 0; i < WearableConstants.UPDATE_INTERVALS.Length; i++)
			{
				var updateInterval = WearableConstants.UPDATE_INTERVALS[i];
				Assert.IsTrue(_updateIntervalToLabels.Any(x => x.updateInterval == updateInterval));
				Assert.IsTrue(_updateIntervalToLabels.Count(x => x.updateInterval == updateInterval) == 1);
				Assert.IsFalse(string.IsNullOrEmpty(_updateIntervalToLabels[i].label));
			}
		}

		#endif
	}
}
