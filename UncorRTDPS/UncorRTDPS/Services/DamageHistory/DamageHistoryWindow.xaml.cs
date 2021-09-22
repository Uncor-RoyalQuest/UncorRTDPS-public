using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UncorRTDPS.DamageCharts;
using UncorRTDPS.DpsModels;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.Services.WindowSize;
using UncorRTDPS.UncorRTDPS_Windows.EventAware;

namespace UncorRTDPS.Services.DamageHistory
{
    /// <summary>
    /// Interaction logic for DamageHistoryWindow.xaml
    /// </summary>
    public partial class DamageHistoryWindow : Window, IDisposable, ICloseAllWindowsButMainAware
    {

        private EntityViewModel entityViewModel = new EntityViewModel();
        private List<DamageModel> listDamageModel = null;

        private const string name = "damageHistoryWindow";
        private WindowPositionService windowPositionService = null;
        private WindowSizeService windowSizeService = null;
        public bool IsStartupPositionUnknown { get; set; } = true;

        public DamageHistoryWindow()
        {
            InitializeComponent();
            InitLocalization();
            UpdColumnsVisibility();
            UpdContextMenuBasedOnColumns();

            (this as ICloseAllWindowsButMainAware).Register_CloseAllWindowsButMainAware();

            Service service = ServicesContainer.GetService("windowPositionService");
            if (service != null && service is WindowPositionService)
            {
                windowPositionService = service as WindowPositionService;
                Point<double> p = windowPositionService.GetWindowPosition(name);
                if (p != null)
                {
                    this.Top = p.Y;
                    this.Left = p.X;
                    IsStartupPositionUnknown = false;
                }
            }

            service = ServicesContainer.GetService("windowSizeService");
            if (service != null && service is WindowSizeService)
            {
                windowSizeService = service as WindowSizeService;
                Size<double> s = windowSizeService.GetWindowPosition(name);
                if (s != null)
                {
                    this.Width = s.Width;
                    this.Height = s.Height;
                }
            }

            listDamageModel = (ServicesContainer.GetService("recentDamage") as RecentDamage).GetClonedRecentDamage();
            entityViewModel.LoadEntities(listDamageModel);
            DataGrid_Entities.DataContext = entityViewModel;

            /*
            AddFakeEntityToDatagrid("Архон", 123, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Деструктор", 1234, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Деструктор", 12345, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("П'атаг", 543, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("П'атаг", 5437, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Пламярык", 54793, DpsModels.TargetsDictionary.TargetType.Elite);
            AddFakeEntityToDatagrid("Пламярык", 50993, DpsModels.TargetsDictionary.TargetType.Elite);
            AddFakeEntityToDatagrid("Пламярык", 9603, DpsModels.TargetsDictionary.TargetType.Elite);
            */

        }

        private void InitLocalization()
        {
            TextBlock_Header_RecentDamage_Name.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Name");
            TextBlock_Header_RecentDamage_DamagePercent.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_DamagePercent");
            TextBlock_Header_RecentDamage_Damage.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Damage");
            TextBlock_Header_RecentDamage_Dps.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Dps");
            TextBlock_Header_RecentDamage_Hits.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Hits");
            TextBlock_Header_RecentDamage_MaxHitDmg.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_MaxHitDmg");
            TextBlock_Header_RecentDamage_Time.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Time");
            TextBlock_Header_RecentDamage_TimeStart.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeStart");
            TextBlock_Header_RecentDamage_TimeEnd.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeEnd");
            TextBlock_Header_RecentDamage_Details.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Details");

            ContextMenu_Item_ShowIcon.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Icon");
            ContextMenu_Item_ShowName.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Name");
            ContextMenu_Item_ShowDamagePercent.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_DamagePercent");
            ContextMenu_Item_ShowDamage.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Damage");
            ContextMenu_Item_ShowDps.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Dps");
            ContextMenu_Item_ShowHits.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Hits");
            ContextMenu_Item_ShowMaxHitDmg.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_MaxHitDmg");
            ContextMenu_Item_ShowTime.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Time");
            ContextMenu_Item_ShowTimeStart.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeStart");
            ContextMenu_Item_ShowTimeEnd.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeEnd");
            ContextMenu_Item_ShowDetails.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Details");

        }

        private void UpdColumnsVisibility()
        {
            Column_Img.Visibility = GetVisibilityForColumn("dmgHistory_Show_Icon");
            Column_Name.Visibility = GetVisibilityForColumn("dmgHistory_Show_Name");
            Column_DamagePercent.Visibility = GetVisibilityForColumn("dmgHistory_Show_DamagePercent");
            Column_Damage.Visibility = GetVisibilityForColumn("dmgHistory_Show_Damage");
            Column_Dps.Visibility = GetVisibilityForColumn("dmgHistory_Show_Dps");
            Column_Hits.Visibility = GetVisibilityForColumn("dmgHistory_Show_Hits");
            Column_MaxHitDmg.Visibility = GetVisibilityForColumn("dmgHistory_Show_MaxHitDmg");
            Column_Img.Visibility = GetVisibilityForColumn("dmgHistory_Show_Icon");
            Column_BattleDuration.Visibility = GetVisibilityForColumn("dmgHistory_Show_Time");
            Column_TimeStart.Visibility = GetVisibilityForColumn("dmgHistory_Show_TimeStart");
            Column_TimeEnd.Visibility = GetVisibilityForColumn("dmgHistory_Show_TimeEnd");
            Column_Details.Visibility = GetVisibilityForColumn("dmgHistory_Show_Details");
        }

