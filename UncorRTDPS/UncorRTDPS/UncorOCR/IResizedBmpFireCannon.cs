using System.Drawing;

namespace UncorRTDPS.UncorOCR
{
    public interface IResizedBmpFireCannon
    {
        void RegisterResizedBmpListener(IResizedBmpListener listener);
        void UnregisterResizedBmpListener(IResizedBmpListener listener);
        void NotifyAllResizedBmpListeners(Bitmap bmp);
    }
}
