using System.Text.RegularExpressions;

namespace Bose.Wearable.Extensions
{
	/// <summary>
	/// Internal extension methods for <see cref="string"/>
	/// </summary>
	internal static class StringExtensions
	{
		/// <summary>
		/// Converts a CamelCase/UICamelCase string into a more human readable 'Camel Case' or
		/// `UI Camel Case' string.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string Nicify(this string value)
		{
			return Regex.Replace(Regex.Replace(value,@"(\P{Ll})(\P{Ll}\p{Ll})","$1 $2"),@"(\p{Ll})(\P{Ll})","$1 $2");
		}
	}
}
