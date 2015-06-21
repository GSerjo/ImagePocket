using Xamarin.InAppPurchase;

namespace Dojo
{
	public sealed class PurchaseManager
	{
		private static readonly PurchaseManager _instance = new PurchaseManager();
		private readonly InAppPurchaseManager _purchaseManager = new InAppPurchaseManager();

		private PurchaseManager ()
		{
		}

		public static PurchaseManager Instance
		{
			get { return _instance; }
		}
	}
}