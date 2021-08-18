using UncorRTDPS.DpsModels.TargetsDictionary;

namespace UncorRTDPS.DpsModels
{
    public class DamageTarget_DamageModel : ADamageModel
    {

        public Target target = null;
        
        //util
        public long msTimeSeparation = 5000;
        //

        public void AddDamage(long damage, long dmgTime)
        {
            if (IsDamageExpired(dmgTime) || resetAsked)
            {
                //assume it is new damage
                damageTotal = 0;
                hits = 0;
                damageTimeStart = dmgTime;
                resetAsked = false;
            }

            damageTotal += damage;
            hits += 1;
            damageTimeLast = dmgTime;
        }

        public void InitDamageTarget_DamageModel(Target target, long msTimeSeparation)
        {
            this.isActive = false;
            this.target = target;
            this.msTimeSeparation = msTimeSeparation;
            this.resetAsked = false;
        }

        public bool IsDamageExpired(long timeNow)
        {
            return timeNow - damageTimeLast > msTimeSeparation;
        }
    }
}
