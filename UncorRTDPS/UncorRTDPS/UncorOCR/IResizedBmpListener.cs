using System.Drawing;

namespace UncorRTDPS.UncorOCR
{
    public interface IResizedBmpListener
    {
        void FireBmp(Bitmap bmp);
    }
}
