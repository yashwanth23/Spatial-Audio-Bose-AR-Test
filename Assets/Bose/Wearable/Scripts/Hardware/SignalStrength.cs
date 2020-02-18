using System;

namespace Bose.Wearable
{
	/// <summary>
	/// Indicates the observed signal strength of a device suitable for display in the user interface.
	/// </summary>
	[Serializable]
	public enum SignalStrength
	{
		Weak,
		Moderate,
		Strong,
		Full
	}
}
