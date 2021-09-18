using System;
using System.Windows;

namespace UncorRTDPS.UtilWindows
{
    /// <summary>
    /// Interaction logic for MissingVisualCPPRedist.xaml
    /// </summary>
    public partial class MissingVisualCPPRedist : Window
    {
        public MissingVisualCPPRedist()
        {
            InitializeComponent();
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.CurrentUICulture;
            if (ci.Name.ToLower().StartsWith("ru"))
            {
                RichTextBox_RussianVersion.Visibility = Visibility.Visible;
            }
            else
            {
                RichTextBox_EnglishVersion.Visibility = Visibility.Visible;
            }

            if (Environment.Is64BitOperatingSystem)
            {
                Run_En_Bit.Text += " 64-bit.";
                Run_Ru_Bit.Text += " 64-bit.";
            }
            else
            {
                Run_En_Bit.Text += " 32-bit.";
                Run_Ru_Bit.Text += " 32-bit.";
            }
        }

        private void HyperLink_En_64bit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/C start https://aka.ms/vs/16/release/vc_redist.x64.exe");
        }

        private void HyperLink_En_32bit_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/C start https://aka.ms/vs/16/release/vc_redist.x86.exe");
        }
    }
}
