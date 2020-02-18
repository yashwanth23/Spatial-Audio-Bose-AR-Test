using UnityEditor;
using UnityEngine.EventSystems;

namespace Bose.Wearable.Connection.Editor
{
	[CustomEditor(typeof(WearableConnectUIPanel))]
	internal sealed class WearableConnectUIPanelInspector : UnityEditor.Editor
	{
		private EventSystem _eventSystem;

		private const string EVENT_SYSTEM_NOT_FOUND_WARNING = "EventSystem not found. Please create one or input will not "+
															"be detected.";

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			WarnIfNoEventSystemPresent();
		}

		private void WarnIfNoEventSystemPresent()
		{
			if (_eventSystem == null)
			{
				_eventSystem = FindObjectOfType<EventSystem>();

				if (_eventSystem == null)
				{
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox(EVENT_SYSTEM_NOT_FOUND_WARNING, MessageType.Warning);
					EditorGUILayout.Space();
				}
			}
		}
	}
}
