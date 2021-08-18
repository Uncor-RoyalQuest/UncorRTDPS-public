using System.Drawing;
using System.IO;
using Tesseract;

namespace UncorRTDPS.UncorOCR
{
    public class EngineOCR
    {
        protected TesseractEngine tesEngine;

        protected int removeGarbagedRows_LastGarbageSeparator_Starting;
        protected int removeGarbagedRows_LastGarbageSeparator_Ending;
        public readonly static int BlackARGB = Color.Black.ToArgb();
        public readonly static int WhiteARGB = Color.White.ToArgb();

        public void InitEngine(RTDPS_Settings.UncorRTDPS_StaticSettings.Languages l)
        {
            if (tesEngine != null)
                DisposeEngine();

            string lang = "eng";
            switch (l)
            {
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian:
                    lang = "rus";
                    break;
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English:
                    lang = "eng";
                    break;

            }
            tesEngine = new TesseractEngine(Path.GetFullPath(Path.Combine(RTDPS_Settings.UncorRTDPS_StaticSettings.ResourcesPath, @"src\tessdata")), lang, EngineMode.Default);
            //tesEngine.DefaultPageSegMode = PageSegMode.SingleWord;
            tesEngine.DefaultPageSegMode = PageSegMode.Auto;
        }

        public void DisposeEngine()
        {
            if (tesEngine == null)
                return;
            tesEngine.Dispose();
            tesEngine = null;
        }


        /// <summary>
        /// With pix converter
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public string ParseStringFromBitmap(Bitmap bmp)
        {
            Pix pixImg = PixConverter.ToPix(bmp);
            Page page = tesEngine.Process(pixImg);
            string res = page.GetText();
            page.Dispose();
            pixImg.Dispose();
            return res;
        }

        public string ParseStringFromBitmap_NoPix(Bitmap bmp)
        {
            Page page = tesEngine.Process(bmp);
            string res = page.GetText();
            page.Dispose();
            return res;
        }

    }
}
