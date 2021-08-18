using System.Drawing;

namespace UncorRTDPS.UncorOCR.BitmapFunctions
{
    public static class Util_FastBitmap
    {
        public static int BlackARGB = Color.Black.ToArgb();

        /// <summary>
        /// FastBitmap must be locked
        /// </summary>
        /// <param name="bmpFast"></param>
        /// <param name="horizontalPos"></param>
        /// <param name="rowVerticalStart"></param>
        /// <param name="rowVerticalEnd"></param>
        /// <param name="verticalStep"></param>
        /// <returns></returns>
        public static bool CheckIfVerticallyBlackExists(FastBitmap.FastBitmap bmpFast, int horizontalPos, int rowVerticalStart, int rowVerticalEnd, int verticalStep)
        {
            for (int i = rowVerticalStart; i < rowVerticalEnd; i += verticalStep)
            {
                if (bmpFast.GetPixelInt(horizontalPos, i) == BlackARGB)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// FastBitmap must be locked
        /// </summary>
        /// <param name="bmpFast"></param>
        /// <param name="horizStart"></param>
        /// <param name="vertStart"></param>
        /// <param name="vertEnd"></param>
        /// <param name="averageCharLen"></param>
        /// <returns></returns>
        public static int FindStartOfThisWord(FastBitmap.FastBitmap bmpFast, int horizStart, int vertStart, int vertEnd, int averageCharLen)
        {
            int whiteSpaceToIndicateEnd = averageCharLen / 2;

            int whiteSpaceWidth = 0;
            for (int i = horizStart; i > -1; i--)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, vertStart, vertEnd, 1))
                {
                    whiteSpaceWidth += 1;
                    if (whiteSpaceWidth >= whiteSpaceToIndicateEnd)
                    {
                        return i + whiteSpaceToIndicateEnd;
                    }
                }
                else
                {
                    whiteSpaceWidth = 0;
                }
            }

            return -1;
        }


        /// <summary>
        /// FastBitmap must be locked
        /// </summary>
        /// <param name="bmpFast"></param>
        /// <param name="horizStart"></param>
        /// <param name="vertStart"></param>
        /// <param name="vertEnd"></param>
        /// <param name="averageCharLen"></param>
        /// <returns></returns>
        public static int FindEndOfThisWord(FastBitmap.FastBitmap bmpFast, int horizStart, int vertStart, int vertEnd, int averageCharLen)
        {
            int whiteSpaceToIndicateEnd = averageCharLen / 2;

            int whiteSpaceWidth = 0;
            for (int i = horizStart; i < bmpFast.Width; i++)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, vertStart, vertEnd, 1))
                {
                    whiteSpaceWidth += 1;
                    if (whiteSpaceWidth >= whiteSpaceToIndicateEnd)
                    {
                        return i - whiteSpaceToIndicateEnd + 1;
                    }
                }
                else
                {
                    whiteSpaceWidth = 0;
                }
            }

            return -1;
        }
    }
}
