using System.Drawing;

namespace UncorRTDPS.UncorOCR.Transformations
{
    public static class Transformations_Size
    {
        /// <summary>
        /// Returns new bitmap. No "fill background" before resize, could be "bugged".
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap imgToResize, int newWidth, int newHeight)
        {
            Bitmap b = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            }
            return b;
        }

        public static SolidBrush bBlack = new SolidBrush(Color.Black);

        /// <summary>
        /// Additionally draws black rect before img
        /// </summary>
        /// <param name="imgSource"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <param name="graphicsImgOut"></param>
        public static void ResizeImage(Bitmap imgSource, int newWidth, int newHeight, Graphics graphicsImgOut)
        {
            graphicsImgOut.FillRectangle(bBlack, 0, 0, newWidth, newHeight);
            graphicsImgOut.DrawImage(imgSource, 0, 0, newWidth, newHeight);
        }
    }
}
