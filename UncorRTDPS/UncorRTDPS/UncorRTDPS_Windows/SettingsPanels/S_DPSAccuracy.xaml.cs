using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_DPSAccuracy.xaml
    /// </summary>
    public partial class S_DPSAccuracy : UserControl, IMenuPanel, IDisposable
    {
        private string menu_name = "dps_acc";

        public S_DPSAccuracy()
        {
            InitializeComponent();
            InitLocaleText();
            ocrSettings = new UncorOCR.OCRSettings(RTDPS_Settings.UncorRTDPS_StaticSettings.OcrSettings);
            Slider_Barrier_Value.Value = ocrSettings.OCR_brightnessBarrier;
            Slider_Scaling_Value.Value = ocrSettings.OCR_imageScaling;

            selectedGameLanguage = ocrSettings.Lang == RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian ? 0 : 1;
            ComboBox_GameLang.SelectedIndex = selectedGameLanguage;
            InitDefaultImages();
            //refreshAllElements();
        }

        private int selectedGameLanguage;

        private Screencap.ScreenshotMaker screenshotMaker = new Screencap.ScreenshotMaker();

        private UncorOCR.OCRSettings ocrSettings;
        private UncorOCR.Supplementary_Parameters_DamageOCR learntSupplementaryParameters;

        public void InitLocaleText()
        {
            TextBlock_RecognizedDamage.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiRecognizedDamage");
            TextBlock_RecognizedDamage_Value.ToolTip = "";
            TextBlock_Scaling.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiScaling");
            TextBlock_Scaling.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiScaling_tooltip");
            TextBlock_Barrier.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBarrier");
            TextBlock_Barrier.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBarrier_tooltip");
            Button_ApplyNewTweaks.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiApplyNewAccuracy");
            Button_Refresh.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiRefresh");
            TextBlock_Comment_ApplyNewTweaks.Text = "";
            TextBlock_GameLang.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGameLang");
        }

        private void InitDefaultImages()
        {
            Image_SelectedArea.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
            Image_SelectedArea_PlusFilters.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
        }

        public void UpdateSelectedArea()
        {
            Screencap.SelectedArea selectedArea = new Screencap.SelectedArea(RTDPS_Settings.UncorRTDPS_StaticSettings.SelectedAreaRTDPS);
            if (selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet)
            {
                Bitmap bmp = screenshotMaker.MakeScreenshot(selectedArea.X_topLeft, selectedArea.Y_topLeft, selectedArea.Width, selectedArea.Height);
                if (bmp == null)
                {
                    Image_SelectedArea.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
                    return;
                }
                SetBitmapSelectedArea(bmp);
            }
        }

        public void UpdateSelectedArea_FiltersApplied()
        {
            Screencap.SelectedArea selectedArea = new Screencap.SelectedArea(RTDPS_Settings.UncorRTDPS_StaticSettings.SelectedAreaRTDPS);
            if (selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet)
            {
                Bitmap bmp = screenshotMaker.MakeScreenshot(selectedArea.X_topLeft, selectedArea.Y_topLeft, selectedArea.Width, selectedArea.Height);
                if (bmp == null)
                {
                    Image_SelectedArea_PlusFilters.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
                    return;
                }

                //learn params
                UncorOCR.DamageOCR_Target_LearnParams learnParams = new UncorOCR.DamageOCR_Target_LearnParams();
                Bitmap bmpAfterTraining = learnParams.TrainWithFullCycleOfTransformations(bmp, ocrSettings);
                if (bmpAfterTraining != null)
                {
                    learntSupplementaryParameters = learnParams.packAndGetAllTrainedParams();

                    UpdateRecognizedText(bmp);

                    SetBitmapSelectedArea_FiltersApplied(bmpAfterTraining);
                }
                else
                {
                    TextBlock_RecognizedDamage_Value.Text = "Error #1";
                    Image_SelectedArea_PlusFilters.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
                }
                bmp.Dispose();
                //end learn params
            }
        }

        public void UpdateRecognizedText(Bitmap bmp)
        {
            //make ocr recognition
            UncorOCR.DamageOCR_Target_v0 damageOCR_Target_V0 = new UncorOCR.DamageOCR_Target_v0();
            if (!damageOCR_Target_V0.UpdateSettingsOCR(ocrSettings, learntSupplementaryParameters))
            {
                TextBlock_RecognizedDamage_Value.Text = "Error #2";
                damageOCR_Target_V0.Dispose();
                return;
            }
            damageOCR_Target_V0.InitEngine(ocrSettings.Lang);
            int recRes = damageOCR_Target_V0.ProcessDamageRecognition(bmp);
            if (recRes != 0)
            {
                TextBlock_RecognizedDamage_Value.Text = "Error #3";
                damageOCR_Target_V0.Dispose();
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < damageOCR_Target_V0.damageTargetPair_currLen; i++)
            {
                sb.Append(damageOCR_Target_V0.damageTargetPair[i].damage + "   " + damageOCR_Target_V0.damageTargetPair[i].target);
                if (i < damageOCR_Target_V0.damageTargetPair_currLen - 1)
                    sb.Append(Environment.NewLine);
            }
            TextBlock_RecognizedDamage_Value.Text = sb.ToString();
            damageOCR_Target_V0.Dispose();
        }

        public void SetBitmapSelectedArea(Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            if (w > 300 || h > 300)
            {
                Image_SelectedArea.MaxWidth = 300;
                Image_SelectedArea.MaxHeight = 300;
            }
            else
            {
                Image_SelectedArea.MaxWidth = w;
                Image_SelectedArea.MaxHeight = h;
            }

            Image_SelectedArea.Source = ImageSourceFromBitmap(bmp);
        }

        public void SetBitmapSelectedArea_FiltersApplied(Bitmap bmp)
        {
            if (bmp == null)
            {
                Image_SelectedArea_PlusFilters.Source = null;
                return;
            }
            int w = bmp.Width;
            int h = bmp.Height;
            if (w > 300 || h > 300)
            {
                Image_SelectedArea_PlusFilters.MaxWidth = 300;
                Image_SelectedArea_PlusFilters.MaxHeight = 300;
            }
            else
            {
                Image_SelectedArea_PlusFilters.MaxWidth = w;
                Image_SelectedArea_PlusFilters.MaxHeight = h;
            }

            Image_SelectedArea_PlusFilters.Source = ImageSourceFromBitmap(bmp);
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public void SetApplyComment_Applied()
        {
            TextBlock_Comment_ApplyNewTweaks.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentApplied");
        }

        public void SetApplyComment_ErrorBadValuesInSlider()
        {
            TextBlock_Comment_ApplyNewTweaks.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentErrorBadValInSlider");
        }

        public void SetApplyComment_Clear()
        {
            if (TextBlock_Comment_ApplyNewTweaks.Text == null || TextBlock_Comment_ApplyNewTweaks.Text.Length == 0)
                return;
            TextBlock_Comment_ApplyNewTweaks.Text = "";
        }

        public void ActivateMenuPanel()
        {
            this.Visibility = Visibility.Visible;

            RefreshAllElements();
        }

        public void DeactivateMenuPanel()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void RefreshTweaks()
        {
            float barrierVal = (float)Slider_Barrier_Value.Value;
            float scalingVal = (float)Slider_Scaling_Value.Value;
            int gameLang = ComboBox_GameLang.SelectedIndex;
            if (float.IsNaN(barrierVal) || float.IsNaN(scalingVal))
                return;
            ocrSettings.SetBrightnessBarrier(barrierVal);
            ocrSettings.SetImageScaling(scalingVal);
            ocrSettings.SetLang(gameLang == 0 ? RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian : RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English);
        }

        public bool IsAllArrayNotMinusOne(List<int> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr[i] == -1)
                    return false;
            }
            return true;
        }


        private void Button_ApplyNewTweaks_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllElements();
            float barrierVal = (float)Slider_Barrier_Value.Value;
            float scalingVal = (float)Slider_Scaling_Value.Value;

            if (!float.IsNaN(barrierVal) && !float.IsNaN(scalingVal) && (learntSupplementaryParameters != null))
            {
                //ok
                RefreshTweaks();
                RTDPS_Settings.UncorRTDPS_StaticSettings.UpdateOCRSettings(ocrSettings);
                RTDPS_Settings.UncorRTDPS_StaticSettings.UpdateOCRSupplementaryParameters(learntSupplementaryParameters);
                SetApplyComment_Applied();
            }
            else
            {
                //bad
                SetApplyComment_ErrorBadValuesInSlider();
            }
        }


        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshAllElements();
        }

        public void RefreshAllElements()
        {
            SetApplyComment_Clear();
            RefreshTweaks();
            UpdateSelectedArea();
            UpdateSelectedArea_FiltersApplied();
        }

        public void RefreshAllElements_NoMsgClearing()
        {
            RefreshTweaks();
            UpdateSelectedArea();
            UpdateSelectedArea_FiltersApplied();
        }

        public void Dispose()
        {
            Image_SelectedArea = null;
            Image_SelectedArea_PlusFilters = null;

            TextBlock_RecognizedDamage.Text = null;
            TextBlock_RecognizedDamage_Value.ToolTip = null;
            TextBlock_Scaling.Text = null;
            TextBlock_Scaling.ToolTip = null;
            TextBlock_Barrier.Text = null;
            TextBlock_Barrier.ToolTip = null;
            Button_ApplyNewTweaks.Content = null;
            Button_Refresh.Content = null;
            TextBlock_Comment_ApplyNewTweaks.Text = null;
            TextBlock_GameLang.Text = null;
        }

        public string GetMenuName()
        {
            return this.menu_name;
        }
    }
}
