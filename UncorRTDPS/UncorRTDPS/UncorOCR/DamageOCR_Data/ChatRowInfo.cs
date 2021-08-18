

namespace UncorRTDPS.UncorOCR.DamageOCR_Data
{
    public class ChatRowInfo
    {
        public bool ignoreThisRow = false;
        public int posStart;
        public int posEnd;
        public int height;

        //
        public int damageHorizPosStart = 0;
        public int damageHorizPosEnd = 0;
        public int damageHorizWidth = 0;

        public int targetHorizStart = 0;
        public int targetHorizEnd = 0;
        public int targetHorizWidth = 0;

        //
        public int wordDamageHorizEnd = 0;
    }
}
