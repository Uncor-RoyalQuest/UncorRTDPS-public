using System;
using System.Collections.Generic;

namespace UncorRTDPS.UncorOCR.BufferedBitmaps
{
    public class BufferedBmps_Catalog : IDisposable
    {
        private List<BufferedBmps_WidthScaleOnly> catalogByHeight;
        private List<int> heightsInCatalog;

        private int minHeight, heightStep, heightStepCount;
        private int minWidth, widthStep, widthStepCount;

        public void CreateCatalogByHeights(int minHeight, int heightStep, int heightStepCount, int minWidth, int widthStep, int widthStepCount)
        {
            this.Dispose();

            catalogByHeight = new List<BufferedBmps_WidthScaleOnly>();
            heightsInCatalog = new List<int>();

            int currHeight = minHeight;
            for (int i=0; i<heightStepCount; i++)
            {
                currHeight = minHeight + (heightStep * i);

                catalogByHeight.Add(new BufferedBmps_WidthScaleOnly());
                heightsInCatalog.Add(currHeight);
            }

            for (int i = 0; i<catalogByHeight.Count; i++)
            {
                catalogByHeight[i].CreateBmps(minWidth, heightsInCatalog[i], widthStep, widthStepCount);
            }

            this.minHeight = minHeight;
            this.heightStep = heightStep;
            this.heightStepCount = heightStepCount;

            this.minWidth = minWidth;
            this.widthStep = widthStep;
            this.widthStepCount = widthStepCount;
        }

        public BufferedBitmapPack GetFittingBitmap(int w, int h)
        {
            //1. seek through height catalog

            int iW = ((w - minWidth) / widthStep) + 1;
            int iH = ((h - minHeight) / heightStep) + 1;

            if (iW < 0 || iH < 0)
                return null;

            if (iH >= catalogByHeight.Count)
                return null;

            return catalogByHeight[iH].GetBmpPack(iW);
        }

        public void Dispose()
        {
            if (catalogByHeight != null)
            {
                foreach (BufferedBmps_WidthScaleOnly b in catalogByHeight)
                {
                    b.Dispose();
                }
                catalogByHeight.Clear();
                catalogByHeight = null;
            }

            if (heightsInCatalog!=null)
            {
                heightsInCatalog.Clear();
                heightsInCatalog = null;
            }
        }
    }
}
