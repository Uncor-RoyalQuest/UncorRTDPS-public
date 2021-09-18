using System;
using System.Drawing;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UncorRTDPS.Screencap;
using UncorRTDPS.Util;

namespace UncorRTDPS.UncorRTDPS_Windows
{
    /// <summary>
    /// Interaction logic for BorderlessScreencapWindow.xaml
    /// </summary>
    public partial class BorderlessScreencapWindow : Window, IDisposable
    {

        private ScreenshotMaker screenshotMaker;
        private SolidBrush solidBrush;

        private int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        private int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        public int ResultX { get; set; } = 0;
        public int ResultY { get; set; } = 0;
        public bool ResultExists { get; set; } = false;

        public BorderlessScreencapWindow()
        {
            InitializeComponent();

            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            screenWidth = (int)(SystemParameters.PrimaryScreenWidth * ((double)dpiX / 96));
            screenHeight = (int)(SystemParameters.PrimaryScreenHeight * ((double)dpiY / 96));

            screenshotMaker = new Screencap.ScreenshotMaker();
            solidBrush = new SolidBrush(System.Drawing.Color.FromArgb(50, 171, 171, 167));
            Bitmap bmp = screenshotMaker.MakeScreenshot(0, 0, screenWidth, screenHeight);


            if (bmp == null)
                return;
            MakeBmpDistinctive(bmp);
            SetScreenImage(bmp);
        }

        public void SetScreenImage(Bitmap bmp)
        {
            Image_ScreenImage.Source = ImageSourceFromBitmap(bmp);
        }


        public void MakeBmpDistinctive(Bitmap bmp)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(solidBrush, new System.Drawing.Rectangle(0, 0, screenWidth, screenHeight));
            g.Dispose();
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.ResultExists = false;
            this.WindowState = WindowState.Minimized;
            this.Close();
        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(this);
            ResultX = (int)(p.X * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_X);
            ResultY = (int)(p.Y * RTDPS_Settings.UncorRTDPS_StaticSettings.ScreenScaleFactorByDpi_Y);

            ResultExists = true;
            this.WindowState = WindowState.Minimized;
            Close();
        }

        public ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObjectWrapper.DeleteObject(handle); }
        }

        public void Dispose()
        {
            Image_ScreenImage.Source = null;
            solidBrush = null;
            screenshotMaker = null;
            BindingOperations.ClearAllBindings(Image_ScreenImage);
            BindingOperations.ClearAllBindings(this);
        }
    }
}
