using UnityEditor;
using UnityEngine;

namespace Bose.Wearable.Debugging.Editor
{
	[CustomEditor(typeof(AutoConfigureHorizontalGridLayout))]
	internal sealed class AutoConfigureHorizontalGridLayoutInspector : UnityEditor.Editor
	{
		private const string BUTTON_TEXT = "Update Cell Size";

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var gridConfigurer = (AutoConfigureHorizontalGridLayout)target;
			if (GUILayout.Button(BUTTON_TEXT))
			{
				gridConfigurer.UpdateCellSize();
			}
		}
	}
}
