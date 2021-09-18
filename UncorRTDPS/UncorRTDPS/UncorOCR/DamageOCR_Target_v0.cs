using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Tesseract;
using UncorRTDPS.DpsModels;
using UncorRTDPS.FastBitmap;
using UncorRTDPS.UncorOCR.BufferedBitmaps;
using UncorRTDPS.UncorOCR.DamageOCR_Data;
using UncorRTDPS.UncorOCR.Transformations;

namespace UncorRTDPS.UncorOCR
{
    public class DamageOCR_Target_v0 : EngineOCR, IBitmapUpdateListener, ITargetDamageFireCannon, IBitmapFireCannon, IDisposable
    {
        private double failureRecognitionProportion; //upd every 5 sec.
        private double lossesDamageConcatProportion; //upd every 5 sec.
        private double averageDamageLoad; //upd every 5 sec
        private double maxDamageLoad; //resets every 5 sec

        public double FailureRecognitionProportion { get { return failureRecognitionProportion; } }
        public double LossesDamageConcatProportion { get { return lossesDamageConcatProportion; } }
        public double AverageDamageLoad { get { return averageDamageLoad; } }
        public double MaxDamageLoad { get { return maxDamageLoad; } }

        //failure and losses stats util 
        private long lastStatUpdTime = 0;

        private long failedRecognitionsCount = 0;
        private long allRecognitionsCount = 0;

        private long lossConcatCount = 0;
        private long allConcatCount = 0;

        private double sumALoadProportion = 0;
        private int numberInSumLoadProportion = 0;
        //

        //
        private Bitmap bmpResized = null;
        private FastBitmap.FastBitmap bmpResizedFast = null;
        private Graphics bmpResizedGraphics = null;
        //

        //settings
        private float brightnessBarrier = 0;
        private float resizeScale = 0;

        private int avgCharWidth = 0;
        private int garbageAfterDamageWidth = 0;
        private int maxRowHeight = 0;
        private int avgSpaceBetweenRows = 0;
        private int verticallyDataStart = 0;
        private int damageWordWidth = 0;
        private int damageWordCharsCount = 0;

        //
        private long prevLastDmg = -1;

        //
        private Brush solidBrushWhite = new SolidBrush(Color.White);

        //
        //private int vagueMaximumPossibleRows = -1;
        //
        //optimization
        private BufferedBmps_WidthScaleOnly singleDamageBufferedBmps;
        private BufferedBmps_WidthScaleOnly singleTargetNameBufferedBmps;

        //


        public bool UpdateSettingsOCR(OCRSettings ocrSettings, Supplementary_Parameters_DamageOCR supplementary)
        {
            this.brightnessBarrier = ocrSettings.OCR_brightnessBarrier;
            this.resizeScale = ocrSettings.OCR_imageScaling;

            this.avgCharWidth = supplementary.AverageCharacterWidth;
            this.garbageAfterDamageWidth = supplementary.GarbageWidthAfterDamage;
            this.maxRowHeight = supplementary.MaximumRowHeight;
            this.avgSpaceBetweenRows = supplementary.AverageWhiteSpaceBetweenRows;
            this.verticallyDataStart = supplementary.VerticallyDataStart;
            this.damageWordWidth = supplementary.DamageWordWidth;

            switch (ocrSettings.Lang)
            {
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian:
                    damageWordCharsCount = 5;
                    break;
                case RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English:
                    damageWordCharsCount = 6;
                    break;
            }

            if (avgCharWidth <= 1 || garbageAfterDamageWidth <= 1)
                return false;


            if (bmpResizedFast != null)
            {
                bmpResizedFast.Dispose();
            }
            bmpResizedFast = null;

            if (bmpResizedGraphics != null)
            {
                bmpResizedGraphics.Dispose();
            }
            bmpResizedGraphics = null;

            if (bmpResized != null)
            {
                bmpResized.Dispose();
            }
            bmpResized = null;

            failedRecognitionsCount = 0;
            allRecognitionsCount = 0;

            //create buffered optimized bitmaps
            //clear if existed
            if (singleDamageBufferedBmps != null)
            {
                singleDamageBufferedBmps.Dispose();
            }
            singleDamageBufferedBmps = new BufferedBmps_WidthScaleOnly();
            singleDamageBufferedBmps.CreateBmps(avgCharWidth, maxRowHeight + (avgSpaceBetweenRows * 2), avgCharWidth / 2, 20);

            //create buffered optimized bmps for target name
            if (singleTargetNameBufferedBmps != null)
            {
                singleTargetNameBufferedBmps.Dispose();
            }
            singleTargetNameBufferedBmps = new BufferedBmps_WidthScaleOnly();
            singleTargetNameBufferedBmps.CreateBmps(avgCharWidth, maxRowHeight + (avgSpaceBetweenRows * 3), avgCharWidth * 3, 15);

            return true;
        }