        private void UpdContextMenuBasedOnColumns()
        {
            ContextMenu_Item_ShowIcon.IsChecked = Column_Img.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowName.IsChecked = Column_Name.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowDamagePercent.IsChecked = Column_DamagePercent.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowDamage.IsChecked = Column_Damage.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowDps.IsChecked = Column_Dps.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowHits.IsChecked = Column_Hits.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowMaxHitDmg.IsChecked = Column_MaxHitDmg.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowTime.IsChecked = Column_BattleDuration.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowTimeStart.IsChecked = Column_TimeStart.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowTimeEnd.IsChecked = Column_TimeEnd.Visibility == Visibility.Visible;
            ContextMenu_Item_ShowDetails.IsChecked = Column_Details.Visibility == Visibility.Visible;
        }

        private Visibility GetVisibilityForColumn(string configColumnName)
        {
            bool? bv = UncorRTDPS_Config.GetConfigVal_Bool(configColumnName);
            return bv == null || !bv.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        /*
        private void AddFakeEntityToDatagrid(string name, int seed, DpsModels.TargetsDictionary.TargetType mobType)
        {
            Random r = new Random(seed);
            List<Damage> damages = new List<Damage>();
            int rDamagesLen = r.Next(20, 100);
            for (int i = 0; i < rDamagesLen; i++)
            {
                damages.Add(new Damage(r.Next(1000, 10000), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + i * 250));
            }
            DamageSequence damageSequence = new DamageSequence(damages);
            int rPlusTime = r.Next(1000, 1000000);
            DamageModel fakeDamageModel = new DamageModel(
                    new DpsModels.TargetsDictionary.Target(r.Next(0, 1000000), mobType, name, r.Next(1000000, 10000000)),
                    false,
                    r.Next(500000, 2000000),
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + rPlusTime,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + rPlusTime + r.Next(5000, 30000),
                    damages.Count,
                    r.Next(7000, 10000),
                    false,
                    damageSequence,
                    5000
                    );
            entityViewModel.AddEntity(fakeDamageModel);
            listDamageModel.Add(fakeDamageModel);
        }
        */

        public void Dispose()
        {
            this.windowPositionService = null;
            this.windowSizeService = null;

            this.listDamageModel = null;
            DataGrid_Entities.DataContext = null;
            entityViewModel = null;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int pos = (row.Item as Entity).Id;
                    DamageChartWindow damageChartWindow = new DamageChartWindow();
                    damageChartWindow.Init(listDamageModel[pos].DamageSequence.Clone(), listDamageModel[pos].Target.originalName, listDamageModel[pos].Target.hp);
                    if (damageChartWindow.IsStartupPositionUnknown)
                    {
                        damageChartWindow.Top = this.Top;
                        damageChartWindow.Left = this.Left;
                    }
                    damageChartWindow.Show();
                    break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowPositionService.UpdateWindowPosition(name, new Point<double>(this.Left, this.Top));
            windowSizeService.UpdateWindowSize(name, new Size<double>(this.ActualWidth, this.ActualHeight));
            (this as ICloseAllWindowsButMainAware).Unregister_CloseAllWindowsButMainAware();
            Dispose();
        }

        public void CloseAllWindowsButMainAware_HandleEvent(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ContextMenu_Item_ShowIcon_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowIcon.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Icon", (!visible).ToString());
            Column_Img.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowName_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowName.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Name", (!visible).ToString());
            Column_Name.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowDamagePercent_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowDamagePercent.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_DamagePercent", (!visible).ToString());
            Column_DamagePercent.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowDamage_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowDamage.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Damage", (!visible).ToString());
            Column_Damage.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowDps_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowDps.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Dps", (!visible).ToString());
            Column_Dps.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowHits_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowHits.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Hits", (!visible).ToString());
            Column_Hits.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowMaxHitDmg_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowMaxHitDmg.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_MaxHitDmg", (!visible).ToString());
            Column_MaxHitDmg.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowTime_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowTime.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Time", (!visible).ToString());
            Column_BattleDuration.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowTimeStart_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowTimeStart.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_TimeStart", (!visible).ToString());
            Column_TimeStart.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowTimeEnd_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowTimeEnd.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_TimeEnd", (!visible).ToString());
            Column_TimeEnd.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ContextMenu_Item_ShowDetails_Click(object sender, RoutedEventArgs e)
        {
            bool visible = ContextMenu_Item_ShowDetails.IsChecked;
            UncorRTDPS_Config.UpdateConfigVal("dmgHistory_Show_Details", (!visible).ToString());
            Column_Details.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
