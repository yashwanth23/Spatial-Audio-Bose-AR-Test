using System;

namespace Bose.Wearable
{
	public static class OSPermissionExtensions
	{
		/// <summary>
		/// Returns the appropriate locale key for <param name="permission"></param>
		/// </summary>
		/// <param name="permission"></param>
		/// <returns></returns>
		public static string GetLocaleKey(this OSPermission permission)
		{
			switch (permission)
			{
				case OSPermission.Bluetooth:
					return LocaleConstants.BOSE_AR_UNITY_SDK_BLUETOOTH;
				case OSPermission.Location:
					return LocaleConstants.BOSE_AR_UNITY_SDK_LOCATION;
				default:
					throw new ArgumentOutOfRangeException("permission", permission, null);
			}
		}
	}
}
