using System;
using System.IO;
using System.Windows;

namespace UncorRTDPS_standalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FileStream fs;

        public MainWindow()
        {
            //crude check if already running / doesnt have necessary privileges (admin)
            string fPath = Path.GetFullPath(Path.Combine(ExecutingFolder, "h4QjwnmB0.uncor"));
            try
            {
                fs = new FileStream(fPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Probably, UncorRTDPS is already running." + Environment.NewLine + "If not, try \"Run As Administrator\"");
                this.Close();
            }

            string errLogPath = Path.GetFullPath(Path.Combine(ExecutingFolder, "errlog.txt"));
            AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
            {
                string ex = eventArgs.Exception.ToString();
                if (ex.Contains("shcore.dll") ||
                ex.Contains("DpiAwareness") ||
                ex.Contains("GetDpi") ||
                ex.Contains("GetWindowDpi") ||
                ex.Contains("XmlSerializers.dll") ||
                ex.Contains("The process has no package identity."))
                {
                    return;
                }
                StreamWriter sw = new StreamWriter(errLogPath, append: true);
                sw.WriteLine(DateTime.Now);
                sw.WriteLine(eventArgs.Exception.ToString());
                sw.Flush();
                sw.Close();
                FileInfo fi = new FileInfo(errLogPath);
                if (fi.Exists && fi.Length > 5000000L)
                {
                    fi.Delete();
                }
            };

            InitializeComponent();
        }

        public static string ExecutingFolder { get; set; } = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
        //public static string ExecutingFolder { get; set; } = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        public static string systemUILanguage = System.Globalization.CultureInfo.CurrentUICulture.Name.ToLower();

        public void StartUncorRTDPS()
        {
            string lang = systemUILanguage.StartsWith("ru") ? "ru" : "en";
            string explicitUiLangFileName = Path.GetFullPath(Path.Combine(ExecutingFolder, "lang.txt"));
            if (File.Exists(explicitUiLangFileName))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(explicitUiLangFileName))
                    {
                        string line = streamReader.ReadToEnd().Trim();
                        if (line == "ru")
                            lang = "ru";
                        if (line == "en")
                            lang = "en";
                    }
                }
                catch { }
            }
            UncorRTDPS.UncorRTDPS_Starter.InitUncorRTDPS(ExecutingFolder, lang);
            new UncorRTDPS.UncorRTDPS_Windows.StatsHoveringWindow().Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //prep modules
            if (!UncorRTDPS.UncorRTDPS_Starter.Check_AllModulesInstalled(ExecutingFolder))
            {
                this.Close();
                return;
            }

            StartUncorRTDPS();

            //close entry window
            this.Close();
        }
    }
}
