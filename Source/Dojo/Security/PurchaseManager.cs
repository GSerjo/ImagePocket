using System;
using StoreKit;
using Xamarin.InAppPurchase;

namespace Dojo
{
    public sealed class PurchaseManager
    {
        private const string AppStoreProductId = "";
        private static readonly PurchaseManager _instance = new PurchaseManager();
        private readonly InAppPurchaseManager _purchaseManager = new InAppPurchaseManager();
        private bool _productPurchased;

        private PurchaseManager()
        {
            _purchaseManager.SimulateiTunesAppStore = false;
            _purchaseManager.PublicKey = "5266B284A4D747FFBEAA01F4081A29E2";
            _purchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.UserDefaults;
            QueryInventory();

            _purchaseManager.InAppProductPurchaseFailed += OnInAppProductPurchaseFailed;
            _purchaseManager.InAppPurchasesRestored += OnInAppPurchasesRestored;
            _purchaseManager.InAppProductPurchased += OnInAppProductPurchased;
        }

        public static PurchaseManager Instance
        {
            get { return _instance; }
        }

        public bool ProductPurchased
        {
            get
            {
                try
                {
                    if (_productPurchased)
                    {
                        return _productPurchased;
                    }
                    _productPurchased = _purchaseManager.ProductPurchased(AppStoreProductId);
                    return _productPurchased;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        public void Buy()
        {
            try
            {
                Console.WriteLine("Buy");
                _purchaseManager.BuyProduct(AppStoreProductId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Restore()
        {
            try
            {
                Console.WriteLine("Restore");
                _purchaseManager.RestorePreviousPurchases();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void OnInAppProductPurchaseFailed(SKPaymentTransaction transaction, InAppProduct product)
        {
            Console.WriteLine("OnInAppProductPurchaseFailed");
        }

        private void OnInAppProductPurchased(SKPaymentTransaction transaction, InAppProduct product)
        {
            Console.WriteLine("OnInAppProductPurchased");
        }

        private void OnInAppPurchasesRestored(int count)
        {
            Console.WriteLine("OnInAppPurchasesRestored");
        }

        private void QueryInventory()
        {
            try
            {
                _purchaseManager.QueryInventory(AppStoreProductId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
