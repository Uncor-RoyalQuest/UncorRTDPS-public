using System;
using System.Drawing;

namespace UncorRTDPS.UncorOCR.BufferedBitmaps
{
    public class BufferedBitmapPack : IDisposable
    {
        public Bitmap bitmap;
        public Graphics bitmapGraphics;
        public Size_Uncor bitmapSize;

        public BufferedBitmapPack(Bitmap bmp, Graphics g, Size_Uncor size)
        {
            this.bitmap = bmp;
            this.bitmapGraphics = g;
            this.bitmapSize = size;
        }

        public void Dispose()
        {
            if (bitmapGraphics != null)
            {
                bitmapGraphics.Dispose();
                bitmapGraphics = null;
            }

            if (bitmapSize!=null)
            {
                bitmapSize = null;
            }

            if (bitmap!=null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }
    }
}
