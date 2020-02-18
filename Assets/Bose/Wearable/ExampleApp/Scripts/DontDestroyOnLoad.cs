using UnityEngine;

namespace Bose.Wearable.Examples
{
	/// <summary>
	/// Marks the object as do not destroy
	/// </summary>
	internal sealed class DontDestroyOnLoad : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(this);
		}
	}
}
