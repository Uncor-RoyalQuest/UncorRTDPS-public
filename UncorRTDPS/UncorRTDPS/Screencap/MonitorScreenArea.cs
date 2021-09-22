using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using UncorRTDPS.Screencap;
using UncorRTDPS.Statistics;
using UncorRTDPS.UncorOCR;
using UncorRTDPS.Util;

namespace UncorRTDPS
{
    public class MonitorScreenArea
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        private List<IBitmapUpdateListener> bitmapListeners = new List<IBitmapUpdateListener>();
        private BackgroundWorker backgroundWorker;
        public bool IsWorking { get; set; } = false;

        //statistics
        private Statistics_RefreshRate statistics_RefreshRate = new Statistics_RefreshRate(3000);
        public Statistics_RefreshRate Statistics_RefreshRate { get { return statistics_RefreshRate; } }

        private Statistics_TimeConsumptionPerEvent statistics_TimeConsumptionPerEvent = new Statistics_TimeConsumptionPerEvent(3000);
        public Statistics_TimeConsumptionPerEvent Statistics_TimeConsumptionPerEvent { get { return statistics_TimeConsumptionPerEvent; } }
        //
        private double rpsLimit_Active;
        private double rpsLimit_Inactive;
        private long switchTimespanActiveInactive = 30000;

        public MonitorScreenArea()
        {
            X = 0;
            Y = 0;
            Width = 0;
            Height = 0;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = false;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        public MonitorScreenArea(int x, int y, int width, int height) : this()
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public MonitorScreenArea(SelectedArea sa) : this()
        {
            SetAreaOfMonitoring(sa);
        }

        public void SetAreaOfMonitoring(SelectedArea sa)
        {
            if (sa.IsTopLeftSet && sa.IsBotRightSet)
            {
                X = sa.X_topLeft;
                Y = sa.Y_topLeft;
                Width = sa.Width;
                Height = sa.Height;
            }
            else
            {
                X = 0;
                Y = 0;
                Width = 0;
                Height = 0;
            }
        }

        public int StartMonitoringThread()
        {
            if (IsWorking || Width < 1 || Height < 1)
                return -1;

            IsWorking = true;
            PrepareForMonitoringTask();
            backgroundWorker.RunWorkerAsync();
            return 0;
        }

        public int StopMonitoringThread()
        {
            if (!IsWorking)
                return -1;
            backgroundWorker.CancelAsync();
            return 0;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            long timeStart, timeEnd;
            int monitoringTaskRes;
            long lastTimeConsumed_AllOk = 0;
            long sleepFor;

            double currentRpsLimit;
            long timeLastActive = 0;
            while (true)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }

                timeStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                monitoringTaskRes = MonitoringTask();
                timeEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                statistics_TimeConsumptionPerEvent.AddEvent(1, timeEnd - timeStart, timeEnd);

                /// "0" - all ok, function fully executed
                /// "2" - damage now and prev is equal
                /// "-2" - 0 valid rows
                /// "-3" - cant find fitting bitmap for last row
                /// "-4" - cant parse damage of the last row
                /// "-5" - cant find fitting bitmap for row[i]
                /// "-6" - no new damage, only old
                /// "-7" - cant get valid row start for damage
                /// "-8" - cant find fitting bitmap for target[i]

                if (monitoringTaskRes == 0)
                {
                    timeLastActive = timeEnd;
                }

                if (timeEnd - timeLastActive > switchTimespanActiveInactive)
                {
                    currentRpsLimit = rpsLimit_Inactive;
                }
                else
                {
                    currentRpsLimit = rpsLimit_Active;
                }

                lastTimeConsumed_AllOk = timeEnd - timeStart;


                sleepFor = (long)(1000.0 / currentRpsLimit) - lastTimeConsumed_AllOk;
                if (sleepFor > 0)
                {
                    Thread.Sleep((int)sleepFor);
                }

                statistics_RefreshRate.AddEventsCount(1, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            }
        }

        private Bitmap bitmap;
        private Graphics g;
        private Size sizeOfArea;

        private void PrepareForMonitoringTask()
        {
            if (g != null)
            {
                g.Dispose();
            }
            if (bitmap != null)
            {
                bitmap.Dispose();
            }

            bitmap = new Bitmap(Width, Height);
            g = Graphics.FromImage(bitmap);

            sizeOfArea = new Size(Width, Height);

            rpsLimit_Active = RTDPS_Settings.UncorRTDPS_StaticSettings.RpsMonitoringLimit_Active;
            rpsLimit_Inactive = RTDPS_Settings.UncorRTDPS_StaticSettings.RpsMonitoringLimit_Inactive;

            long activeInactiveSwitchTimespan = SLong.FromString(RTDPS_Settings.UncorRTDPS_Config.GetConfigVal("monitoring_ActiveInactiveSwitchTimespan")) ?? 30000;
            if (activeInactiveSwitchTimespan < 1)
                activeInactiveSwitchTimespan = 30000;

            switchTimespanActiveInactive = activeInactiveSwitchTimespan;
        }

        private int MonitoringTask()
        {
            g.CopyFromScreen(X, Y, 0, 0, sizeOfArea);

            return NotifyBitmapUpdateListeners(bitmap);
        }

        private void FreeMonitoringResources()
        {
            if (g != null)
            {
                g.Dispose();
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                //
            }
            else if (e.Error != null)
            {
                //
            }
            else
            {
                //
            }

            FreeMonitoringResources();
            IsWorking = false;
        }

        public void SubscribeBitmapUpdate(IBitmapUpdateListener bul)
        {
            bitmapListeners.Add(bul);
        }

        public void UnsubscribeBitmapUpdate(IBitmapUpdateListener bul)
        {
            bitmapListeners.Remove(bul);
        }

        private int NotifyBitmapUpdateListeners(Bitmap bmp)
        {
            //long timeStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            int lastRes = -100;
            foreach (IBitmapUpdateListener bul in bitmapListeners)
                lastRes = bul.FireBitmap(bmp);
            //long timeEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return lastRes;
        }
    }
}
