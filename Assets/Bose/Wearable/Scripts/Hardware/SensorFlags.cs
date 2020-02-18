using System;

namespace Bose.Wearable
{
	/// <summary>
	/// A bitfield that represents which sensors are available on a device.
	/// </summary>
	[Flags]
	[Serializable]
	public enum SensorFlags
	{
		None = 0,
		Accelerometer = 1,
		Gyroscope = 2,
		RotationNineDof = 4,
		RotationSixDof = 8
	}
}
