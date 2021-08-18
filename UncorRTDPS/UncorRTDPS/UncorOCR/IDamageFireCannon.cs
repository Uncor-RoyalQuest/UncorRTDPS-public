using System.Collections.Generic;
using UncorRTDPS.DpsModels;

namespace UncorRTDPS.UncorOCR
{
    public interface IDamageFireCannon
    {
        public void RegisterDamageListener(IDamageListener damageListener);
        public void RemoveDamageListener(IDamageListener damageListener);
        public void FireDamageToAllDamageListeners(List<long> damage, int length);
    }
}
