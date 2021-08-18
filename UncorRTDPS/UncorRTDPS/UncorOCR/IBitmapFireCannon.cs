using System.Drawing;

namespace UncorRTDPS.UncorOCR
{
    public interface IBitmapFireCannon
    {
        void RegisterBitmapListener(IBitmapListener listener);
        void UnregisterBitmapListener(IBitmapListener listener);
        void FireBitmapToAllListeners(Bitmap bmp);
    }
}
