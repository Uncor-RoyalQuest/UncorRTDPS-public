using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UncorRTDPS.FastBitmap;
using UncorRTDPS.Screencap;
using static UncorRTDPS.Screencap.SelectedArea;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_CaptureArea.xaml
    /// </summary>
    public partial class S_CaptureArea : UserControl, IParentTopmostChanger, IDisposable, IMenuPanel
    {
        public S_CaptureArea()
        {
            InitializeComponent();
            InitLocaleText();
            InitDefaultImgs();

            selectedArea = new SelectedArea(RTDPS_Settings.UncorRTDPS_StaticSettings.SelectedAreaRTDPS);
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        private SelectedArea selectedArea;
        private bool isBWFilterEnabled = false;
        private ScreenshotMaker screenshotMaker = new ScreenshotMaker();

        public void InitLocaleText()
        {
            TextBlock_TopLeftPoint.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiTopLeft");
            TextBlock_TopLeftPoint.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiTopLeft_tooltip");
            TextBlock_BotRightPoint.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBottomRight");
            TextBlock_BotRightPoint.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBottomRight_tooltip");
            TextBlock_SelectedArea.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiSelectedArea");
            Button_RefreshImgs.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiRefresh");
            Button_SelectTopLeftByClick.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiSelectClick");
            Button_SelectBotRightByClick.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiSelectClick");
            CheckBox_BWFilter.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBWFilter");
            CheckBox_BWFilter.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiBWFilter_tooltip");
            TextBlock_ShiftByCorner.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiShiftByCorner");
            TextBlock_ShiftByCorner.ToolTip = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiShiftByCorner_tooltip_1")
                + Environment.NewLine
                + RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiShiftByCorner_tooltip_2");
            Button_ApplyNewArea.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiApplyNewArea");
            TextBlock_Comment_ApplyNewArea.Text = "";
        }
        
        public void InitDefaultImgs()
        {
            Image_TopLeftPoint.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
            Image_BotRightPoint.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
            Image_SelectedArea.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;

            Image_Icon_TopLeftCorner.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Icon_TopLeft;
            Image_Icon_TopRightCorner.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Icon_TopRight;
            Image_Icon_BotLeftCorner.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Icon_BotLeft;
            Image_Icon_BotRightCorner.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Icon_BotRight;

            Image_Icon_ShiftTop.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiArrow_Icon_Top;
            Image_Icon_ShiftBot.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiArrow_Icon_Bot;
            Image_Icon_ShiftLeft.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiArrow_Icon_Left;
            Image_Icon_ShiftRight.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiArrow_Icon_Right;
        }

        public void SetBitmapTopLeftPoint(Bitmap bmp)
        {
            if (bmp == null)
            {
                Image_TopLeftPoint.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
            }
            else
            {
                Image_TopLeftPoint.Source = ImageSourceFromBitmap(bmp);
            }
        }

        public void SetBitmapBotRightPoint(Bitmap bmp)
        {
            if (bmp == null)
            {
                Image_BotRightPoint.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
            }
            else
            {
                Image_BotRightPoint.Source = ImageSourceFromBitmap(bmp);
            }
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

        private void Button_SelectTopLeftByClick_Click(object sender, RoutedEventArgs e)
        {
            NotifyAllListenersIParentTopmostListener(false);
            BorderlessScreencapWindow borderlessScreencapWindow = new BorderlessScreencapWindow();
            borderlessScreencapWindow.ShowDialog();
            if (borderlessScreencapWindow.ResultExists == true)
            {
                selectedArea.SetTopLeft(borderlessScreencapWindow.ResultX, borderlessScreencapWindow.ResultY);

                UpdateTopLeftBotRightImgs();
                UpdateSelectedArea();
            }
            borderlessScreencapWindow.Dispose();
            System.GC.Collect();
            NotifyAllListenersIParentTopmostListener(true);
        }

        private void Button_SelectBotRightByClick_Click(object sender, RoutedEventArgs e)
        {
            NotifyAllListenersIParentTopmostListener(false);
            BorderlessScreencapWindow borderlessScreencapWindow = new BorderlessScreencapWindow();
            borderlessScreencapWindow.ShowDialog();
            if (borderlessScreencapWindow.ResultExists == true)
            {
                selectedArea.SetBotRight(borderlessScreencapWindow.ResultX, borderlessScreencapWindow.ResultY);

                UpdateTopLeftBotRightImgs();
                UpdateSelectedArea();
            }
            borderlessScreencapWindow.Dispose();
            System.GC.Collect();
            NotifyAllListenersIParentTopmostListener(true);
        }

        public Bitmap MakeBitmapForSelectedPoint(int X_screen, int Y_screen)
        {
            int width = 48;
            int height = 48;
            int x = X_screen - (width / 2);
            int y = Y_screen - (height / 2);
            int subX = width / 2;
            int subY = height / 2;
            if (x < 0)
            {
                subX -= Math.Abs(x);
                x = 0;
            }
            if (y < 0)
            {
                subY -= Math.Abs(y);
                y = 0;
            }
            width = (int)(width * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_X);
            height = (int)(height * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_Y);
            Bitmap bmp = screenshotMaker.MakeScreenshot(x, y, width, height);
            if (bmp == null)
                return null;
            Graphics g = Graphics.FromImage(bmp);
            subX = (int)(subX * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_X);
            subY = (int)(subY * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_Y);
            g.DrawImage(RTDPS_Settings.UncorRTDPS_StaticSettings.ImgCursor, subX, subY);
            g.Dispose();
            return bmp;
        }

        public void UpdateSelectedArea()
        {
            if (selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet)
            {
                Bitmap bmp = screenshotMaker.MakeScreenshot(selectedArea.X_topLeft, selectedArea.Y_topLeft, selectedArea.Width, selectedArea.Height);
                if (bmp == null)
                {
                    Image_SelectedArea.Source = RTDPS_Settings.UncorRTDPS_StaticSettings.BiSquare_Question;
                    return;
                }
                if (isBWFilterEnabled)
                {
                    FastBitmap.FastBitmap fastBitmap = new FastBitmap.FastBitmap(bmp);
                    fastBitmap.Lock();
                    BitmapTransformations.MakeBlackWhite(fastBitmap, RTDPS_Settings.UncorRTDPS_StaticSettings.OcrSettings.OCR_brightnessBarrier);
                    fastBitmap.Unlock();
                    fastBitmap.Dispose();
                }
                SetBitmapSelectedArea(bmp);
            }
            SetApplyComment_Clear();
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

        //public bool[] isSelectedCorner = { false, false, false, false }; //0 - top left; 1 - top right; 2 - bot left; 3 - bot right;
        private Corners nowSelectedCorner = Corners.None;
        private SolidColorBrush defaultButtonBackground = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 45, 45, 48));
        private SolidColorBrush defaultButtonBackgroundSelected = new SolidColorBrush(Colors.White);
        private void Button_Icon_TopLeftCorner_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedCorner == Corners.TopLeft)
            {
                Button_Icon_TopLeftCorner.Background = defaultButtonBackground;
                nowSelectedCorner = Corners.None;
            }
            else
            {
                SetBackgroundForAllCorners(defaultButtonBackground);
                Button_Icon_TopLeftCorner.Background = defaultButtonBackgroundSelected;
                nowSelectedCorner = Corners.TopLeft;
            }
        }

        private void Button_Icon_TopRightCorner_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedCorner == Corners.TopRight)
            {
                Button_Icon_TopRightCorner.Background = defaultButtonBackground;
                nowSelectedCorner = Corners.None;
            }
            else
            {
                SetBackgroundForAllCorners(defaultButtonBackground);
                Button_Icon_TopRightCorner.Background = defaultButtonBackgroundSelected;
                nowSelectedCorner = Corners.TopRight;
            }
        }

        private void Button_Icon_BotLeftCorner_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedCorner == Corners.BotLeft)
            {
                Button_Icon_BotLeftCorner.Background = defaultButtonBackground;
                nowSelectedCorner = Corners.None;
            }
            else
            {
                SetBackgroundForAllCorners(defaultButtonBackground);
                Button_Icon_BotLeftCorner.Background = defaultButtonBackgroundSelected;
                nowSelectedCorner = Corners.BotLeft;
            }
        }

        private void Button_Icon_BotRightCorner_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedCorner == Corners.BotRight)
            {
                Button_Icon_BotRightCorner.Background = defaultButtonBackground;
                nowSelectedCorner = Corners.None;
            }
            else
            {
                SetBackgroundForAllCorners(defaultButtonBackground);
                Button_Icon_BotRightCorner.Background = defaultButtonBackgroundSelected;
                nowSelectedCorner = Corners.BotRight;
            }
        }

        public void SetBackgroundForAllCorners(SolidColorBrush scb)
        {
            Button_Icon_TopLeftCorner.Background = scb;
            Button_Icon_TopRightCorner.Background = scb;
            Button_Icon_BotLeftCorner.Background = scb;
            Button_Icon_BotRightCorner.Background = scb;
        }

        private void CheckBox_BWFilter_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBox_BWFilter.IsChecked == true)
            {
                isBWFilterEnabled = true;
            }
            else if (CheckBox_BWFilter.IsChecked == false)
            {
                isBWFilterEnabled = false;
            }
            UpdateSelectedArea();
        }

        public void UpdateTopLeftBotRightImgs()
        {
            if (selectedArea.IsTopLeftSet)
            {
                SetBitmapTopLeftPoint(MakeBitmapForSelectedPoint(selectedArea.X_topLeft, selectedArea.Y_topLeft));
            }

            if (selectedArea.IsBotRightSet)
            {
                SetBitmapBotRightPoint(MakeBitmapForSelectedPoint(selectedArea.X_botRight, selectedArea.Y_botRight));
            }
        }

        private List<IParentTopmostListener> parentTopmostListeners = new List<IParentTopmostListener>();
        public void RegisterIParentTopmostListener(IParentTopmostListener listener)
        {
            if (parentTopmostListeners.Contains(listener))
                return;
            parentTopmostListeners.Add(listener);
        }

        public void UnregisterIParentTopmostListener(IParentTopmostListener listener)
        {
            if (!parentTopmostListeners.Contains(listener))
                return;
            parentTopmostListeners.Remove(listener);
        }

        public void NotifyAllListenersIParentTopmostListener(bool topmost)
        {
            for (int i = 0; i < parentTopmostListeners.Count; i++)
                parentTopmostListeners[i].FireTopmostChanged(topmost);
        }

        public void Dispose()
        {
            parentTopmostListeners.Clear();

            Image_TopLeftPoint.Source = null;
            Image_BotRightPoint.Source = null;
            Image_SelectedArea.Source = null;

            Image_Icon_TopLeftCorner.Source = null;
            Image_Icon_TopRightCorner.Source = null;
            Image_Icon_BotLeftCorner.Source = null;
            Image_Icon_BotRightCorner.Source = null;

            Image_Icon_ShiftTop.Source = null;
            Image_Icon_ShiftBot.Source = null;
            Image_Icon_ShiftLeft.Source = null;
            Image_Icon_ShiftRight.Source = null;

            TextBlock_TopLeftPoint.Text = null;
            TextBlock_TopLeftPoint.ToolTip = null;
            TextBlock_BotRightPoint.Text = null;
            TextBlock_BotRightPoint.ToolTip = null;
            TextBlock_SelectedArea.Text = null;
            Button_RefreshImgs.Content = null;
            Button_SelectTopLeftByClick.Content = null;
            Button_SelectBotRightByClick.Content = null;
            CheckBox_BWFilter.Content = null;
            CheckBox_BWFilter.ToolTip = null;
            TextBlock_ShiftByCorner.Text = null;
            TextBlock_ShiftByCorner.ToolTip = null;
            Button_ApplyNewArea.Content = null;
            TextBlock_Comment_ApplyNewArea.Text = null;
        }

        private void Button_ApplyNewArea_Click(object sender, RoutedEventArgs e)
        {
            if (!selectedArea.IsTopLeftSet || !selectedArea.IsBotRightSet)
            {
                SetApplyComment_MissingPoints();
                return;
            }
            if (!selectedArea.IsThisPointsCoorectlyCorresponding())
            {
                SetApplyComment_WrognPoints();
                return;
            }
            RTDPS_Settings.UncorRTDPS_StaticSettings.UpdateSelectedAreaRTDPS(selectedArea);
            SetApplyComment_Applied();
        }

        private void Button_RefreshImgs_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        public void SetApplyComment_Applied()
        {
            TextBlock_Comment_ApplyNewArea.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentApplied");
        }

        public void SetApplyComment_MissingPoints()
        {
            TextBlock_Comment_ApplyNewArea.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentMissingPoint");
        }

        public void SetApplyComment_WrognPoints()
        {
            TextBlock_Comment_ApplyNewArea.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentPointSetWrong");
        }

        public void SetApplyComment_Clear()
        {
            if (TextBlock_Comment_ApplyNewArea.Text == null || TextBlock_Comment_ApplyNewArea.Text.Length == 0)
                return;
            TextBlock_Comment_ApplyNewArea.Text = "";
        }

        public void ActivateMenuPanel()
        {
            this.Visibility = Visibility.Visible;
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        public void DeactivateMenuPanel()
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Button_Icon_ShiftTop_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!(selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet))
                return;
            if (nowSelectedCorner == Corners.None)
            {
                selectedArea.ShiftTop(1);
            }
            else
            {
                selectedArea.ShiftCornerTop(nowSelectedCorner, 1);
            }
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        private void Button_Icon_ShiftLeft_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!(selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet))
                return;
            if (nowSelectedCorner == Corners.None)
            {
                selectedArea.ShiftLeft(1);
            }
            else
            {
                selectedArea.ShiftCornerLeft(nowSelectedCorner, 1);
            }
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        private void Button_Icon_ShiftRight_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!(selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet))
                return;
            if (nowSelectedCorner == Corners.None)
            {
                selectedArea.ShiftRight(1);
            }
            else
            {
                selectedArea.ShiftCornerRight(nowSelectedCorner, 1);
            }
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }

        private void Button_Icon_ShiftBot_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            if (!(selectedArea.IsTopLeftSet && selectedArea.IsBotRightSet))
                return;
            if (nowSelectedCorner == Corners.None)
            {
                selectedArea.ShiftBot(1);
            }
            else
            {
                selectedArea.ShiftCornerBot(nowSelectedCorner, 1);
            }
            UpdateSelectedArea();
            UpdateTopLeftBotRightImgs();
        }
    }
}
