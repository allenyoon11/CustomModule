using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Android;

namespace neuroears.allen.utils
{
    public class CameraPermissionChecker
    {
        public static void CheckCameraPermission(Action cb, bool forcePopupTrigger = false)
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                if (forcePopupTrigger)
                {
                    var _ = WebCamTexture.devices;
                }

                Permission.RequestUserPermission(Permission.Camera);
                WaitForPermission(cb).Forget();
            }
            else
            {
                cb?.Invoke();
            }
#else
            cb?.Invoke();
#endif
        }

        private static async UniTaskVoid WaitForPermission(Action cb)
        {
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                await UniTask.Yield();
            }
            cb?.Invoke();
        }
    }

}
