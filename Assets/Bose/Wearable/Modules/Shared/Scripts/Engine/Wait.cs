using UnityEngine;

namespace Bose.Wearable
{
	/// <summary>
	/// Utility class to cache <see cref="YieldInstruction"/>s for reuse.
	/// </summary>
	public static class Wait
	{
		/// <summary>
		/// Wait until FixedUpdate has been called on all scripts.
		/// </summary>
		public static WaitForFixedUpdate ForFixedUpdate
		{
			get { return _forFixedUpdate; }
		}

		private static WaitForFixedUpdate _forFixedUpdate;

		/// <summary>
		/// Wait until Update has been called on all scripts.
		/// </summary>
		/// <value>For update.</value>
		public static YieldInstruction ForUpdate
		{
			get { return _forUpdate; }
		}

		private static YieldInstruction _forUpdate;

		/// <summary>
		/// Wait until the frame has been rendered, but right before it has been drawn.
		/// </summary>
		public static WaitForEndOfFrame ForEndOfFrame
		{
			get { return _forEndOfFrame; }
		}

		private static WaitForEndOfFrame _forEndOfFrame;

		static Wait()
		{
			_forFixedUpdate = new WaitForFixedUpdate();
			_forUpdate = null;
			_forEndOfFrame = new WaitForEndOfFrame();
		}
	}
}
