
using UnityEngine.SceneManagement;

namespace Bose.Wearable.Examples
{
	/// <summary>
	/// AppRoot is the common entry point for the app and facilitates start-up behavior.
	/// </summary>
	internal sealed class AppRoot : Singleton<AppRoot>
	{
		private void Start()
		{
			SceneManager.LoadScene(WearableDemoConstants.MAIN_MENU_SCENE);
		}
	}
}
