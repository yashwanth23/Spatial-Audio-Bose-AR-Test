using System;
using UnityEngine;

namespace Bose.Wearable.Examples
{
	/// <summary>
	/// Processes single-touch drags. Subsequent touches are ignored for the duration of the drag.
	/// </summary>
	internal sealed class MouseButtonRecognizer : Singleton<MouseButtonRecognizer>
	{
		/// <summary>
		/// Called when a new touch begins.
		/// </summary>
		public event Action<Vector3> DragBegan;

		/// <summary>
		/// Continuously called when a tracked touch moves.
		/// </summary>
		public event Action<Vector3> DragMoved;

		/// <summary>
		/// Called when a touch ends.
		/// </summary>
		public event Action<Vector3> DragEnded;

		private bool _mouseDown;
		private Vector3 _lastPosition;

		protected override void Awake()
		{
			_mouseDown = false;

			base.Awake();
		}

		private void Update()
		{
			if (_mouseDown)
			{
				// Check for drag release
				if (Input.GetMouseButtonUp(0))
				{
					// Touch ended; cancel tracking
					_mouseDown = false;
					_lastPosition = Vector3.zero;
					if (DragEnded != null)
					{
						DragEnded.Invoke(Input.mousePosition);
					}
				}
				else if (_lastPosition != Input.mousePosition)
				{
					// Touch still occuring and moved
					if (DragMoved != null)
					{
						DragMoved.Invoke(_lastPosition - Input.mousePosition);
					}

					_lastPosition = Input.mousePosition;
				}
			}
			else if (Input.GetMouseButtonDown(0))
			{
				// Otherwise, start tracking the first touch to begin
				_lastPosition = Input.mousePosition;
				_mouseDown = true;

				if (DragBegan != null)
				{
					DragBegan.Invoke(_lastPosition);
				}
			}
		}
	}
}
