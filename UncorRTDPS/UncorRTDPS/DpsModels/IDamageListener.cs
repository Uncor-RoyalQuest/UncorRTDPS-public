
using System.Collections.Generic;

namespace UncorRTDPS.DpsModels
{
    public interface IDamageListener
    {
        public void FireDamage(List<long> damage, int length, long dmgTime);
    }
}
