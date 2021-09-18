using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using UncorRTDPS.DpsModels;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.Services;
using UncorRTDPS.Services.WindowSize;
using UncorRTDPS.UncorRTDPS_Windows.EventAware;

namespace UncorRTDPS.DamageCharts
{
    /// <summary>
    /// Interaction logic for DamageChartWindow.xaml
    /// </summary>
    public partial class DamageChartWindow : Window, IDisposable, ICloseAllWindowsButMainAware, ICloseAllWindowsChartsAware
    {
        private bool ignoreCheckedUncheckedEvent = true;
        private ScatterPlot damageHitsScatter;
        private ScatterPlot damageTimeScatter;
        private HLine chart_DamageByTime_HLine_AvgDmg, chart_DamageByHit_HLine_AvgDmg, chart_DamageByHit_HLine_AvgHitDmg, chart_DamageByTime_HLine_AvgHitDmg;
        private Text chart_DamageByTime_Text_AvgDmg, chart_DamageByHit_Text_AvgDmg, chart_DamageByHit_Text_AvgHitDmg, chart_DamageByTime_Text_AvgHitDmg;
        private List<ScatterPlot> chart_DamageByTime_Arrows_Max, chart_DamageByTime_Arrows_Min;
        private List<ScatterPlot> chart_DamageByHits_Arrows_Max, chart_DamageByHits_Arrows_Min;
        private Text chart_DamageByTime_Text_Max, chart_DamageByTime_Text_Min;
        private Text chart_DamageByHits_Text_Max, chart_DamageByHits_Text_Min;
        private HLine chart_DamageByTime_HLine_Max, chart_DamageByTime_HLine_Min;
        private HLine chart_DamageByHits_HLine_Max, chart_DamageByHits_HLine_Min;
        private ScatterPlot highlightedPoint_ByHitsChart;
        private ScatterPlot highlightedPoint_ByTimeChart;
        private int LastHighlightedIndex = -1;

        private System.Drawing.Color lightBlueColor = System.Drawing.Color.FromArgb(255, 31, 119, 180);
        private double[] damagePerHits;
        private double[] hitsIteration;
        private double damagePerHits_Max, damagePerHits_Min;

        private double[] damagePerTime;
        private double[] leftEdges;
        private double damagePerTime_Max, damagePerTime_Min;

        private double prevMouseCoordX = 0, prevMouseCoordY = 0;
        private double barrierDistancePx = Math.Pow(5, 2);

        private BarPlot barPlot_DamageByTime;

        private bool isHighlightPointEnabled = true;

        private NumberFormatInfo numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = " " };


        private string titleDamageTime = "Damage/Time";
        private string titleDamageHit = "Damage/Hit (sorted)";

        private long dps;
        private long avgHit;

        private const string name = "damageChartWindow";
        private WindowPositionService windowPositionService = null;
        private WindowSizeService windowSizeService = null;
        public bool IsStartupPositionUnknown { get; set; } = true;
        public DamageChartWindow()
        {
            InitializeComponent();
            InitLocalization();

            Chart_DamageByHits.RightClicked -= Chart_DamageByHits.DefaultRightClickEvent;
            Chart_DamageByTime.RightClicked -= Chart_DamageByTime.DefaultRightClickEvent;

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
        }

