using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_AboutProgram.xaml
    /// </summary>
    public partial class S_AboutProgram : UserControl, IMenuPanel
    {
        public S_AboutProgram()
        {
            InitializeComponent();
            switch (RTDPS_Settings.UncorRTDPS_StaticSettings.Language_UI)
            {
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English:
                    RichTextBox_En.Visibility = Visibility.Visible;
                    RichTextBox_Ru.Visibility = Visibility.Collapsed;
                    break;
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian:
                    RichTextBox_Ru.Visibility = Visibility.Visible;
                    RichTextBox_En.Visibility = Visibility.Collapsed;
                    break;
                default:
                    RichTextBox_En.Visibility = Visibility.Visible;
                    RichTextBox_Ru.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        public void ActivateMenuPanel()
        {
            this.Visibility = Visibility.Visible;
        }

        public void DeactivateMenuPanel()
        {
            this.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("cmd", "/C start https://www.royalquest.ru/forum/index.php?showtopic=43506");
        }
    }
}
