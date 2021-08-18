

namespace UncorRTDPS.UncorOCR.DamageOCR_Data
{
    public class DamageTarget
    {
        public long damage;
        public string target;

        public DamageTarget(int damage, string target)
        {
            this.damage = damage;
            this.target = target;
        }
    }
}
