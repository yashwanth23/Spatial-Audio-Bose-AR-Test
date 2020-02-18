using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Helper methods for dealing with locale at runtime.
	/// </summary>
	internal static class LocaleTools
	{
		/// <summary>
		/// Returns all <see cref="LocaleData"/> instances in the project.
		/// </summary>
		/// <returns></returns>
		internal static LocaleData[] GetAllLocaleData()
		{
			return Resources.LoadAll<LocaleData>(string.Empty);
		}

		/// <summary>
		/// Returns a fallback <see cref="SystemLanguage"/> in the event. Call in the case we do not support
		/// the preferred one.
		/// </summary>
		/// <returns></returns>
		internal static SystemLanguage GetFallbackSystemLanguage()
		{
			// English is our fallback language for any unsupported system language.
			return SystemLanguage.English;
		}
	}
}
