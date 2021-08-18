using System.Drawing;
using System.Drawing.Imaging;
using UncorRTDPS.FastBitmap;
using UncorRTDPS.Screencap;
using UncorRTDPS.UncorOCR.BitmapFunctions;
using UncorRTDPS.UncorOCR.Transformations;

namespace UncorRTDPS.UncorOCR.Preparators
{
    public class LeftBorderOptimizer_DamageIsMostLeft
    {
        public SelectedArea OptimizeLeftBorderOfCaptureArea(Bitmap originalImage, OCRSettings ocrSettings, SelectedArea selectedArea)
        {
            Transformations_DamageOCR_Target transformations_DamageOCR_Target = new Transformations_DamageOCR_Target();

            float resizeScale = ocrSettings.OCR_imageScaling;
            float brightnessBarrier = ocrSettings.OCR_brightnessBarrier;

            Bitmap bmpResized = new Bitmap((int)(originalImage.Width * resizeScale), (int)(originalImage.Height * resizeScale), PixelFormat.Format32bppArgb);
            FastBitmap.FastBitmap bmpResizedFast = new FastBitmap.FastBitmap(bmpResized);
            Graphics bmpResizedGraphics = Graphics.FromImage(bmpResized);
            bmpResizedGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Transformations_Size.ResizeImage(originalImage, bmpResizedFast.Width, bmpResizedFast.Height, bmpResizedGraphics);

            bmpResizedFast.Lock();

            BitmapTransformations.MakeBlackWhite(bmpResizedFast, brightnessBarrier);
            transformations_DamageOCR_Target.RemoveGarbagedRows_FullWidthCheck(bmpResizedFast);

            //check if vertically black exists then its some garbage
            int widthOfBlackGarbage = 0;
            while (Util_FastBitmap.CheckIfVerticallyBlackExists(bmpResizedFast, widthOfBlackGarbage, 0, bmpResizedFast.Height, 1))
            {
                widthOfBlackGarbage += 1;

                if (widthOfBlackGarbage >= bmpResizedFast.Width)
                    return null;
            }

            int posOfWhiteSpaceBeforeDamage = widthOfBlackGarbage;
            while (!Util_FastBitmap.CheckIfVerticallyBlackExists(bmpResizedFast, posOfWhiteSpaceBeforeDamage, 0, bmpResizedFast.Height, 1))
            {
                posOfWhiteSpaceBeforeDamage += 1;
                if (posOfWhiteSpaceBeforeDamage >= bmpResizedFast.Width)
                    return null;
            }

            bmpResizedFast.Unlock();


            int scaledToNormal = (int)(posOfWhiteSpaceBeforeDamage / resizeScale);
            SelectedArea newSelectedArea = new SelectedArea(selectedArea.X_topLeft + scaledToNormal - 2, selectedArea.Y_topLeft, selectedArea.X_botRight, selectedArea.Y_botRight);
            return newSelectedArea;
        }
    }
}
