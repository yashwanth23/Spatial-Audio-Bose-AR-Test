using System;

namespace Bose.Wearable
{
	public struct FirmwareUpdateInformation
	{
		public BoseUpdateIcon icon;
		public string title;
		public string message;
		public FirmwareUpdateAlertOption[] options;
	}
}