using UnityEngine;

namespace neuroears.allen.utils
{
    public class CameraPermissionCheckMono : MonoBehaviour
    {
        private void Awake()
        {
            CameraPermissionChecker.CheckCameraPermission(null, true);
        }
    }

}
