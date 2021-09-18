using System;
using System.Collections.Generic;

namespace UncorRTDPS.UncorOCR.BufferedBitmaps
{
    public class BufferedBmps : IDisposable
    {
        protected List<BufferedBitmapPack> bmpPacks;

        public void addBmpPack(BufferedBitmapPack p)
        {
            if (bmpPacks == null)
            {
                bmpPacks = new List<BufferedBitmapPack>();
            }

            bmpPacks.Add(p);
        }

        public BufferedBitmapPack GetBmpPack(int i)
        {
            if (i < 0 || i >= bmpPacks.Count)
                return null;

            return bmpPacks[i];
        }

        public void Dispose()
        {
            if (bmpPacks != null)
            {
                foreach (BufferedBitmapPack p in bmpPacks)
                {
                    p.Dispose();
                }
                bmpPacks.Clear();
                bmpPacks = null;
            }
        }

    }
}
