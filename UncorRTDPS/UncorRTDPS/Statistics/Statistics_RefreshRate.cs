
namespace UncorRTDPS.Statistics
{
    public class Statistics_RefreshRate
    {
        private int eventsCount;
        private double lastRefreshRatePerSecond;

        private long eventTimeStart=0;
        private long eventTimeLast=0;

        private long option_RefreshDelay = 3000;

        public Statistics_RefreshRate(long option_RefreshDelay)
        {
            this.option_RefreshDelay = option_RefreshDelay;
        }

        public void AddEventsCount(int eventsCount, long eventTime)
        {
            if (eventTimeLast - eventTimeStart > option_RefreshDelay)
            {
                //assume new
                long lastTimeDur = eventTimeLast - eventTimeStart;
                if (lastTimeDur == 0)
                    lastTimeDur = 1;
                lastRefreshRatePerSecond = (double)(1000 * this.eventsCount) / lastTimeDur;

                this.eventsCount = 0;
                eventTimeStart = eventTime;
                eventTimeLast = eventTime;
            }
            else
            {
                this.eventsCount += eventsCount;
                eventTimeLast = eventTime;
            }
        }

        public double GetRefreshRate()
        {
            return lastRefreshRatePerSecond;
        }
        
    }
}
