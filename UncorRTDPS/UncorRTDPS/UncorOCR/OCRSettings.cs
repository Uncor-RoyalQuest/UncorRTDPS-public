using static UncorRTDPS.RTDPS_Settings.UncorRTDPS_StaticSettings;

namespace UncorRTDPS.UncorOCR
{
    public class OCRSettings
    {

        public float OCR_brightnessBarrier { get; set; }
        private bool isBrightnessBarrierSet = false;

        public float OCR_imageScaling { get; set; }
        private bool isImageScalingSet = false;

        public Languages Lang { get; set; }
        private bool isLangSet = false;

        public OCRSettings() { }

        public OCRSettings(OCRSettings s)
        {
            SetBrightnessBarrier(s.OCR_brightnessBarrier);
            SetImageScaling(s.OCR_imageScaling);
            SetLang(s.Lang);
        }

        public OCRSettings(float barrier, float scaling, Languages lang)
        {
            SetBrightnessBarrier(barrier);
            SetImageScaling(scaling);
            SetLang(lang);
        }

        public void SetBrightnessBarrier(float b)
        {
            OCR_brightnessBarrier = b;
            isBrightnessBarrierSet = true;
        }

        public void SetImageScaling(float s)
        {
            OCR_imageScaling = s;
            isImageScalingSet = true;
        }

        public void SetLang(Languages l)
        {
            Lang = l;
            isLangSet = true;
        }

        public bool IsAllSettingsSet()
        {
            return isBrightnessBarrierSet && isImageScalingSet && isLangSet;
        }

        public static bool IsEquals(OCRSettings s1, OCRSettings s2)
        {
            if (s1.OCR_brightnessBarrier == s2.OCR_brightnessBarrier &&
                s1.OCR_imageScaling == s2.OCR_imageScaling &&
                s1.Lang == s2.Lang)
                return true;
            return false;
        }
    }
}
