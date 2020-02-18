using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="TimingDebugUIWindow"/> is a debug window for showing the device's current and cached
	/// recent sensor frames.
	/// </summary>
	[RequireComponent(typeof(DataMetrics))]
	internal sealed class TimingDebugUIWindow : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UI Refs: Unity-Specific")]
		[SerializeField]
		private Text _renderFramesPerSecond;

		[SerializeField]
		private Text _sensorFramesPerRenderFrames;

		[SerializeField]
		private Text _lastTransferInterval;

		[SerializeField]
		private Text _lastTransferAge;

		[SerializeField]
		private Text _sensorFramesPerSecond;

		[SerializeField]
		private Text _unityToDeviceOffset;

		[Header("UI Refs: Bridge-Specific"), Space(5)]
		[SerializeField]
		private Text _frameTimestamp;

		[SerializeField]
		private Text _frameDelta;

		[Header("Data Refs"), Space(5)]
		[SerializeField]
		private DataMetrics _dataMetrics;

		[Header("Data Refs"), Space(5)]
		[Range(0, 10)]
		[SerializeField]
		private int _sensorFramesPerRenderFrameCaptureLimit;

		#pragma warning restore 0649

		private int _captureIndex;
		private float[] _capturedSensorFramesPerRenderFrame;
		private StringBuilder _captureStringBuilder;

		private const char EMPTY_SPACE = ' ';

		private void Awake()
		{
			_capturedSensorFramesPerRenderFrame = new float[_sensorFramesPerRenderFrameCaptureLimit];
			_captureStringBuilder = new StringBuilder();
		}

		private void Update()
		{
			UpdateSensorFramesPerRenderFrameCapture();

			_frameTimestamp.text = _dataMetrics.LastTimestamp.HasValue
				? string.Format(DebuggingConstants.SECONDS_FORMAT, _dataMetrics.LastTimestamp)
				: string.Empty;

			_frameDelta.text = _dataMetrics.LastDeltaTime.HasValue
				? string.Format(DebuggingConstants.SECONDS_FORMAT, _dataMetrics.LastDeltaTime)
				: string.Empty;

			_renderFramesPerSecond.text = string.Format(
				DebuggingConstants.FRAMES_PER_SECOND_FORMAT,
				_dataMetrics.RenderFramesLastSecond);

			_sensorFramesPerRenderFrames.text = GetRenderFrameCapturesString();

			_lastTransferInterval.text = _dataMetrics.LastTransferInterval.HasValue
				? string.Format(DebuggingConstants.SECONDS_FORMAT, _dataMetrics.LastTransferInterval)
				: string.Empty;

			_lastTransferAge.text = _dataMetrics.LastTransferAge.HasValue
				? string.Format(DebuggingConstants.SECONDS_FORMAT, _dataMetrics.LastTransferAge)
				: string.Empty;

			_sensorFramesPerSecond.text = string.Format(
				DebuggingConstants.FRAMES_PER_SECOND_FORMAT,
				_dataMetrics.SensorFramesLastSecond);

			_unityToDeviceOffset.text = _dataMetrics.UnityToDeviceOffset.HasValue
				? string.Format(DebuggingConstants.SECONDS_FORMAT, _dataMetrics.UnityToDeviceOffset)
				: string.Empty;
		}

		/// <summary>
		/// Updates a finite capture of sensor frames per render frames from DataMetrics where the history is
		/// equal to <seealso cref="_sensorFramesPerRenderFrameCaptureLimit"/>.
		/// </summary>
		private void UpdateSensorFramesPerRenderFrameCapture()
		{
			_capturedSensorFramesPerRenderFrame[_captureIndex] = _dataMetrics.SensorFramesLastUpdate;
			_captureIndex = (_captureIndex + 1) % _capturedSensorFramesPerRenderFrame.Length;
		}

		/// <summary>
		/// Returns a radar string for sensor frames per render frame based on whether or not we are capturing
		/// or not.
		/// </summary>
		/// <returns></returns>
		private string GetRenderFrameCapturesString()
		{
			if (_capturedSensorFramesPerRenderFrame == null ||
			   _capturedSensorFramesPerRenderFrame.Length == 0)
			{
				return _dataMetrics.RenderFramesLastSecond.ToString();
			}

			_captureStringBuilder.Length = 0;
			for (var i = 0; i < _capturedSensorFramesPerRenderFrame.Length; i++)
			{
				_captureStringBuilder.Append(_capturedSensorFramesPerRenderFrame[i]);

				if (i != _capturedSensorFramesPerRenderFrame.Length - 1)
				{
					_captureStringBuilder.Append(EMPTY_SPACE);
				}
			}

			return _captureStringBuilder.ToString();
		}

		#if UNITY_EDITOR

		private void Reset()
		{
			_sensorFramesPerRenderFrameCaptureLimit = 10;
		}

		#endif
	}
}
