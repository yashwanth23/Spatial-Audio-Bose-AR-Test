using System;

namespace Bose.Wearable
{
	public static class OSServiceExtensions
	{
		/// <summary>
		/// Returns the appropriate locale key for <param name="service"></param>
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public static string GetLocaleKey(this OSService service)
		{
			switch (service)
			{
				case OSService.Bluetooth:
					return LocaleConstants.BOSE_AR_UNITY_SDK_BLUETOOTH;
				case OSService.LocationServices:
					return LocaleConstants.BOSE_AR_UNITY_SDK_LOCATION;
				default:
					throw new ArgumentOutOfRangeException("service", service, null);
			}
		}
	}
}
