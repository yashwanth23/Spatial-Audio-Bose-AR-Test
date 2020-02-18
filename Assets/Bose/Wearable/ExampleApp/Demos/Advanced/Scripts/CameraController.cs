using UnityEngine;

namespace Bose.Wearable.Examples
{
	/// <summary>
	/// Provides a very simple touch camera controller that orbits around the origin at a fixed distance and elevation.
	/// </summary>
	public class CameraController : MonoBehaviour
	{
		#pragma warning disable 0649
		[Header("Scene References")]
		[SerializeField]
		private InfoUIPanel _infoUIPanel;
		#pragma warning restore 0649

		/// <summary>
		/// The distance from the origin.
		/// </summary>
		[Header("Camera Settings")]
		[SerializeField]
		protected float _distance;

		/// <summary>
		/// Elevation from horizontal in degrees. Positive values look down on the origin from above.
		/// </summary>
		[SerializeField]
		protected float _elevation;

		/// <summary>
		/// Control-Display ratio of the controller. The camera will rotate by this many degrees when swiping completely
		/// across the screen.
		/// </summary>
		[SerializeField]
		protected float _cdRatio;

		private float _azimuth;

		private void Start()
		{
			_azimuth = 0.0f;
		}

		private void OnEnable()
		{
			MouseButtonRecognizer.Instance.DragMoved += OnDragMoved;
		}

		private void OnDisable()
		{
			if (MouseButtonRecognizer.Instance != null)
			{
				MouseButtonRecognizer.Instance.DragMoved -= OnDragMoved;
			}
		}

		private void OnDragMoved(Vector3 mouseDelta)
		{
			if (_infoUIPanel.IsVisible)
			{
				return;
			}

			_azimuth += _cdRatio * -mouseDelta.x / Screen.width;
		}

		private void LateUpdate()
		{
			transform.rotation = Quaternion.Euler(_elevation, _azimuth, 0.0f);
			transform.position = -transform.forward * _distance;
		}
	}
}
