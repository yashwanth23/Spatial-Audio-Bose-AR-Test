using UnityEngine;

namespace Bose.Wearable.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="RectTransform"/>.
	/// </summary>
	internal static class RectTransformExtensions
	{
		private static readonly Vector2 POINT_LEFT_MIDDLE = new Vector2(0f, 0.5f);
		private static readonly Vector2 POINT_CENTER_MIDDLE = new Vector2(0.5f, 0.5f);
		private static readonly Vector2 POINT_RIGHT_MIDDLE = new Vector2(1f, 0.5f);

		/// <summary>
		/// Sets the anchor to the left-middle of its parent <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the anchor.</param>
		public static void SetAnchorLeftMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetAnchorPoint(rectTransform, POINT_LEFT_MIDDLE, worldPositionStays);
		}

		/// <summary>
		/// Sets the pivot to the left, middle of the <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the pivot.</param>
		public static void SetPivotLeftMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetPivotPoint(rectTransform, POINT_LEFT_MIDDLE, worldPositionStays);
		}

		/// <summary>
		/// Sets the anchor to the center of its parent <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the anchor.</param>
		public static void SetAnchorCenterMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetAnchorPoint(rectTransform, POINT_CENTER_MIDDLE, worldPositionStays);
		}

		/// <summary>
		/// Sets the pivot to the center, middle of the <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the pivot.</param>
		public static void SetPivotCenterMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetPivotPoint(rectTransform, POINT_CENTER_MIDDLE, worldPositionStays);
		}

		/// <summary>
		/// Sets the anchor to the right-middle of its parent <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the anchor.</param>
		public static void SetAnchorRightMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetAnchorPoint(rectTransform, POINT_RIGHT_MIDDLE, worldPositionStays);
		}

		/// <summary>
		/// Sets the pivot to the right, middle of the <see cref="RectTransform"/>.
		/// </summary>
		/// <param name="rectTransform"></param>
		/// <param name="worldPositionStays">Keep or reset the anchorPosition after moving the pivot.</param>
		public static void SetPivotRightMiddle(this RectTransform rectTransform, bool worldPositionStays = true)
		{
			SetPivotPoint(rectTransform, POINT_RIGHT_MIDDLE, worldPositionStays);
		}

		public static void SetAnchorPoint(RectTransform rectTransform, Vector2 point, bool worldPositionStays = true)
		{
			var position = rectTransform.position;

			rectTransform.anchorMin = point;
			rectTransform.anchorMax = point;

			if (worldPositionStays)
			{
				rectTransform.position = position;
			}
			else
			{
				rectTransform.ResetAnchorPosition();
			}
		}

		public static void SetPivotPoint(RectTransform rectTransform, Vector2 point, bool worldPositionStays = true)
		{
			var pivotDelta = point - rectTransform.pivot;

			rectTransform.pivot = point;

			if (worldPositionStays)
			{
				rectTransform.anchoredPosition += Vector2.Scale(rectTransform.sizeDelta, pivotDelta);
			}
			else
			{
				rectTransform.ResetAnchorPosition();
			}
		}

		public static void ResetAnchorPosition(this RectTransform rectTransform)
		{
			rectTransform.anchoredPosition = Vector2.zero;
		}
	}
}
