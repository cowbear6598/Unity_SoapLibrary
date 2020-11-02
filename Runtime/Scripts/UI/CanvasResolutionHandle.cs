using UnityEngine;
using UnityEngine.UI;

namespace Soap.UI
{
    public class CanvasResolutionHandle : MonoBehaviour
    {
        private CanvasScaler canvasScaler;

        private void Awake()
        {
            canvasScaler = GetComponent<CanvasScaler>();
        
            SetScaler();    
        }

        public void SetScaler()
        {
            float screenWidthScale = Screen.width / canvasScaler.referenceResolution.x;
            float screenHeightScale = Screen.height / canvasScaler.referenceResolution.y;

            canvasScaler.matchWidthOrHeight = screenWidthScale > screenHeightScale ? 1 : 0;
        }
    }

}