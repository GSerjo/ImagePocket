using System;
using MonoTouch.Photos;

namespace Core.Security
{
    public class PrivacyRequestor
    {
        public static bool PhotoAccess()
        {
            if (PHPhotoLibrary.AuthorizationStatus == PHAuthorizationStatus.Authorized)
            {
                return true;
            }
            var result = PHAuthorizationStatus.NotDetermined;
            PHPhotoLibrary.RequestAuthorization(x => result = x);
            return result == PHAuthorizationStatus.Authorized;
            //            var library = new ALAssetsLibrary();
            //            library.Enumerate(ALAssetsGroupType.All, delegate { }, delegate { });
            //            return ALAssetsLibrary.AuthorizationStatus == ALAuthorizationStatus.Authorized;
        }
    }
}
