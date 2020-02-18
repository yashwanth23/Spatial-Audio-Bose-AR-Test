using UnityEngine.UI;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="NonDrawingGraphic"/> is an invisible graphic element that prevents click-through to other
	/// UI elements.
	/// </summary>
	internal sealed class NonDrawingGraphic : Graphic
	{
		public override void SetMaterialDirty() { }
		public override void SetVerticesDirty() { }

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
