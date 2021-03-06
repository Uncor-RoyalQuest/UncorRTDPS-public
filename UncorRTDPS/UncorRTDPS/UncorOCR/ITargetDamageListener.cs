using System.Collections.Generic;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.UncorOCR
{
    public interface ITargetDamageListener
    {
        public void RecieveTargetDamage(List<DamageTarget> damageTargets, int posStart, int posEnd, long dmgTime);
    }
}
