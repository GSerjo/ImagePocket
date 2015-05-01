using System;
using MonoTouch.AssetsLibrary;

namespace Core.Security
{
    public class PrivacyRequestor
    {
        public static bool PhotoAccess()
        {
            var library = new ALAssetsLibrary();
            library.Enumerate(ALAssetsGroupType.All, delegate { }, delegate { });
            return ALAssetsLibrary.AuthorizationStatus == ALAuthorizationStatus.Authorized;
        }
    }
}
