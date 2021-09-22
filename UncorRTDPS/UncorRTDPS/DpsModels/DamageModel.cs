using System.IO;
using UncorRTDPS.DpsModels.TargetsDictionary;
using UncorRTDPS.Util.Serialization;

namespace UncorRTDPS.DpsModels
{
    public class DamageModel : ICustomBinarySerializable
    {

        private Target target = null;
        private bool isActive;
        private long damageTotal;
        private long damageTimeStart;
        private long damageTimeLast;
        private long hits;
        private long maxHitDmg;
        private bool resetAsked;
        private DamageSequence damageSequence = new DamageSequence();

        private Services.DamageHistory.RecentDamage recentDamageService = null;

        //util
        private long msTimeSeparation = 5000;
        //

        public Target Target { get { return target; } }
        public long TotalDamage { get { return damageTotal; } }
        public long TimeStart { get { return damageTimeStart; } }
        public long TimeLast { get { return damageTimeLast; } }
        public long Hits { get { return hits; } }
        public long MaxHitDmg { get { return maxHitDmg; } }
        public DamageSequence DamageSequence { get { return damageSequence; } }

        public long MsTimeSeparation { set { msTimeSeparation = value; } }

        public DamageModel()
        {
            try
            {
                recentDamageService = Services.ServicesContainer.GetService("recentDamage") as Services.DamageHistory.RecentDamage;
            }
            catch { }
        }

        public DamageModel(Target target, bool isActive, long damageTotal, long damageTimeStart, long damageTimeLast, long hits, long maxHitDmg, bool resetAsked, DamageSequence damageSequence, long msTimeSeparation)
        {
            this.target = target;
            this.isActive = isActive;
            this.damageTotal = damageTotal;
            this.damageTimeStart = damageTimeStart;
            this.damageTimeLast = damageTimeLast;
            this.hits = hits;
            this.maxHitDmg = maxHitDmg;
            this.resetAsked = resetAsked;
            this.damageSequence = damageSequence;
            this.msTimeSeparation = msTimeSeparation;
        }

        public void InitDamageModel(Target target, long msTimeSeparation)
        {
            this.isActive = false;
            this.target = target;
            this.msTimeSeparation = msTimeSeparation;
            this.resetAsked = false;
        }

        public void AddDamage(long damage, long dmgTime)
        {
            if (IsDamageExpired(dmgTime) || resetAsked)
            {
                if (hits > 0 && target != null)
                {
                    recentDamageService?.ReplaceNewDamageInRecent(this, this.Clone());
                }
                //assume it is new damage
                damageTotal = 0;
                hits = 0;
                maxHitDmg = 0;
                damageTimeStart = dmgTime;
                damageSequence.ClearDamageSequence();
                resetAsked = false;
            }

            if (hits == 0L)
            {
                damageTimeStart = dmgTime;
                damageTimeLast = dmgTime;
            }
            if (damage < 1L)
            {
                return;
            }

            damageTotal += damage;
            hits += 1;
            damageTimeLast = dmgTime;
            if (damage > maxHitDmg)
                maxHitDmg = damage;
            damageSequence.AddDamageToSequence(damage, dmgTime);
            if (hits == 1 && target != null)
            {
                recentDamageService?.AddNewDamageToRecent(this);
            }
        }

        public bool IsDamageExpired(long timeNow)
        {
            return timeNow - damageTimeLast > msTimeSeparation;
        }


        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        /// <summary>
        /// damageTimeLast - damageTimeStart (ms)
        /// </summary>
        /// <returns></returns>
        public long CalcDamageTime()
        {
            return damageTimeLast - damageTimeStart;
        }

        /// <summary>
        /// Calculates "on the fly"
        /// </summary>
        /// <returns></returns>
        public long CalcDps()
        {
            double t = (double)(damageTimeLast - damageTimeStart) / 1000;
            if (t == 0.0)
                t = 1.0;
            return (long)(damageTotal / t);
        }

        public void AskForReset()
        {
            resetAsked = true;
        }

        public bool IsResetAsked()
        {
            return resetAsked;
        }

        public DamageModel Clone()
        {
            return new DamageModel(target, isActive, damageTotal, damageTimeStart, damageTimeLast, hits, maxHitDmg, resetAsked, damageSequence.Clone(), msTimeSeparation);
        }

        public void ReadObject(BinaryReader binaryReader)
        {
            string searchFriendlyOriginalTargetName = binaryReader.ReadString();
            target = TargetsDictionary.TargetsDictionary.TryGetTargetFromSimpleDictionary(searchFriendlyOriginalTargetName);

            damageTotal = binaryReader.ReadInt64();
            damageTimeStart = binaryReader.ReadInt64();
            damageTimeLast = binaryReader.ReadInt64();
            hits = binaryReader.ReadInt64();
            maxHitDmg = binaryReader.ReadInt64();

            damageSequence.ReadObject(binaryReader);
        }

        public void WriteObject(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(target?.searchFriendlyOriginalName);

            binaryWriter.Write(damageTotal);
            binaryWriter.Write(damageTimeStart);
            binaryWriter.Write(damageTimeLast);
            binaryWriter.Write(hits);
            binaryWriter.Write(maxHitDmg);

            damageSequence.WriteObject(binaryWriter);
        }
    }
}
