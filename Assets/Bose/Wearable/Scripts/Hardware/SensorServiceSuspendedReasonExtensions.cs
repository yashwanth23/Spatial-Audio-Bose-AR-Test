using System;

namespace Bose.Wearable
{
	public static class SensorServiceSuspendedReasonExtensions
	{
		/// <summary>
		/// Returns the appropriate locale key for <param name="reason"></param>
		/// </summary>
		/// <param name="reason"></param>
		/// <returns></returns>
		public static string GetLocaleKey(this SensorServiceSuspendedReason reason)
		{
			switch (reason)
			{
				case SensorServiceSuspendedReason.UnknownReason:
					return LocaleConstants.BOSE_AR_UNITY_SDK_SENSOR_SERVICE_SUSPENSION_REASON_UNKNOWN_MESSAGE;
				case SensorServiceSuspendedReason.VoiceAssistantInUse:
					return LocaleConstants.BOSE_AR_UNITY_SDK_SENSOR_SERVICE_SUSPENSION_REASON_VOICE_ASSISTANT_MESSAGE;
				case SensorServiceSuspendedReason.MultipointConnectionActive:
					return LocaleConstants.BOSE_AR_UNITY_SDK_SENSOR_SERVICE_SUSPENSION_REASON_MULTI_POINT_MESSAGE;
				case SensorServiceSuspendedReason.MusicSharingActive:
					return LocaleConstants.BOSE_AR_UNITY_SDK_SENSOR_SERVICE_SUSPENSION_REASON_MUSIC_SHARING_MESSAGE;
				case SensorServiceSuspendedReason.OTAFirmwareUpdate:
					return LocaleConstants.BOSE_AR_UNITY_SDK_SENSOR_SERVICE_SUSPENSION_REASON_OTA_FIRMWARE_UPDATE;
				default:
					throw new ArgumentOutOfRangeException("reason", reason, null);
			}
		}
	}
}
