using System;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the status of the intent validation procedure.
	/// </summary>
	[Serializable]
	public enum IntentValidationStatus
	{
		/// <summary>
		/// Intent validation is not active.
		/// </summary>
		Disabled,
		
		/// <summary>
		/// The set intent profile has not been checked for validity.
		/// </summary>
		Unknown,
		
		/// <summary>
		/// The set intent profile is currently being validated.
		/// </summary>
		Validating,
		
		/// <summary>
		/// The set intent profile was successfully validated.
		/// </summary>
		Success,
		
		/// <summary>
		/// The set intent profile was deemed invalid.
		/// </summary>
		Failure
	}
}
