

namespace UncorRTDPS.Statistics
{
    public class Statistics_TimeConsumptionPerEvent
    {
        private long totalDuration;
        private double avgLastDuration;

        private int eventsCount;

        private long eventTimeStart = 0;
        private long eventTimeLast = 0;


        private long option_RefreshDelay = 3000;

        public Statistics_TimeConsumptionPerEvent(long option_RefreshDelay)
        {
            this.option_RefreshDelay = option_RefreshDelay;
        }

        public void AddEvent(int eventCount, long duration, long eventTime)
        {
            if (eventTimeLast - eventTimeStart > option_RefreshDelay)
            {
                //assume new
                this.avgLastDuration = (double)totalDuration / (eventsCount == 0 ? 1 : eventsCount);

                this.totalDuration = 0;
                this.eventsCount = 0;
                this.eventTimeStart = eventTime;
            }

            this.totalDuration += duration;
            this.eventsCount += eventCount;
            this.eventTimeLast = eventTime;
        }

        public double GetAvgDuration()
        {
            return avgLastDuration;
        }
    }
}
