using System.Collections.Generic;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.UncorOCR
{
    public interface ITargetDamageFireCannon
    {
        public void RegisterTargetDamageListener(ITargetDamageListener damageListener);
        public void RemoveTargetDamageListener(ITargetDamageListener damageListener);
        public void FireDamageToAllTargetDamageListeners(List<DamageTarget> damageTargets, int posStart, int posEnd, long dmgTime);
    }
}
