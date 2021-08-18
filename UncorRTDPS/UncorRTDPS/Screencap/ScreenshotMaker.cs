using System.Drawing;

namespace UncorRTDPS.Screencap
{
    public class ScreenshotMaker
    {
        public Bitmap MakeScreenshot(int X, int Y, int width, int height)
        {
            if (width < 1 || height < 1)
                return null;
            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, width, height);
            g.CopyFromScreen(X, Y, 0, 0, new Size(width, height));
            g.Dispose();
            return bmp;
        }
    }
}
