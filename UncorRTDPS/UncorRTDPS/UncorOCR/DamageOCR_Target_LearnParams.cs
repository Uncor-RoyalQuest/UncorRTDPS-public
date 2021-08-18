using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using UncorRTDPS.FastBitmap;
using UncorRTDPS.UncorOCR.BitmapFunctions;
using UncorRTDPS.UncorOCR.DamageOCR_Data;
using UncorRTDPS.UncorOCR.Transformations;

namespace UncorRTDPS.UncorOCR
{
    public class DamageOCR_Target_LearnParams
    {
        public int learntAverageCharacterWidth = -1;
        public int learntDamageTargetGarbageWidth = -1; //12321 "damage. Target:" Name.
        public int learntMaximumRowHeight = -1;
        public int learntAverageWhiteSpaceBetweenRows = -1;

        public int learntDamageWordWidth = -1;
        public int learntTargetWordWidth = -1;

        public int learntVerticallyDataStart = -1;

        public int learntBitmapHeight = -1;
        public int learntBitmapWidth = -1;

        public int BlackARGB = Color.Black.ToArgb();


        //end optimize

        //1.1 start
        public int FindAverageLengthOfCharacter(FastBitmap.FastBitmap bmpFast, int rowPosStart, int rowPosEnd)
        {
            int chars = 0;
            int summCharsLen = 0;

            List<int> charsLength = new List<int>();

            bool prevWasWhite = false;
            for (int i = 0; i < bmpFast.Width; i++)
            {
                if (Util_FastBitmap.CheckIfVerticallyBlackExists(bmpFast, i, rowPosStart, rowPosEnd, 1) && prevWasWhite)
                {
                    int posHorizCharEnd = -1;
                    for (int h = i + 1; h < bmpFast.Width; h++)
                    {
                        if (!Util_FastBitmap.CheckIfVerticallyBlackExists(bmpFast, h, rowPosStart, rowPosEnd, 1))
                        {
                            posHorizCharEnd = h;
                            break;
                        }
                    }

                    if (posHorizCharEnd != -1)
                    {
                        prevWasWhite = true;
                        chars += 1;
                        summCharsLen += posHorizCharEnd - i;
                        charsLength.Add(posHorizCharEnd - i);
                        i = posHorizCharEnd + 1;
                    }
                }
                else
                {
                    prevWasWhite = true;
                }
            }

            if (charsLength.Count < 1)
                return -1;
            int minLen = charsLength.Min();
            int maxLen = charsLength.Max();
            int avg = (int) charsLength.Average();
            if (minLen < avg / 2)
            {
                for (int i=0; i<charsLength.Count; i++)
                {
                    if (charsLength[i] < avg / 2)
                    {
                        charsLength.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            int topBorderCut = avg + (avg / 2);
            if (maxLen > topBorderCut)
            {
                for (int i = 0; i < charsLength.Count; i++)
                {
                    if (charsLength[i] > topBorderCut)
                    {
                        charsLength.RemoveAt(i);
                        i -= 1;
                    }
                }
            }

            if (charsLength.Count < 1)
                return -1;
            return (int)(charsLength.Average()+1);
            //return summCharsLen / chars;
        }

        public int startFillerWord1 = 0;
        public int endFillerWord1 = 0;
        public int startFillerWord2 = 0;
        public int endFillerWord2 = 0;


        /// <summary>
        /// row.damageHorizPosStart should be defined first
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bmpFast"></param>
        /// <param name="averageCharLen"></param>
        /// <param name="damageWordCharsCount"></param>
        /// <param name="targetWordCharsCound"></param>
        /// <returns></returns>
        public int FindFillersLengthAfterDamage(ChatRowInfo row, FastBitmap.FastBitmap bmpFast, int averageCharLen, int damageWordCharsCount, int targetWordCharsCound)
        {
            //+2 because 
            //+1 as "minimum one character of damage is a must"
            //+1 as compensation of the "." and white space after damage
            int posStartSearch = row.damageHorizPosStart + (averageCharLen * (damageWordCharsCount + 2));

            //find end of the "damage."
            int horizEndWordDamage = Util_FastBitmap.FindEndOfThisWord(bmpFast, posStartSearch, row.posStart, row.posEnd, averageCharLen);
            if (horizEndWordDamage == -1)
                return -1;
            
            //find start of the "damage."
            int horizStartWordDamage = Util_FastBitmap.FindStartOfThisWord(bmpFast, posStartSearch, row.posStart, row.posEnd, averageCharLen);

            //find start of the "Target:"
            int posStartSearchEnd = horizEndWordDamage + averageCharLen + ((targetWordCharsCound / 2) * averageCharLen);
            int horizStartWordTarget = Util_FastBitmap.FindStartOfThisWord(bmpFast, posStartSearchEnd, row.posStart, row.posEnd, averageCharLen);

            //find end of "Target:"
            int horizEndWordTarget = Util_FastBitmap.FindEndOfThisWord(bmpFast, posStartSearchEnd, row.posStart, row.posEnd, averageCharLen);

            //return result length of the "damage. Target:"
            startFillerWord1 = horizStartWordDamage;
            endFillerWord1 = horizEndWordDamage;
            startFillerWord2 = horizStartWordTarget;
            endFillerWord2 = horizEndWordTarget;

            return horizEndWordTarget - horizStartWordDamage;
        }



        public Bitmap TrainWithFullCycleOfTransformations(Bitmap bmp, OCRSettings ocrSettings)
        {
            Transformations_DamageOCR_Target transformations_DamageOCR_Target = new Transformations_DamageOCR_Target();

            float resizeScale = ocrSettings.OCR_imageScaling;
            float brightnessBarrier = ocrSettings.OCR_brightnessBarrier;

            Bitmap bmpResized = new Bitmap((int)(bmp.Width * resizeScale), (int)(bmp.Height * resizeScale), PixelFormat.Format32bppArgb);
            FastBitmap.FastBitmap bmpResizedFast = new FastBitmap.FastBitmap(bmpResized);
            Graphics bmpResizedGraphics = Graphics.FromImage(bmpResized);
            bmpResizedGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Transformations_Size.ResizeImage(bmp, bmpResizedFast.Width, bmpResizedFast.Height, bmpResizedGraphics);

            bmpResizedFast.Lock();
            BitmapTransformations.MakeBlackWhite(bmpResizedFast, brightnessBarrier);
            transformations_DamageOCR_Target.RemoveGarbagedRows_FullWidthCheck(bmpResizedFast);



            long tStart, tEnd;

            tStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            transformations_DamageOCR_Target.FindAllRowsInfo(bmpResizedFast, 30, 0, 1);
            tEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            ChatRowInfo rowFirst = transformations_DamageOCR_Target.GetFirstValidRowInfo();
            if (rowFirst == null)
            {
                bmpResizedFast.Unlock();
                return null;
            }
            
            int avgCharLen = FindAverageLengthOfCharacter(bmpResizedFast, rowFirst.posStart, rowFirst.posEnd);
            if (avgCharLen == -1)
            {
                bmpResizedFast.Unlock();
                return null;
            }

            transformations_DamageOCR_Target.DefineDamageStartForRow(rowFirst, bmpResizedFast, 0);

            int fillersLengthAfterDamage = FindFillersLengthAfterDamage(rowFirst, bmpResizedFast, avgCharLen, 5, 4);
            if (fillersLengthAfterDamage == -1)
            {
                bmpResizedFast.Unlock();
                return null;
            }
            //

            //define start/end of the damage for each row
            tStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            transformations_DamageOCR_Target.DefineDamageStartEndForEachRow(bmpResizedFast, avgCharLen, 5, endFillerWord1 - startFillerWord1);
            tEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            tStart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //?change implementation of this function
            transformations_DamageOCR_Target.FindAllTargetsPositions(bmpResizedFast, fillersLengthAfterDamage, avgCharLen);
            tEnd = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


            //find minimum start by vertical
            int vertStart = 0;
            while (!transformations_DamageOCR_Target.CheckIfVerticallyBlackExists(bmpResizedFast, vertStart, 0, bmpResizedFast.Height, 1))
            {
                vertStart += 1;
            }
            //DONE find minimum start by vertical
            bmpResizedFast.Unlock();

            Transformations_DrawBoxes.DrawBoxesAroundDamage(bmpResized, transformations_DamageOCR_Target.chatRowsInfo, transformations_DamageOCR_Target.currentlyRowsCount);

            Transformations_DrawBoxes.DrawVagueBoxAroundFillers(bmpResized, fillersLengthAfterDamage, avgCharLen, transformations_DamageOCR_Target.chatRowsInfo, transformations_DamageOCR_Target.currentlyRowsCount);

            Transformations_DrawBoxes.DrawBoxAroundTargets(bmpResized, transformations_DamageOCR_Target.chatRowsInfo, transformations_DamageOCR_Target.currentlyRowsCount);


            learntAverageCharacterWidth = avgCharLen;
            learntDamageTargetGarbageWidth = fillersLengthAfterDamage;

            //find max height
            int maxHeight = -1;
            int currentlyRowsCount = transformations_DamageOCR_Target.currentlyRowsCount;
            List<ChatRowInfo> chatRowsInfo = transformations_DamageOCR_Target.chatRowsInfo;
            for (int i=0; i<currentlyRowsCount; i++)
            {
                ChatRowInfo rowI = chatRowsInfo[i];
                if (rowI.ignoreThisRow)
                    continue;

                int height = rowI.posEnd - rowI.posStart;
                if (height > maxHeight)
                    maxHeight = height;
            }
            learntMaximumRowHeight = maxHeight;

            //find average whitespace between rows
            int sumWS = 0;
            int countWS = 0;
            for (int i = 0; i < currentlyRowsCount - 1; i+=1)
            {
                ChatRowInfo rowThis = chatRowsInfo[i];
                ChatRowInfo rowNext = chatRowsInfo[i+1];
                if (rowThis.ignoreThisRow || rowNext.ignoreThisRow)
                    continue;

                int spaceBetween = rowNext.posStart - rowThis.posEnd;
                sumWS += spaceBetween;
                countWS += 1;
            }

            if (countWS == 0)
            {
                return null;
            }
            learntAverageWhiteSpaceBetweenRows = sumWS / countWS;

            //
            learntDamageWordWidth = endFillerWord1 - startFillerWord1;
            learntTargetWordWidth = endFillerWord2 - startFillerWord2;

            //
            learntVerticallyDataStart = vertStart;

            learntBitmapHeight = bmpResizedFast.Height;
            learntBitmapWidth = bmpResizedFast.Width;

            return bmpResized;
        }

        public Supplementary_Parameters_DamageOCR packAndGetAllTrainedParams()
        {
            return new Supplementary_Parameters_DamageOCR(
                learntAverageCharacterWidth,
                learntDamageTargetGarbageWidth,
                learntMaximumRowHeight,
                learntAverageWhiteSpaceBetweenRows,
                learntDamageWordWidth,
                learntTargetWordWidth,
                learntVerticallyDataStart,
                learntBitmapHeight,
                learntBitmapWidth);
        }

    }
}
