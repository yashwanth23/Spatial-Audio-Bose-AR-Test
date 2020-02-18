using System.Runtime.InteropServices;

namespace Bose.Wearable
{
	/// <summary>
	/// Represents the device information available during the connection process.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct DeviceConnectionInfo
	{
		/// <summary>
		/// The ProductId of the device.
		/// </summary>
		internal ProductId productId;

		/// <summary>
		/// The VariantId of the device.
		/// </summary>
		internal byte variantId;

		/// <summary>
		/// Returns the <see cref="ProductType"/> of the device.
		/// </summary>
		/// <returns></returns>
		public ProductType GetProductType()
		{
			return WearableTools.GetProductType(productId);
		}

		/// <summary>
		/// Returns the Product <see cref="VariantType"/> of the device.
		/// </summary>
		/// <returns></returns>
		public VariantType GetVariantType()
		{
			return WearableTools.GetVariantType(GetProductType(), variantId);
		}
	}
}