        Transformations_DamageOCR_Target transformations_DamageOCR_Target = new Transformations_DamageOCR_Target();

        public void UpdStats()
        {
            failureRecognitionProportion = (double)failedRecognitionsCount / (allRecognitionsCount == 0 ? 1 : allRecognitionsCount);
            failedRecognitionsCount = 0;
            allRecognitionsCount = 0;

            lossesDamageConcatProportion = (double)lossConcatCount / (allConcatCount == 0 ? 1 : allConcatCount);
            lossConcatCount = 0;
            allConcatCount = 0;

            averageDamageLoad = sumALoadProportion / (numberInSumLoadProportion == 0 ? 1 : numberInSumLoadProportion);
            sumALoadProportion = 0.0;
            numberInSumLoadProportion = 0;

            maxDamageLoad = 0;
        }

        /// <summary>
        /// "0" - all ok, function fully executed
        /// "2" - damage now and prev are equal
        /// "-2" - 0 valid rows
        /// "-3" - cant find fitting bitmap for last row
        /// "-4" - cant parse damage of the last row
        /// "-5" - cant find fitting bitmap for row[i]
        /// "-6" - no new damage, only old
        /// "-7" - cant get valid row start for damage
        /// "-8" - cant find fitting bitmap for target[i]
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public int ProcessDamageRecognition(Bitmap bmp)
        {
            long statTimeStart;
            statTimeStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (statTimeStart - lastStatUpdTime > 5000)
            {
                UpdStats();
                lastStatUpdTime = statTimeStart;
            }


            if (bmpResized == null)
            {
                bmpResized = new Bitmap((int)(bmp.Width * resizeScale), (int)(bmp.Height * resizeScale), PixelFormat.Format32bppArgb);
                bmpResizedFast = new FastBitmap.FastBitmap(bmpResized);
                bmpResizedGraphics = Graphics.FromImage(bmpResized);
                bmpResizedGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            }

            Transformations_Size.ResizeImage(bmp, bmpResizedFast.Width, bmpResizedFast.Height, bmpResizedGraphics);

            bmpResizedFast.Lock();
            //1. Make black-white and remove garbage in the beginning and in the end
            BitmapTransformations.MakeBlackWhite(bmpResizedFast, brightnessBarrier);
            //optimized function. Might switch to full row check?
            //optimization point: check only a first character by width 
            int posCheckEndVertGarb = verticallyDataStart + (avgCharWidth * 5);
            if (posCheckEndVertGarb > bmpResizedFast.Width)
                posCheckEndVertGarb = bmpResizedFast.Width;
            transformations_DamageOCR_Target.RemoveGarbagedRows_ManualAreaForCheck(bmpResizedFast, verticallyDataStart, posCheckEndVertGarb);

            //2. Find start/end for each row
            transformations_DamageOCR_Target.FindAllRowsInfo(bmpResizedFast, verticallyDataStart + avgCharWidth, verticallyDataStart, 1);

            if (transformations_DamageOCR_Target.currentlyValidRowsCount < 1)
            {
                //throw new Exception("[Error] No valid rows");
                bmpResizedFast.Unlock();
                return -2;
            }

            //3. Define last damage position
            //find last row
            ChatRowInfo lastRow = transformations_DamageOCR_Target.GetLastValidRowInfo();
            transformations_DamageOCR_Target.DefineDamageStartEndForRow(lastRow, bmpResizedFast, avgCharWidth, 5, damageWordWidth);

            //4. Move the last damage to a bmp with a fitting size for tesseract
            bmpResizedFast.Unlock();
            int fittingBmpInd = singleDamageBufferedBmps.GetIndexOfFittingBitmap(lastRow.damageHorizPosEnd, lastRow.height);
            if (fittingBmpInd < 0)
            {
                //throw new Exception("[Error] Cant find fitting bitmap. Probably error in the last row params");
                return -3;
            }

            BufferedBitmapPack bufferedBitmapPack = singleDamageBufferedBmps.GetBmpPack(fittingBmpInd);
            DrawDamageOnFittingBitmap(bmpResized, bufferedBitmapPack, lastRow);

            //5. Tesseract the last damage
            string dmgLastRow;
            dmgLastRow = OcrStringOneWordFromBitmap(bufferedBitmapPack.bitmap);
            allRecognitionsCount += 1;


            long int_dmgLastRow = ParseDamageFromString(dmgLastRow);
            if (int_dmgLastRow == -1)
            {
                failedRecognitionsCount += 1;
                return -4;
            }

            //6. Compare dmg with prev
            if (prevLastDmg == int_dmgLastRow)
            {
                return 2;
            }
            prevLastDmg = int_dmgLastRow;

            //7. Define damage pos for other rows
            bmpResizedFast.Lock();
            transformations_DamageOCR_Target.DefineDamageStartEndForEachRow_Except(bmpResizedFast, avgCharWidth, 5, damageWordWidth, lastRow);
            bmpResizedFast.Unlock();

            //8. Tesseract damage row by row
            int posFillDmgArr = 0;

            //expand damageTargetPair if needed
            if (damageTargetPair.Count < transformations_DamageOCR_Target.currentlyValidRowsCount)
            {
                while (damageTargetPair.Count < transformations_DamageOCR_Target.currentlyValidRowsCount)
                {
                    damageTargetPair.Add(new DamageTarget(-1, null));
                }
            }
            damageTargetPair_currLen = transformations_DamageOCR_Target.currentlyValidRowsCount;
            //check if valid rows > 1 then go for 8.
            if (transformations_DamageOCR_Target.currentlyValidRowsCount != 1)
            {
                lastRow.ignoreThisRow = true;

                //iterate through all rows info
                ChatRowInfo rowInfo;
                for (int i = 0; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
                {
                    rowInfo = transformations_DamageOCR_Target.chatRowsInfo[i];
                    if (rowInfo.ignoreThisRow)
                        continue;

                    BufferedBitmapPack bufferedBitmapPack_DamageBmp = singleDamageBufferedBmps.GetFittingBitmap(rowInfo.damageHorizWidth, rowInfo.height);
                    if (bufferedBitmapPack_DamageBmp == null)
                    {
                        //throw new Exception("[Error] Cant find fitting bitmap. Probably error in the last row params");
                        return -5;
                    }
                    DrawDamageOnFittingBitmap(bmpResized, bufferedBitmapPack_DamageBmp, rowInfo);

                    string ocrResStr = OcrStringOneWordFromBitmap(bufferedBitmapPack_DamageBmp.bitmap);
                    allRecognitionsCount += 1;

                    long damage = ParseDamageFromString(ocrResStr);
                    damageTargetPair[posFillDmgArr].damage = damage;
                    if (damage == -1)
                        failedRecognitionsCount += 1;

                    posFillDmgArr += 1;
                }

                lastRow.ignoreThisRow = false;
            }

            //9. Write last damage to the end of the array
            damageTargetPair[posFillDmgArr].damage = int_dmgLastRow;

            //10. Send dmg arr to dmg history and get index of the start of "new damage"
            int newDmgIndexStart = damageHistory.GetIndexOfStartNewDamage(damageTargetPair, damageTargetPair_currLen);
            allConcatCount += 1;
            if (newDmgIndexStart == 0 && damageHistory.LastDamageArrLen > 2)
            {
                lossConcatCount += 1;
            }

            if (newDmgIndexStart == -1)
            {
                //no new dmg
                return -6;
            }

            if (newDmgIndexStart >= damageTargetPair_currLen)
            {
                //no new dmg
                return -6;
            }

            double loadOfThisDamage = (double)(damageTargetPair_currLen - newDmgIndexStart) / damageTargetPair_currLen;
            sumALoadProportion += loadOfThisDamage;
            numberInSumLoadProportion += 1;
            if (loadOfThisDamage > maxDamageLoad)
            {
                maxDamageLoad = loadOfThisDamage;
            }

            //11. Define target pos for the "new damage rows"
            int validI = transformations_DamageOCR_Target.GetIndexOfTheIValidElemFromChatRowsInfo(newDmgIndexStart);
            if (validI == -1)
            {
                //unexpected error
                return -7;
            }
            ChatRowInfo rowForDefineTarget;
            bmpResizedFast.Lock();
            for (int i = validI; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
            {
                rowForDefineTarget = transformations_DamageOCR_Target.chatRowsInfo[i];
                if (rowForDefineTarget.ignoreThisRow)
                    continue;
                transformations_DamageOCR_Target.DefineTargetPositionForRow(rowForDefineTarget, bmpResizedFast, garbageAfterDamageWidth, avgCharWidth);
            }
            bmpResizedFast.Unlock();

            //12. Tesseract this targets
            int posDmgToInsert_I = newDmgIndexStart;
            for (int i = validI; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
            {
                rowForDefineTarget = transformations_DamageOCR_Target.chatRowsInfo[i];
                if (rowForDefineTarget.ignoreThisRow)
                    continue;
                BufferedBitmapPack bufferedBmpPack = singleTargetNameBufferedBmps.GetFittingBitmap(rowForDefineTarget.targetHorizWidth + (int)(rowForDefineTarget.targetHorizWidth * 0.1), rowForDefineTarget.height);
                if (bufferedBmpPack == null)
                {
                    //throw new Exception("[Error] Cant find fitting bitmap. Probably error in the last row params");
                    return -8;
                }
                DrawTargetNameOnFittingBitmap(bmpResized, bufferedBmpPack, rowForDefineTarget);


                damageTargetPair[posDmgToInsert_I].target = OcrStringOneLineFromBitmap(bufferedBmpPack.bitmap).Trim();
                posDmgToInsert_I += 1;
            }

            damageHistory.UpdateLastDamageArr(damageTargetPair, damageTargetPair_currLen);
            FireDamageToAllTargetDamageListeners(damageTargetPair, newDmgIndexStart, damageTargetPair_currLen, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

            return 0;
        }

        public List<DamageTarget> damageTargetPair = new List<DamageTarget>();
        public int damageTargetPair_currLen = 0;
        DamageHistory damageHistory = new DamageHistory();

        public long ParseDamageFromString(string s)
        {
            s = s.Trim();
            if (s.Length < 1)
                return -1;
            s = Transformations_DamageString.RemoveGarbageFromRawOcrDamageString_Regex(s);
            long res;
            if (Int64.TryParse(s, out res))
            {
                return res;
            }
            return -1;
        }

        public string OcrStringOneLineFromBitmap(Bitmap bmp)
        {
            Page page = tesEngine.Process(bmp, PageSegMode.SingleLine);
            string res = page.GetText();
            page.Dispose();
            return res;
        }

        public string OcrStringOneWordFromBitmap(Bitmap bmp)
        {
            Page page = tesEngine.Process(bmp, PageSegMode.SingleWord);
            string res = page.GetText();
            page.Dispose();
            return res;
        }

        public string OcrStringOneColumnFromBitmap(Bitmap bmp)
        {
            Page page = tesEngine.Process(bmp, PageSegMode.SingleColumn);
            string res = page.GetText();
            page.Dispose();
            return res;
        }

        public void DrawDamageOnFittingBitmap(Bitmap bmpSrc, BufferedBitmapPack fittingBmp, ChatRowInfo chatRow)
        {
            Graphics gFitting = fittingBmp.bitmapGraphics;
            Size_Uncor sizeFitting = fittingBmp.bitmapSize;

            int posX_dest = (sizeFitting.width - chatRow.damageHorizWidth) / 2;
            int posY_dest = (sizeFitting.height - chatRow.height) / 2;

            gFitting.FillRectangle(solidBrushWhite, 0, 0, sizeFitting.width, sizeFitting.height);
            gFitting.DrawImage(
                bmpSrc,
                new Rectangle(posX_dest, posY_dest, chatRow.damageHorizWidth, chatRow.height),
                new Rectangle(chatRow.damageHorizPosStart, chatRow.posStart, chatRow.damageHorizWidth, chatRow.height),
                GraphicsUnit.Pixel);
        }

        public void DrawTargetNameOnFittingBitmap(Bitmap bmpSrc, BufferedBitmapPack fittingBmp, ChatRowInfo chatRow)
        {
            Graphics gFitting = fittingBmp.bitmapGraphics;
            Size_Uncor sizeFitting = fittingBmp.bitmapSize;

            int newChatRowHeight = chatRow.height + (int)(avgSpaceBetweenRows * 0.8);

            int posX_dest = (sizeFitting.width - chatRow.targetHorizWidth) / 2;
            int posY_dest = (sizeFitting.height - newChatRowHeight) / 2;

            gFitting.FillRectangle(solidBrushWhite, 0, 0, sizeFitting.width, sizeFitting.height);
            gFitting.DrawImage(
                bmpSrc,
                new Rectangle(posX_dest, posY_dest, chatRow.targetHorizWidth, newChatRowHeight),
                new Rectangle(chatRow.targetHorizStart, chatRow.posStart, chatRow.targetHorizWidth, newChatRowHeight),
                GraphicsUnit.Pixel);
        }


        public void DrawDamageOnFittingBitmap(Bitmap bitmapSource, BufferedBitmapPack fittingBmp)
        {
            Graphics gFitting = fittingBmp.bitmapGraphics;
            Size_Uncor sizeFitting = fittingBmp.bitmapSize;

            gFitting.FillRectangle(solidBrushWhite, new Rectangle(0, 0, sizeFitting.width, sizeFitting.height));

            ChatRowInfo rowInfo;
            int posVertStart_dest = avgSpaceBetweenRows;
            int posHorizDraw_dest = avgCharWidth / 2;
            for (int i = 0; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
            {
                rowInfo = transformations_DamageOCR_Target.chatRowsInfo[i];
                if (rowInfo.ignoreThisRow)
                    continue;

                gFitting.DrawImage(
                    bitmapSource,
                    new Rectangle(posHorizDraw_dest, posVertStart_dest, rowInfo.damageHorizWidth, rowInfo.height),
                    new Rectangle(rowInfo.damageHorizPosStart, rowInfo.posStart, rowInfo.damageHorizWidth, rowInfo.height),
                    GraphicsUnit.Pixel);

                posVertStart_dest = posVertStart_dest + avgSpaceBetweenRows + rowInfo.height;
            }
        }

        public int GetMaximumRowDamageWidth()
        {
            int maxWidth = 0;
            ChatRowInfo rowInfo;
            for (int i = 0; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
            {
                rowInfo = transformations_DamageOCR_Target.chatRowsInfo[i];
                if (rowInfo.ignoreThisRow)
                    continue;


                if (rowInfo.damageHorizWidth > maxWidth)
                    maxWidth = rowInfo.damageHorizWidth;
            }
            return maxWidth;
        }

        public int GetSummaryHeightRows()
        {
            int sumHeight = 0;
            ChatRowInfo rowInfo;
            for (int i = 0; i < transformations_DamageOCR_Target.currentlyRowsCount; i++)
            {
                rowInfo = transformations_DamageOCR_Target.chatRowsInfo[i];
                if (rowInfo.ignoreThisRow)
                    continue;

                sumHeight += rowInfo.height;
            }
            return sumHeight;
        }

        public int FireBitmap(Bitmap bmp)
        {
            return ProcessDamageRecognition(bmp);
        }


        private List<ITargetDamageListener> damageTargetListeners = new List<ITargetDamageListener>();
        public void RegisterTargetDamageListener(ITargetDamageListener damageTargetListener)
        {
            if (!damageTargetListeners.Contains(damageTargetListener))
            {
                damageTargetListeners.Add(damageTargetListener);
            }
        }

        public void RemoveTargetDamageListener(ITargetDamageListener damageTargetListener)
        {
            damageTargetListeners.Remove(damageTargetListener);
        }

        public void FireDamageToAllTargetDamageListeners(List<DamageTarget> damageTargets, int posStart, int posEnd, long dmgTime)
        {
            for (int i = 0; i < damageTargetListeners.Count; i++)
            {
                damageTargetListeners[i].RecieveTargetDamage(damageTargets, posStart, posEnd, dmgTime);
            }
        }

        public void RegisterBitmapListener(IBitmapListener listener)
        {
            throw new NotImplementedException();
        }

        public void UnregisterBitmapListener(IBitmapListener listener)
        {
            throw new NotImplementedException();
        }

        public void FireBitmapToAllListeners(Bitmap bmp)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            base.DisposeEngine();
            if (singleDamageBufferedBmps != null)
            {
                singleDamageBufferedBmps.Dispose();
                singleDamageBufferedBmps = null;
            }

            if (singleTargetNameBufferedBmps != null)
            {
                singleTargetNameBufferedBmps.Dispose();
                singleTargetNameBufferedBmps = null;
            }
            //

            if (damageTargetListeners != null)
            {
                damageTargetListeners.Clear();
            }

            if (bmpResizedFast != null)
            {
                bmpResizedFast.Dispose();
                bmpResizedFast = null;
            }

            if (bmpResizedGraphics != null)
            {
                bmpResizedGraphics.Dispose();
                bmpResizedGraphics = null;
            }

            if (bmpResized != null)
            {
                bmpResized.Dispose();
                bmpResized = null;
            }
        }
    }
}
