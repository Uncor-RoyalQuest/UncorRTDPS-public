
namespace UncorRTDPS.DpsModels
{
    public abstract class ADamageModel
    {
        protected bool isActive;
        protected long damageTotal;
        protected long damageTimeStart;
        protected long damageTimeLast;
        protected long hits;
        protected bool resetAsked;

        public bool IsActive 
        {
            get { return isActive; }
            set { isActive = value; }
        }

        public long GetTotalDamage()
        {
            return damageTotal;
        }

        public long GetDamageTime()
        {
            return damageTimeLast - damageTimeStart;
        }

        public long GetHits()
        {
            return hits;
        }

        /// <summary>
        /// Calculates "on the fly"
        /// </summary>
        /// <returns></returns>
        public long GetDps()
        {
            double t = (double)(damageTimeLast - damageTimeStart) / 1000;
            if (t == 0.0)
                t = 1.0;
            return (long)(damageTotal / t);
        }

        public long GetLastDamageTime()
        {
            return damageTimeLast;
        }

        public void AskForReset()
        {
            resetAsked = true;
        }

        public bool IsResetAsked()
        {
            return resetAsked;
        }
    }
}