        private void InitLocalization()
        {
            TextBlock_Menu.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Menu");

            TextBlock_Menu_Option_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MenuOption_DamageTime");
            TextBlock_Menu_Option_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MenuOption_DamageHit");

            GeneralInfo_Title.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_GeneralInfo");
            GeneralInfo_Damage.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Damage");
            GeneralInfo_Time.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Time");
            GeneralInfo_Hits.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Hits");

            DamageTimeInfo_Title.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageTimeTitle");
            DamageTimeInfo_Dps.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Dps");
            DamageTimeInfo_MaxDPS.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MaxDps");
            DamageTimeInfo_MinDPS.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MinDps");
            DamageTimeInfo_DpsExplanation.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DpsExplanation");

            DamageHitInfo_Title.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageHitTitle");
            DamageHitInfo_Dph.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Dph");
            DamageHitInfo_Dph.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_Dph_Tooltip");
            DamageHitInfo_MaxHit.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MaxHit");
            DamageHitInfo_MinHit.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_MinHit");
            DamageHitInfo_DphExplanation.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DphExplanation");

            CheckBox_ShowDPS.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_ShowDps");
            CheckBox_ShowDPS.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DpsExplanation");
            CheckBox_ShowAvgHitDamage.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_ShowAvgHitDmg");
            CheckBox_ShowAvgHitDamage.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DphExplanation");
            CheckBox_ShowMax.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_ShowMax");
            CheckBox_ShowMin.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_ShowMin");
            CheckBox_ShowLegend.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_ShowLegend");
            CheckBox_EnableHighlightByCursor.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_EnableHighlightByCursor");
        }


