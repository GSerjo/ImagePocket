using System;
using Domain;
using StoreKit;
using UIKit;
using Xamarin.InAppPurchase;

namespace Dojo
{
    public sealed class PurchaseManager
    {
        private const string AppStoreProductId = "org.nelibur.ImagePocket";
        private const int MaxTagCount = 2;
        private static readonly PurchaseManager _instance = new PurchaseManager();
        private readonly InAppPurchaseManager _purchaseManager = new InAppPurchaseManager();
        private readonly TagCache _tagCache = TagCache.Instance;
        private bool _productPurchased;

        private PurchaseManager()
        {
            _purchaseManager.SimulateiTunesAppStore = true;

//			_purchaseManager.SimulateiTunesAppStore = false;

            _purchaseManager.PublicKey = "5266B284A4D747FFBEAA01F4081A29E2";
            _purchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.LocalFile;
            _purchaseManager.PersistenceFilename = "5C3419ECCBDB4257894C8460686C92FE";
            _purchaseManager.RestoreProducts();

            QueryInventory();

            _purchaseManager.InAppProductPurchaseFailed += OnInAppProductPurchaseFailed;
            _purchaseManager.InAppPurchasesRestored += OnInAppPurchasesRestored;
            _purchaseManager.InAppProductPurchased += OnInAppProductPurchased;
        }

        public static PurchaseManager Instance
        {
            get { return _instance; }
        }

        public bool RequirePurchase
        {
            get
            {
                if (_tagCache.UserTagCount < MaxTagCount)
                {
                    return false;
                }
                return !ProductPurchased;
            }
        }

        private bool ProductPurchased
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
                if (_purchaseManager.CanMakePayments == false)
                {
                    using (var alert = new UIAlertView("Warning", "Sorry but you cannot make purchases from the In App Billing store. Please try again later.", null, "OK", null))
                    {
                        alert.Show();
                    }
                    return;
                }
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
