
using System.Drawing;

namespace UncorRTDPS.UncorOCR.BufferedBitmaps
{
    public class BufferedBmps_WidthScaleOnly : BufferedBmps
    {
        private int minWidth, height;
        private int stepWidth;

        public void CreateBmps(int minWidth, int height, int stepWidth, int count)
        {
            Dispose();
            this.minWidth = minWidth;
            this.height = height;
            this.stepWidth = stepWidth;

            int currWidth;
            for (int i = 0; i < count; i++)
            {
                currWidth = minWidth + (stepWidth * i);

                Bitmap bmp = new Bitmap(currWidth, height);
                addBmpPack(new BufferedBitmapPack(bmp, Graphics.FromImage(bmp), new Size_Uncor(currWidth, height)));
            }
        }

        public BufferedBitmapPack GetFittingBitmap(int w, int h)
        {
            if (h > height || h < 1)
            {
                return null;
            }
            int iW = ((w - minWidth) / stepWidth) + 1;

            return GetBmpPack(iW);
        }

        public int GetIndexOfFittingBitmap(int w, int h)
        {
            if (h > height || w < 1 || h < 1)
            {
                return -1;
            }

            int iW = ((w - minWidth) / stepWidth) + 1;

            if (iW >= bmpPacks.Count || iW < 0)
            {
                return -1;
            }
            return iW;
        }
    }
}