        private bool isAwareEventsRegistered = false;
        public void Init(DamageSequence damageSequence, string mobName)
        {
            titleDamageTime = mobName + Environment.NewLine + UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageSlTime");
            titleDamageHit = mobName + Environment.NewLine + UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageSlHit");

            InitNumberFormat();
            System.Windows.Media.SolidColorBrush bgColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 49, 54, 58));
            Background = bgColor;
            RichTextBox_BasicInfo.Background = bgColor;
            RichTextBox_DamageTime.Background = bgColor;
            RichTextBox_DamageHit.Background = bgColor;

            dps = damageSequence.CalcDps();
            avgHit = damageSequence.CalcAverageHit();
            InitDamageTimeScatter(damageSequence);
            InitDamageHitScatter(damageSequence);


            GeneralInfo_Damage_Value.Text = damageSequence.CalcSumDamage().ToString("#,0", numberFormatInfo_FancyLong);
            GeneralInfo_Hits_Value.Text = damageSequence.GetHitsCount().ToString("#,0", numberFormatInfo_FancyLong);
            GeneralInfo_Time_Value.Text = (damageSequence.GetDamageDurationInMs() / 1000).ToString("#,0", numberFormatInfo_FancyLong);


            DamageTimeInfo_Dps_Value.Text = dps.ToString("#,0", numberFormatInfo_FancyLong);
            DamageTimeInfo_MaxDPS_Value.Text = ((long)damagePerTime_Max).ToString("#,0", numberFormatInfo_FancyLong);
            DamageTimeInfo_MinDPS_Value.Text = ((long)damagePerTime_Min).ToString("#,0", numberFormatInfo_FancyLong);


            DamageHitInfo_Dph_Value.Text = avgHit.ToString("#,0", numberFormatInfo_FancyLong);
            DamageHitInfo_MaxHit_Value.Text = ((long)damagePerHits_Max).ToString("#,0", numberFormatInfo_FancyLong);
            DamageHitInfo_MinHit_Value.Text = ((long)damagePerHits_Min).ToString("#,0", numberFormatInfo_FancyLong);

            //load default checkboxes data
            ignoreCheckedUncheckedEvent = false;
            string s;

            s = UncorRTDPS_Config.getConfigVal("charts_NotShowDps");
            CheckBox_ShowDPS.IsChecked = s == "0";

            s = UncorRTDPS_Config.getConfigVal("charts_NotShowDph");
            CheckBox_ShowAvgHitDamage.IsChecked = s == "0";

            s = UncorRTDPS_Config.getConfigVal("charts_NotShowMax");
            CheckBox_ShowMax.IsChecked = s == "0";

            s = UncorRTDPS_Config.getConfigVal("charts_NotShowMin");
            CheckBox_ShowMin.IsChecked = s == "0";

            s = UncorRTDPS_Config.getConfigVal("charts_NotShowLegend");
            CheckBox_ShowLegend.IsChecked = s == "0";

            s = UncorRTDPS_Config.getConfigVal("charts_NotEnableHighlightByCursor");
            CheckBox_EnableHighlightByCursor.IsChecked = s == "0";

            if (!isAwareEventsRegistered)
            {
                (this as ICloseAllWindowsButMainAware).Register_CloseAllWindowsButMainAware();
                (this as ICloseAllWindowsChartsAware).Register_CloseAllWindowsChartsAware();
                isAwareEventsRegistered = true;
            }
        }

        public void InitNumberFormat()
        {
            string nf = UncorRTDPS_Config.getConfigVal("numberFormat_ThrousandsSeparator");
            if (nf == null || nf.Length < 1 || nf.Equals("0"))
                nf = " ";
            numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = nf };
        }


        public void InitDamageHitScatter(DamageSequence damageSequence)
        {
            Chart_DamageByHits.Plot.Clear();
            Chart_DamageByHits.Plot.Style(ScottPlot.Style.Gray1);
            Chart_DamageByHits.Plot.Legend();

            Chart_DamageByHits.Plot.Title(titleDamageHit + Environment.NewLine + " ");

            damagePerHits = damageSequence.GetDamage();
            hitsIteration = new double[damagePerHits.Length];
            for (int i = 0; i < damagePerHits.Length; i++)
                hitsIteration[i] = i + 1;

            Array.Sort(damagePerHits);
            damageHitsScatter = Chart_DamageByHits.Plot.AddScatter(hitsIteration, damagePerHits, color: lightBlueColor, lineWidth: 2);
            damageHitsScatter.Label = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageSlHit");

            Chart_DamageByHits.Plot.XLabel(UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_JustHitsLowerCase"));
            Chart_DamageByHits.Plot.YLabel(UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_JustDamageLowerCase"));

            //dps horizontal line
            (chart_DamageByHit_HLine_AvgDmg, chart_DamageByHit_Text_AvgDmg) = AddDpsHLineToPlot(Chart_DamageByHits.Plot);

            //avg hit horizontal line
            (chart_DamageByHit_HLine_AvgHitDmg, chart_DamageByHit_Text_AvgHitDmg) = AddAvgHitDmgHLineToPlot(Chart_DamageByHits.Plot);

            //point for cursor search
            highlightedPoint_ByHitsChart = Chart_DamageByHits.Plot.AddPoint(0, 0);
            highlightedPoint_ByHitsChart.Color = System.Drawing.Color.Blue;
            highlightedPoint_ByHitsChart.MarkerSize = 10;
            highlightedPoint_ByHitsChart.MarkerShape = ScottPlot.MarkerShape.openCircle;
            highlightedPoint_ByHitsChart.IsVisible = false;

            //min/max

            damagePerHits_Max = damagePerHits.Max();
            damagePerHits_Min = damagePerHits.Min();

            string maxText = "Max. = " + damagePerHits_Max.ToString("#,0", numberFormatInfo_FancyLong);
            (chart_DamageByHits_HLine_Max, chart_DamageByHits_Text_Max, chart_DamageByHits_Arrows_Max) = AddMaxToPlot(Chart_DamageByHits.Plot, damagePerHits, hitsIteration, maxText, damagePerHits_Max, 0.001);

            string minText = "Min. = " + damagePerHits_Min.ToString("#,0", numberFormatInfo_FancyLong);
            (chart_DamageByHits_HLine_Min, chart_DamageByHits_Text_Min, chart_DamageByHits_Arrows_Min) = AddMinToPlot(Chart_DamageByHits.Plot, damagePerHits, hitsIteration, minText, damagePerHits_Min, 0.001);
        }

        public void InitDamageTimeScatter(DamageSequence damageSequence)
        {
            Chart_DamageByTime.Plot.Clear();
            Chart_DamageByTime.Plot.Style(ScottPlot.Style.Gray1);
            Chart_DamageByTime.Plot.Legend();

            Chart_DamageByTime.Plot.Title(titleDamageTime + Environment.NewLine + " ");

            damagePerTime = damageSequence.GetDamageGroupedIntoSeconds();

            leftEdges = new double[damagePerTime.Length];
            for (int i = 0; i < leftEdges.Length; i++)
                leftEdges[i] = i + 0.5 - 0.05 / 2.0;

            barPlot_DamageByTime = Chart_DamageByTime.Plot.AddBar(values: damagePerTime, positions: leftEdges);
            barPlot_DamageByTime.BarWidth = 1.05;
            barPlot_DamageByTime.FillColor = lightBlueColor;
            barPlot_DamageByTime.BorderLineWidth = 0;

            barPlot_DamageByTime.Label = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_DamageSlTime");

            Chart_DamageByTime.Plot.XAxis.Grid(false);
            Chart_DamageByTime.Plot.XLabel(UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_TimeAsAxisLabel"));
            Chart_DamageByTime.Plot.YLabel(UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_JustDamageLowerCase"));

            //dps horizontal line
            (chart_DamageByTime_HLine_AvgDmg, chart_DamageByTime_Text_AvgDmg) = AddDpsHLineToPlot(Chart_DamageByTime.Plot);

            //avg hit horizontal line
            (chart_DamageByTime_HLine_AvgHitDmg, chart_DamageByTime_Text_AvgHitDmg) = AddAvgHitDmgHLineToPlot(Chart_DamageByTime.Plot);

            //point for cursor search
            highlightedPoint_ByTimeChart = Chart_DamageByTime.Plot.AddPoint(0, 0);
            highlightedPoint_ByTimeChart.Color = System.Drawing.Color.Blue;
            highlightedPoint_ByTimeChart.MarkerSize = 10;
            highlightedPoint_ByTimeChart.MarkerShape = ScottPlot.MarkerShape.openCircle;
            highlightedPoint_ByTimeChart.IsVisible = false;

            //helper invisible scatter
            damageTimeScatter = Chart_DamageByTime.Plot.AddScatter(leftEdges, damagePerTime);
            damageTimeScatter.IsVisible = false;


            damagePerTime_Max = damagePerTime.Max();
            damagePerTime_Min = damagePerTime.Min();

            string maxText = "Max. = " + damagePerTime_Max.ToString("#,0", numberFormatInfo_FancyLong);
            (chart_DamageByTime_HLine_Max, chart_DamageByTime_Text_Max, chart_DamageByTime_Arrows_Max) = AddMaxToPlot(Chart_DamageByTime.Plot, damagePerTime, leftEdges, maxText, damagePerTime_Max, 0.001);

            string minText = "Min. = " + damagePerTime_Min.ToString("#,0", numberFormatInfo_FancyLong);
            (chart_DamageByTime_HLine_Min, chart_DamageByTime_Text_Min, chart_DamageByTime_Arrows_Min) = AddMinToPlot(Chart_DamageByTime.Plot, damagePerTime, leftEdges, minText, damagePerTime_Min, 0.001);
        }

        private string dpsLocale = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_JustDps");
        private (HLine, Text) AddDpsHLineToPlot(Plot plot)
        {
            HLine hLine = plot.AddHorizontalLine(dps, width: 2);
            string fancyDps = dpsLocale + " = " + dps.ToString("#,0", numberFormatInfo_FancyLong);
            hLine.Label = fancyDps;
            Text text = plot.AddText(fancyDps, 0, dps, color: hLine.Color);
            return (hLine, text);
        }

        private string dphLocale = UncorRTDPS_Localization.GetLocaleGuiVal("guiCharts_JustDph");
        private (HLine, Text) AddAvgHitDmgHLineToPlot(Plot plot)
        {
            System.Drawing.Color c = plot.Palette.GetColor(9);
            HLine hLine = plot.AddHorizontalLine(avgHit, width: 2, color: c);
            string fancyAvgHit = dphLocale + " = " + avgHit.ToString("#,0", numberFormatInfo_FancyLong);
            hLine.Label = fancyAvgHit;
            Text text = plot.AddText(fancyAvgHit, 0, avgHit, color: c);
            return (hLine, text);
        }

        /// <summary>
        /// List<ScatterPlot> - list of arrows pointing at MAX
        /// </summary>
        /// <param name="plot"></param>
        /// <returns>HLine, Text, List<ScatterPlot></returns>
        private (HLine, Text, List<ScatterPlot>) AddMaxToPlot(Plot plot, double[] ys, double[] xs, string text, double max, double errorInPrecisionAllowed)
        {
            HLine hLine = plot.AddHorizontalLine(max, color: System.Drawing.Color.White, width: 1);
            Text plotText = plot.AddText(text, 0, max, color: System.Drawing.Color.White);

            List<ScatterPlot> arrowsPointingAtMax = new List<ScatterPlot>();
            double lowerLimit = max - errorInPrecisionAllowed;
            double higherLimit = max + errorInPrecisionAllowed;
            for (int i = 0; i < xs.Length; i++)
            {
                if (IsDoubleBetween(ys[i], lowerLimit, higherLimit))
                {
                    arrowsPointingAtMax.Add(plot.AddArrow(xs[i], ys[i], xs[i], ys[i] + 1, color: System.Drawing.Color.White, lineWidth: 1));
                }
            }

            return (hLine, plotText, arrowsPointingAtMax);
        }

        private (HLine, Text, List<ScatterPlot>) AddMinToPlot(Plot plot, double[] ys, double[] xs, string text, double min, double errorInPrecisionAllowed)
        {
            HLine hLine = plot.AddHorizontalLine(min, color: System.Drawing.Color.White, width: 1);
            Text plotText = plot.AddText(text, 0, min, color: System.Drawing.Color.White);

            List<ScatterPlot> arrowsPointingAtMax = new List<ScatterPlot>();
            double lowerLimit = min - errorInPrecisionAllowed;
            double higherLimit = min + errorInPrecisionAllowed;
            for (int i = 0; i < xs.Length; i++)
            {
                if (IsDoubleBetween(ys[i], lowerLimit, higherLimit))
                {
                    arrowsPointingAtMax.Add(plot.AddArrow(xs[i], ys[i], xs[i], ys[i] - 1, color: System.Drawing.Color.White, lineWidth: 1));
                }
            }

            return (hLine, plotText, arrowsPointingAtMax);
        }

        private bool IsDoubleBetween(double val, double minLim, double maxLim)
        {
            if (val >= minLim && val <= maxLim)
                return true;
            return false;
        }

        private double MeasureSquareDistance(double x1, double y1, double x2, double y2)
        {
            return ((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2));
        }


        private void Button_Menu_Option_0_Click(object sender, RoutedEventArgs e)
        {
            if (Chart_DamageByTime.Visibility == Visibility.Visible)
                return;

            Chart_DamageByTime.Visibility = Visibility.Visible;
            Chart_DamageByHits.Visibility = Visibility.Collapsed;

            RichTextBox_DamageTime.Visibility = Visibility.Visible;
            RichTextBox_DamageHit.Visibility = Visibility.Collapsed;

            TextBlock_Arrow_0.Visibility = Visibility.Visible;
            TextBlock_Arrow_1.Visibility = Visibility.Collapsed;
        }


        private void Button_Menu_Option_1_Click(object sender, RoutedEventArgs e)
        {
            if (Chart_DamageByHits.Visibility == Visibility.Visible)
                return;

            Chart_DamageByHits.Visibility = Visibility.Visible;
            Chart_DamageByTime.Visibility = Visibility.Collapsed;

            RichTextBox_DamageHit.Visibility = Visibility.Visible;
            RichTextBox_DamageTime.Visibility = Visibility.Collapsed;

            TextBlock_Arrow_1.Visibility = Visibility.Visible;
            TextBlock_Arrow_0.Visibility = Visibility.Collapsed;
        }

        private void Chart_DamageByTime_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isHighlightPointEnabled)
                return;


            (double mouseCoordX, double mouseCoordY) = Chart_DamageByTime.GetMouseCoordinates();
            double xyRatio = Chart_DamageByTime.Plot.XAxis.Dims.PxPerUnit / Chart_DamageByTime.Plot.YAxis.Dims.PxPerUnit;

            double xRatio = Chart_DamageByTime.Plot.XAxis.Dims.PxPerUnit;
            double yRatio = Chart_DamageByTime.Plot.YAxis.Dims.PxPerUnit;
            double dist = MeasureSquareDistance(prevMouseCoordX, prevMouseCoordY, mouseCoordX * xRatio, mouseCoordY * yRatio);

            if (dist < barrierDistancePx)
            {
                return;
            }
            prevMouseCoordX = mouseCoordX * xRatio;
            prevMouseCoordY = mouseCoordY * yRatio;

            (double pointX, double pointY, int pointIndex) = damageTimeScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

            highlightedPoint_ByTimeChart.Xs[0] = pointX;
            highlightedPoint_ByTimeChart.Ys[0] = pointY;
            highlightedPoint_ByTimeChart.IsVisible = true;

            // render if the highlighted point changed
            if (LastHighlightedIndex != pointIndex)
            {
                Chart_DamageByTime.Plot.XAxis2.Label(
                    titleDamageTime +
                    Environment.NewLine +
                    "Dmg: " + ((long)damagePerTime[pointIndex]).ToString("#,0", numberFormatInfo_FancyLong) + "   t: " + (leftEdges[pointIndex] + (0.5 + 0.05 / 2.0)).ToString("#,0", numberFormatInfo_FancyLong)
                    );

                LastHighlightedIndex = pointIndex;
                Chart_DamageByTime.Render();
            }
            //$"Point index {pointIndex} at ({pointX}, {pointY})"

        }

        private void Chart_DamageByHits_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isHighlightPointEnabled)
                return;
            (double mouseCoordX, double mouseCoordY) = Chart_DamageByHits.GetMouseCoordinates();
            double xyRatio = Chart_DamageByHits.Plot.XAxis.Dims.PxPerUnit / Chart_DamageByHits.Plot.YAxis.Dims.PxPerUnit;

            double xRatio = Chart_DamageByHits.Plot.XAxis.Dims.PxPerUnit;
            double yRatio = Chart_DamageByHits.Plot.YAxis.Dims.PxPerUnit;
            double dist = MeasureSquareDistance(prevMouseCoordX, prevMouseCoordY, mouseCoordX * xRatio, mouseCoordY * yRatio);

            if (dist < barrierDistancePx)
            {
                return;
            }
            prevMouseCoordX = mouseCoordX * xRatio;
            prevMouseCoordY = mouseCoordY * yRatio;

            (double pointX, double pointY, int pointIndex) = damageHitsScatter.GetPointNearest(mouseCoordX, mouseCoordY, xyRatio);

            highlightedPoint_ByHitsChart.Xs[0] = pointX;
            highlightedPoint_ByHitsChart.Ys[0] = pointY;
            highlightedPoint_ByHitsChart.IsVisible = true;

            // render if the highlighted point changed
            if (LastHighlightedIndex != pointIndex)
            {
                Chart_DamageByHits.Plot.XAxis2.Label(
                    titleDamageHit +
                    Environment.NewLine +
                    "Damage: " + ((long)damagePerHits[pointIndex]).ToString("#,0", numberFormatInfo_FancyLong)
                    );

                LastHighlightedIndex = pointIndex;
                Chart_DamageByHits.Render();
            }
            //$"Point index {pointIndex} at ({pointX}, {pointY})"
        }

        private void UpdLegendVisibility(bool visibility)
        {
            Chart_DamageByTime.Plot.Legend(visibility);
            Chart_DamageByHits.Plot.Legend(visibility);
            Chart_DamageByTime.Render();
            Chart_DamageByHits.Render();
        }

        private void UpdDpsVisibility(bool visibility)
        {
            chart_DamageByTime_HLine_AvgDmg.IsVisible = visibility;
            chart_DamageByHit_HLine_AvgDmg.IsVisible = visibility;

            chart_DamageByTime_Text_AvgDmg.IsVisible = visibility;
            chart_DamageByHit_Text_AvgDmg.IsVisible = visibility;

            Chart_DamageByTime.Render();
            Chart_DamageByHits.Render();
        }

        private void UpdDphVisibility(bool visibility)
        {
            chart_DamageByTime_HLine_AvgHitDmg.IsVisible = visibility;
            chart_DamageByHit_HLine_AvgHitDmg.IsVisible = visibility;

            chart_DamageByTime_Text_AvgHitDmg.IsVisible = visibility;
            chart_DamageByHit_Text_AvgHitDmg.IsVisible = visibility;

            Chart_DamageByTime.Render();
            Chart_DamageByHits.Render();
        }

        private void UpdMinVisibility(bool visibility)
        {
            chart_DamageByTime_HLine_Min.IsVisible = visibility;
            chart_DamageByTime_Text_Min.IsVisible = visibility;
            chart_DamageByTime_Arrows_Min.ForEach(i => i.IsVisible = visibility);

            chart_DamageByHits_HLine_Min.IsVisible = visibility;
            chart_DamageByHits_Text_Min.IsVisible = visibility;
            chart_DamageByHits_Arrows_Min.ForEach(i => i.IsVisible = visibility);

            Chart_DamageByTime.Render();
            Chart_DamageByHits.Render();
        }

        private void UpdMaxVisibility(bool visibility)
        {
            chart_DamageByTime_HLine_Max.IsVisible = visibility;
            chart_DamageByTime_Text_Max.IsVisible = visibility;
            chart_DamageByTime_Arrows_Max.ForEach(i => i.IsVisible = visibility);

            chart_DamageByHits_HLine_Max.IsVisible = visibility;
            chart_DamageByHits_Text_Max.IsVisible = visibility;
            chart_DamageByHits_Arrows_Max.ForEach(i => i.IsVisible = visibility);

            Chart_DamageByTime.Render();
            Chart_DamageByHits.Render();
        }

        private void UpdHighlightByCursor(bool visibility)
        {
            this.isHighlightPointEnabled = visibility;
            if (visibility == false)
            {
                highlightedPoint_ByTimeChart.IsVisible = false;
                Chart_DamageByTime.Plot.Title(titleDamageTime + Environment.NewLine + " ");
                Chart_DamageByTime.Render();

                highlightedPoint_ByHitsChart.IsVisible = false;
                Chart_DamageByHits.Plot.Title(titleDamageHit + Environment.NewLine + " ");
                Chart_DamageByHits.Render();
            }
        }

        private void CheckBox_ShowDPS_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowDPS.IsChecked == true ? true : false;
            UpdDpsVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowDps", "0");
        }

        private void CheckBox_ShowDPS_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowDPS.IsChecked == true ? true : false;
            UpdDpsVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowDps", "1");
        }

        private void CheckBox_ShowAvgHitDamage_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowAvgHitDamage.IsChecked == true ? true : false;
            UpdDphVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowDph", "0");

        }

        private void CheckBox_ShowAvgHitDamage_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowAvgHitDamage.IsChecked == true ? true : false;
            UpdDphVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowDph", "1");
        }

        private void CheckBox_ShowMax_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowMax.IsChecked == true ? true : false;
            UpdMaxVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowMax", "0");
        }

        private void CheckBox_ShowMax_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowMax.IsChecked == true ? true : false;
            UpdMaxVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowMax", "1");
        }

        private void CheckBox_ShowMin_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowMin.IsChecked == true ? true : false;
            UpdMinVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowMin", "0");
        }

        private void CheckBox_ShowMin_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowMin.IsChecked == true ? true : false;
            UpdMinVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowMin", "1");
        }

        private void CheckBox_ShowLegend_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowLegend.IsChecked == true ? true : false;
            UpdLegendVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowLegend", "0");
        }

        private void CheckBox_ShowLegend_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_ShowLegend.IsChecked == true ? true : false;
            UpdLegendVisibility(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotShowLegend", "1");
        }

        private void CheckBox_EnableHighlightByCursor_Checked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_EnableHighlightByCursor.IsChecked == true ? true : false;
            UpdHighlightByCursor(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotEnableHighlightByCursor", "0");
        }

        private void CheckBox_EnableHighlightByCursor_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ignoreCheckedUncheckedEvent)
                return;
            bool visibility = CheckBox_EnableHighlightByCursor.IsChecked == true ? true : false;
            UpdHighlightByCursor(visibility);
            UncorRTDPS_Config.UpdateConfigVal("charts_NotEnableHighlightByCursor", "1");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double w = StackPanel_CheckBoxes.ActualWidth;
            RichTextBox_BasicInfo.Width = w;
            RichTextBox_DamageHit.Width = w;
            RichTextBox_DamageTime.Width = w;
        }


        public void Dispose()
        {
            TextBlock_Menu.Text = null;
            TextBlock_Arrow_0.Text = null;
            TextBlock_Arrow_1.Text = null;
            TextBlock_Menu_Option_0.Text = null;
            TextBlock_Menu_Option_1.Text = null;

            Button_Menu_Option_0.Content = null;
            Button_Menu_Option_1.Content = null;

            GeneralInfo_Damage_Value.Text = null;
            GeneralInfo_Hits_Value.Text = null;
            GeneralInfo_Time_Value.Text = null;
            GeneralInfo_Title.Text = null;

            DamageHitInfo_Dph.Text = null;
            DamageHitInfo_Dph_Value.Text = null;
            DamageHitInfo_MaxHit.Text = null;
            DamageHitInfo_MaxHit_Value.Text = null;
            DamageHitInfo_MinHit.Text = null;
            DamageHitInfo_MinHit_Value.Text = null;
            DamageHitInfo_Title.Text = null;

            DamageTimeInfo_Dps.Text = null;
            DamageTimeInfo_Dps_Value.Text = null;
            DamageTimeInfo_MaxDPS.Text = null;
            DamageTimeInfo_MaxDPS_Value.Text = null;
            DamageTimeInfo_MinDPS.Text = null;
            DamageTimeInfo_MinDPS_Value.Text = null;
            DamageTimeInfo_Title.Text = null;


            damageHitsScatter = null;
            damageTimeScatter = null;
            chart_DamageByTime_HLine_AvgDmg = null;
            chart_DamageByHit_HLine_AvgDmg = null;
            chart_DamageByHit_HLine_AvgHitDmg = null;
            chart_DamageByTime_HLine_AvgHitDmg = null;
            chart_DamageByTime_Text_AvgDmg = null;
            chart_DamageByHit_Text_AvgDmg = null;
            chart_DamageByHit_Text_AvgHitDmg = null;
            chart_DamageByTime_Text_AvgHitDmg = null;
            chart_DamageByTime_Arrows_Max.Clear();
            chart_DamageByTime_Arrows_Min.Clear();
            chart_DamageByHits_Arrows_Max.Clear();
            chart_DamageByHits_Arrows_Min.Clear();
            chart_DamageByTime_Text_Max = null;
            chart_DamageByTime_Text_Min = null;
            chart_DamageByHits_Text_Max = null;
            chart_DamageByHits_Text_Min = null;
            chart_DamageByTime_HLine_Max = null;
            chart_DamageByTime_HLine_Min = null;
            chart_DamageByHits_HLine_Max = null;
            chart_DamageByHits_HLine_Min = null;
            highlightedPoint_ByHitsChart = null;
            highlightedPoint_ByTimeChart = null;

            barPlot_DamageByTime = null;

            numberFormatInfo_FancyLong = null;

            titleDamageTime = null;
            titleDamageHit = null;
            Chart_DamageByHits.Plot.Clear();
            Chart_DamageByTime.Plot.Clear();

            windowPositionService = null;
            windowSizeService = null;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowPositionService.UpdateWindowPosition(name, new Point<double>(this.Left, this.Top));
            windowSizeService.UpdateWindowSize(name, new Size<double>(this.ActualWidth, this.ActualHeight));
            (this as ICloseAllWindowsButMainAware).Unregister_CloseAllWindowsButMainAware();
            (this as ICloseAllWindowsChartsAware).Unregister_CloseAllWindowsChartsAware();
            Dispose();
        }

        public void CloseAllWindowsButMainAware_HandleEvent(object sender, EventArgs e)
        {
            this.Close();
        }

        public void CloseAllWindowsChartsAware_HandleEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
