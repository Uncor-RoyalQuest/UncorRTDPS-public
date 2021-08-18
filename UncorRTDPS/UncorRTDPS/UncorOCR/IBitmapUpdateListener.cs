using System.Drawing;

namespace UncorRTDPS.UncorOCR
{
    public interface IBitmapUpdateListener
    {
        public int FireBitmap(Bitmap bmp);
    }
}
