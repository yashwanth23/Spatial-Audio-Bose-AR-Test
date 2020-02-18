using UnityEngine;
using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="AutoConfigureHorizontalGridLayout"/> is used to configure a <see cref="GridLayoutGroup"/>
	/// keeps a consistent column count despite whether or not the screen is held in portrait or landscape view.
	/// </summary>
	internal sealed class AutoConfigureHorizontalGridLayout : MonoBehaviour
	{
		#pragma warning disable 0649

		[Header("UI Refs")]
		[SerializeField]
		private GridLayoutGroup _gridLayoutGroup;

		[SerializeField]
		private VerticalLayoutGroup _parentLayoutGroup;

		private RectTransform _parentLayoutGroupRectTransform;

		[Header("Controls")]
		[Range(1, 10)]
		[SerializeField]
		private int _numberOfColumns;

		[SerializeField]
		[Range(0, 1000)]
		private float _height;

		#pragma warning restore 0649

		private void Awake()
		{
			_gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			_parentLayoutGroupRectTransform = _parentLayoutGroup.GetComponent<RectTransform>();
		}

		private void Update()
		{
			UpdateCellSize();
		}

		internal void UpdateCellSize()
		{
			var targetWidth = _parentLayoutGroupRectTransform.rect.width -
			                  _parentLayoutGroup.padding.left -
			                  _parentLayoutGroup.padding.right;
			var cellWidth = targetWidth / _numberOfColumns;
			if (!Mathf.Approximately(_gridLayoutGroup.cellSize.x, cellWidth) ||
			    !Mathf.Approximately(_gridLayoutGroup.cellSize.y, _height) ||
			    _gridLayoutGroup.constraintCount != _numberOfColumns)
			{
				_gridLayoutGroup.constraintCount = _numberOfColumns;
				_gridLayoutGroup.cellSize = new Vector2(cellWidth, _height);
			}
		}

		#if UNITY_EDITOR

		private void Reset()
		{
			_height = 100f;
			_numberOfColumns = 2;
		}

		#endif
	}
}
