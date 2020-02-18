using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// <see cref="DebugUIControlBase"/> is an abstract base class for all debug UI controls.
	/// </summary>
	internal abstract class DebugUIControlBase : MonoBehaviour
	{
		[Header("Style Data Refs")]
		[SerializeField]
		protected WearableUIColorPalette _colorPalette;

		protected WearableControl _wearableControl;

		protected virtual void Start()
		{
			_wearableControl = WearableControl.Instance;
		}

		/// <summary>
		/// Returns true if this control's value conflicts in some way from its intended Requirement
		/// value, otherwise returns false.
		/// </summary>
		/// <returns></returns>
		protected virtual bool IsConflicting()
		{
			return false;
		}
	}
}
