using System.Drawing;

namespace UncorRTDPS.FastBitmap
{
    public class BitmapTransformations
    {
        public static int BlackARGB = Color.Black.ToArgb();
        public static int WhiteARGB = Color.White.ToArgb();

        //must be locked
        public static void MakeBlackWhite(FastBitmap fastBmp, float brightness)
        {
            for (int y = 0; y < fastBmp.Height; y++)
            {
                for (int x = 0; x < fastBmp.Width; x++)
                {
                    if (fastBmp.GetPixel(x, y).GetBrightness() > brightness)
                        fastBmp.SetPixel(x, y, BlackARGB);
                    else
                        fastBmp.SetPixel(x, y, WhiteARGB);
                }
            }
        }

        //must be locked
        public static void MakeBlackWhite(FastBitmap fastBmp, float brigthness, bool reversedBlack)
        {
            Color c1, c2;
            if (reversedBlack)
            {
                c1 = Color.White;
                c2 = Color.Black;
            }
            else
            {
                c1 = Color.Black;
                c2 = Color.White;
            }
            for (int y = 0; y < fastBmp.Height; y++)
            {
                for (int x = 0; x < fastBmp.Width; x++)
                {
                    if (fastBmp.GetPixel(x, y).GetBrightness() > brigthness)
                        fastBmp.SetPixel(x, y, c1);
                    else
                        fastBmp.SetPixel(x, y, c2);
                }
            }
        }
    }
}
