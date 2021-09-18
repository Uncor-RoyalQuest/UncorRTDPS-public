namespace UncorRTDPS.UncorOCR
{
    public class Supplementary_Parameters_DamageOCR
    {
        public int AverageCharacterWidth { get; private set; } = -1;
        public int GarbageWidthAfterDamage { get; private set; } = -1;

        public int DamageWordWidth { get; private set; } = -1; //sub of garbageWidthAfterDamage
        public int TargetWordWidth { get; private set; } = -1; //sub of garbageWidthAfterDamage


        public int MaximumRowHeight { get; private set; } = -1;
        public int AverageWhiteSpaceBetweenRows { get; private set; } = -1;

        public int VerticallyDataStart { get; private set; } = -1;

        public int BitmapHeight { get; private set; } = -1;
        public int BitmapWidth { get; private set; } = -1;

        public Supplementary_Parameters_DamageOCR(
            int averageCharacterWidth,
            int garbageWidthAfterDamage,
            int maximumRowHeight,
            int averageWhiteSpaceBetweenRows,
            int damageWordWidth,
            int targetWordWidth,
            int verticallyDataStart,
            int bitmapHeight,
            int bitmapWidth)
        {
            this.AverageCharacterWidth = averageCharacterWidth;
            this.GarbageWidthAfterDamage = garbageWidthAfterDamage;

            this.MaximumRowHeight = maximumRowHeight;
            this.AverageWhiteSpaceBetweenRows = averageWhiteSpaceBetweenRows;

            this.DamageWordWidth = damageWordWidth;
            this.TargetWordWidth = targetWordWidth;

            this.VerticallyDataStart = verticallyDataStart;

            this.BitmapHeight = bitmapHeight;
            this.BitmapWidth = bitmapWidth;
        }

        public bool CheckParamsValid()
        {
            if (AverageCharacterWidth > 0 &&
                GarbageWidthAfterDamage > 0 &&
                DamageWordWidth > 0 &&
                TargetWordWidth > 0 &&
                MaximumRowHeight > 0 &&
                AverageWhiteSpaceBetweenRows > 0 &&
                VerticallyDataStart > 0 &&
                BitmapHeight > 0 &&
                BitmapWidth > 0)
                return true;
            return false;
        }
    }
}
