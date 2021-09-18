using System.Collections.Generic;
using System.Drawing;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.UncorOCR.Transformations
{
    public class Transformations_DamageOCR_Target
    {
        private int removeGarbagedRows_LastGarbageSeparator_Starting;
        private int removeGarbagedRows_LastGarbageSeparator_Ending;
        public int BlackARGB = Color.Black.ToArgb();
        public int WhiteARGB = Color.White.ToArgb();

        public List<ChatRowInfo> chatRowsInfo = new List<ChatRowInfo>();
        public int chatRowsInfo_RealLen = 0;
        public int currentlyRowsCount = 0;
        public int currentlyValidRowsCount = 0;


        /// <summary>
        /// Returns a real index of the I valid element from chatRowsInfo
        /// </summary>
        /// <param name="indexFind"></param>
        /// <returns></returns>
        public int GetIndexOfTheIValidElemFromChatRowsInfo(int findIndex)
        {
            int countValid = 0;

            int findN = findIndex + 1;
            for (int i = 0; i < currentlyRowsCount; i++)
            {
                if (chatRowsInfo[i].ignoreThisRow)
                    continue;

                countValid += 1;
                if (findN == countValid)
                    return i;
            }

            return -1;
        }

        //
        public void DefineDamageStartEndForEachRow(FastBitmap.FastBitmap bmpFast, int avgCharLen, int damageWordCharsCount, int damageWordWidth)
        {
            for (int i = 0; i < currentlyRowsCount; i++)
            {
                if (chatRowsInfo[i].ignoreThisRow)
                    continue;

                DefineDamageStartForRow(chatRowsInfo[i], bmpFast, 0);
                DefineDamageEndForRow(chatRowsInfo[i], bmpFast, avgCharLen, damageWordCharsCount, damageWordWidth);
            }
        }

        public void DefineDamageStartEndForEachRow_Except(FastBitmap.FastBitmap bmpFast, int avgCharLen, int damageWordCharsCount, int damageWordWidth, ChatRowInfo chatRowExcept)
        {
            for (int i = 0; i < currentlyRowsCount; i++)
            {
                if (chatRowsInfo[i].ignoreThisRow || chatRowsInfo[i] == chatRowExcept)
                    continue;

                DefineDamageStartForRow(chatRowsInfo[i], bmpFast, 0);
                DefineDamageEndForRow(chatRowsInfo[i], bmpFast, avgCharLen, damageWordCharsCount, damageWordWidth);
            }
        }

        public void DefineDamageStartEndForRow(ChatRowInfo chatRow, FastBitmap.FastBitmap bmpFast, int avgCharLen, int damageWordCharsCount, int damageWordWidth)
        {
            DefineDamageStartForRow(chatRow, bmpFast, 0);
            DefineDamageEndForRow(chatRow, bmpFast, avgCharLen, damageWordCharsCount, damageWordWidth);
        }

        //

        public int FindStartOfThisWord(FastBitmap.FastBitmap bmpFast, int horizStart, int vertStart, int vertEnd, int averageCharLen)
        {
            int whiteSpaceToIndicateEnd = averageCharLen / 2;

            int whiteSpaceWidth = 0;
            for (int i = horizStart; i > -1; i--)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, vertStart, vertEnd, 1))
                {
                    whiteSpaceWidth += 1;
                    if (whiteSpaceWidth >= whiteSpaceToIndicateEnd)
                    {
                        return i + whiteSpaceToIndicateEnd;
                    }
                }
                else
                {
                    whiteSpaceWidth = 0;
                }
            }

            return -1;
        }

        public int FindEndOfThisWord(FastBitmap.FastBitmap bmpFast, int horizStart, int vertStart, int vertEnd, int averageCharLen)
        {
            int whiteSpaceToIndicateEnd = averageCharLen / 2;

            int whiteSpaceWidth = 0;
            for (int i = horizStart; i < bmpFast.Width; i++)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, vertStart, vertEnd, 1))
                {
                    whiteSpaceWidth += 1;
                    if (whiteSpaceWidth >= whiteSpaceToIndicateEnd)
                    {
                        return i - whiteSpaceToIndicateEnd + 1;
                    }
                }
                else
                {
                    whiteSpaceWidth = 0;
                }
            }

            return -1;
        }

        public bool CheckIfVerticallyBlackExists(FastBitmap.FastBitmap bmpFast, int horizontalPos, int rowVerticalStart, int rowVerticalEnd, int verticalStep)
        {
            for (int i = rowVerticalStart; i < rowVerticalEnd; i += verticalStep)
            {
                if (bmpFast.GetPixelInt(horizontalPos, i) == BlackARGB)
                    return true;
            }
            return false;
        }

        public ChatRowInfo GetFirstValidRowInfo()
        {
            for (int i = 0; i < currentlyRowsCount; i++)
            {
                if (!chatRowsInfo[i].ignoreThisRow)
                {
                    return chatRowsInfo[i];
                }
            }
            return null;
        }

        public ChatRowInfo GetLastValidRowInfo()
        {
            for (int i = currentlyRowsCount - 1; i > -1; i--)
            {
                if (!chatRowsInfo[i].ignoreThisRow)
                {
                    return chatRowsInfo[i];
                }
            }
            return null;
        }

        public void FindAllTargetsPositions(FastBitmap.FastBitmap bmpFast, int fillerLength, int avgCharLength)
        {
            ChatRowInfo row;
            for (int r = 0; r < currentlyRowsCount; r++)
            {
                row = chatRowsInfo[r];
                if (row.ignoreThisRow)
                    continue;

                DefineTargetPositionForRow(row, bmpFast, fillerLength, avgCharLength);
            }
        }

        public void DefineTargetPositionForRow(ChatRowInfo row, FastBitmap.FastBitmap bmpFast, int fillerLength, int avgCharLength)
        {
            row.targetHorizStart = row.damageHorizPosEnd + avgCharLength + fillerLength;
            int i;
            for (i = row.targetHorizStart; i < bmpFast.Width; i++)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, row.posStart, row.posEnd, 1))
                {
                    int countWhiteSpace = 1;
                    for (int w = i + 1; w < bmpFast.Width; w++)
                    {
                        if (CheckIfVerticallyBlackExists(bmpFast, w, row.posStart, row.posEnd, 1))
                        {
                            break;
                        }
                        else
                        {
                            countWhiteSpace += 1;
                        }
                    }

                    if (countWhiteSpace > avgCharLength)
                    {
                        row.targetHorizEnd = i;
                        row.targetHorizWidth = row.targetHorizEnd - row.targetHorizStart;
                        break;
                    }
                }
            }
            if (i == bmpFast.Width)
            {
                row.targetHorizEnd = bmpFast.Width - 1;
                row.targetHorizWidth = row.targetHorizEnd - row.targetHorizStart;
            }
        }



        public void DefineDamageStartForRow(ChatRowInfo row, FastBitmap.FastBitmap bmpFast, int horizPosStart)
        {
            //1. find start
            int rowCenter = row.posStart + ((row.posEnd - row.posStart) / 2);
            int firstBlack = 0;

            //1.1 forward fast search
            for (int i = horizPosStart; i < bmpFast.Width; i++)
            {
                if (bmpFast.GetPixelInt(i, rowCenter) == BlackARGB)
                {
                    firstBlack = i;
                    break;
                }
            }

            //1.2 backward full search
            int dmgHorizStart = firstBlack;
            for (int i = firstBlack - 1; i > -1; i--)
            {
                if (!CheckIfVerticallyBlackExists(bmpFast, i, row.posStart, row.posEnd, 1))
                {
                    dmgHorizStart = i + 1;
                    break;
                }
            }

            row.damageHorizPosStart = dmgHorizStart;
        }

        /// <summary> Firstly, start of the damage must be defined.
        /// Defines through "damage. Target:" whitespace and minus "damage." length.
        /// Also defines the "row.wordDamageHorizEnd" param.
        /// </summary>
        public int DefineDamageEndForRow(ChatRowInfo row, FastBitmap.FastBitmap bmpFast, int averageCharLen, int damageWordCharsCount, int damageWordLearntWidth)
        {
            //+2 because 
            //+1 as "minimum one character of damage is a must"
            //+1 as compensation of the "." and white space after damage
            int posStartSearch = row.damageHorizPosStart + (averageCharLen * (damageWordCharsCount + 2));

            //find end of the "damage."
            int horizEndWordDamage = FindEndOfThisWord(bmpFast, posStartSearch, row.posStart, row.posEnd, averageCharLen);
            if (horizEndWordDamage == -1)
                return -1;

            row.wordDamageHorizEnd = horizEndWordDamage;

            //calc vague pos between "123" and "damage."
            int horizVaguePosBetween = horizEndWordDamage - damageWordLearntWidth - (averageCharLen / 3);

            //backward walk until black = damage end
            for (int i = horizVaguePosBetween; i > -1; i--)
            {
                if (CheckIfVerticallyBlackExists(bmpFast, i, row.posStart, row.posEnd, 1))
                {
                    row.damageHorizPosEnd = i + 1;
                    row.damageHorizWidth = row.damageHorizPosEnd - row.damageHorizPosStart;
                    return 0;
                }
            }
            return -1;
        }


        /// <summary>
        /// Find start and end of each row. Uses removeGarbagedRows_LastGarbageSeparator_Starting and removeGarbagedRows_LastGarbageSeparator_Ending as area definer for processing. Set them to -1 to ignore
        /// </summary>
        /// <param name="fastBmp"></param>
        /// <param name="horizPosEndCheck"></param>
        /// <returns></returns>
        public int FindAllRowsInfo(FastBitmap.FastBitmap fastBmp, int horizPosEndCheck, int horizPosStartCheck, int horizStepForCheck)
        {
            int posStartCheck = 0;
            int posEndCheck = fastBmp.Height;
            if (removeGarbagedRows_LastGarbageSeparator_Starting != -1)
            {
                posStartCheck = removeGarbagedRows_LastGarbageSeparator_Starting + 1;
            }

            if (removeGarbagedRows_LastGarbageSeparator_Ending != -1)
            {
                posEndCheck = removeGarbagedRows_LastGarbageSeparator_Ending;
            }
            /*
            int stepY = (int)(fastBmp.Height * 0.01);
            if (stepY < 2)
                stepY = 2;
            */
            int stepY = 1;
            bool emptyRow;
            int dataRowsCount = 0;

            //
            int maxDataRowHeight = 0;
            currentlyRowsCount = 0;
            //
            //
            for (int i = posStartCheck; i < posEndCheck; i += stepY)
            {
                if (IsBmpRowHasSmth_FullManualArea(fastBmp, i, horizPosEndCheck, horizPosStartCheck, horizStepForCheck))
                {
                    emptyRow = false;
                }
                else
                {
                    emptyRow = true;
                }

                if (!emptyRow)
                {
                    dataRowsCount += 1;
                    //find end of DataRow
                    bool endOfDataRowFound = false;
                    int startOfDataRow = i;
                    for (int re = i + 1; re < fastBmp.Height; re += stepY)
                    {
                        if (!IsBmpRowHasSmth_FullManualArea(fastBmp, re, horizPosEndCheck, horizPosStartCheck, horizStepForCheck))
                        {
                            //push i to new line
                            i = re + 1; //be careful, i+=stepY is the next step after break
                            endOfDataRowFound = true;

                            //i = endOfDataRow
                            int dataRowHeight = i - startOfDataRow;

                            //expand if needed
                            if (chatRowsInfo_RealLen < currentlyRowsCount + 1)
                            {
                                for (int h = chatRowsInfo_RealLen; h < currentlyRowsCount + 1; h++)
                                {
                                    chatRowsInfo.Add(new ChatRowInfo());
                                }
                                chatRowsInfo_RealLen = chatRowsInfo.Count;
                            }
                            //add to arr
                            chatRowsInfo[currentlyRowsCount].posEnd = i;
                            chatRowsInfo[currentlyRowsCount].posStart = startOfDataRow;
                            chatRowsInfo[currentlyRowsCount].height = i - startOfDataRow;
                            chatRowsInfo[currentlyRowsCount].ignoreThisRow = false;
                            currentlyRowsCount += 1;

                            if (dataRowHeight > maxDataRowHeight)
                                maxDataRowHeight = dataRowHeight;

                            break;
                        }
                    }
                    if (!endOfDataRowFound)
                        break;
                }
            }

            //
            int compareHeight = maxDataRowHeight / 2;
            currentlyValidRowsCount = currentlyRowsCount;
            for (int h = 0; h < currentlyRowsCount; h++)
            {
                if (chatRowsInfo[h].height < compareHeight)
                {
                    chatRowsInfo[h].ignoreThisRow = true;
                    currentlyValidRowsCount -= 1;
                }
            }
            //

            return dataRowsCount;
        }

        /// <summary>
        /// Optimize horizStep before calling. It is a pure function, no "ifs".
        /// </summary>
        /// <param name="fastBmp"></param>
        /// <param name="row"></param>
        /// <param name="horizPosEnd"></param>
        /// <param name="horizPosStart"></param>
        /// <param name="horizStep"></param>
        /// <returns></returns>
        public bool IsBmpRowHasSmth_FullManualArea(FastBitmap.FastBitmap fastBmp, int row, int horizPosEnd, int horizPosStart, int horizStep)
        {
            for (int roll = 0; roll < horizStep; roll++)
            {
                for (int i = horizPosStart + roll; i < horizPosEnd; i += horizStep)
                {
                    if (fastBmp.GetPixelInt(i, row) == BlackARGB)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        //end 1.1


        public void RemoveGarbagedRows_ManualAreaForCheck(FastBitmap.FastBitmap fastBmp, int horizStart, int horizEnd)
        {
            removeGarbagedRows_LastGarbageSeparator_Starting = -1;
            removeGarbagedRows_LastGarbageSeparator_Ending = -1;

            /*
            int stepY = (int)(fastBmp.Height * 0.01);
            if (stepY < 2)
                stepY = 2;
            */
            int stepY = 1;

            int horizStep = (int)(fastBmp.Width * 0.01);
            if (horizStep < 2)
                horizStep = 2;

            if (IsBmpRowHasSmth_FullManualArea(fastBmp, 0, horizEnd, horizStart, horizStep))
            {
                int lastGarbagedRow = 0;

                for (int i = 1; i < fastBmp.Height; i += stepY)
                {
                    if (!IsBmpRowHasSmth_FullManualArea(fastBmp, i, horizEnd, horizStart, horizStep))
                    {
                        lastGarbagedRow = i - 1;
                        break;
                    }
                }

                fastBmp.ClearRegion(new Rectangle(0, 0, fastBmp.Width, lastGarbagedRow + 1), WhiteARGB);
                removeGarbagedRows_LastGarbageSeparator_Starting = lastGarbagedRow;
            }

            if (IsBmpRowHasSmth_FullManualArea(fastBmp, fastBmp.Height - 1, horizEnd, horizStart, horizStep))
            {
                int firstGarbagedRow = fastBmp.Height - 1;

                for (int i = fastBmp.Height - 2; i > -1; i -= stepY)
                {
                    if (!IsBmpRowHasSmth_FullManualArea(fastBmp, i, horizEnd, horizStart, horizStep))
                    {
                        firstGarbagedRow = i + 1;
                        break;
                    }
                }

                fastBmp.ClearRegion(new Rectangle(0, firstGarbagedRow, fastBmp.Width, fastBmp.Height - firstGarbagedRow), WhiteARGB);
                removeGarbagedRows_LastGarbageSeparator_Ending = firstGarbagedRow;
            }
        }


        //old suppl funcs
        /// <summary>
        /// Must be locked
        /// </summary>
        /// <param name="fastBmp"></param>
        public void RemoveGarbagedRows_FullWidthCheck(FastBitmap.FastBitmap fastBmp)
        {
            removeGarbagedRows_LastGarbageSeparator_Starting = -1;
            removeGarbagedRows_LastGarbageSeparator_Ending = -1;

            /*
            int stepY = (int)(fastBmp.Height * 0.01);
            if (stepY < 2)
                stepY = 2;
            */
            int stepY = 1;

            if (IsBmpRowHasSmth_FullCheck(fastBmp, 0))
            {
                int lastGarbagedRow = 0;

                for (int i = 1; i < fastBmp.Height; i += stepY)
                {
                    if (!IsBmpRowHasSmth_FullCheck(fastBmp, i))
                    {
                        lastGarbagedRow = i - 1;
                        break;
                    }
                }

                fastBmp.ClearRegion(new Rectangle(0, 0, fastBmp.Width, lastGarbagedRow + 1), WhiteARGB);
                removeGarbagedRows_LastGarbageSeparator_Starting = lastGarbagedRow;
            }

            if (IsBmpRowHasSmth_FullCheck(fastBmp, fastBmp.Height - 1))
            {
                int firstGarbagedRow = fastBmp.Height - 1;

                for (int i = fastBmp.Height - 2; i > -1; i -= stepY)
                {
                    if (!IsBmpRowHasSmth_FullCheck(fastBmp, i))
                    {
                        firstGarbagedRow = i + 1;
                        break;
                    }
                }

                fastBmp.ClearRegion(new Rectangle(0, firstGarbagedRow, fastBmp.Width, fastBmp.Height - firstGarbagedRow), WhiteARGB);
                removeGarbagedRows_LastGarbageSeparator_Ending = firstGarbagedRow;
            }
        }

        /// <summary>
        /// Must be locked
        /// </summary>
        /// <param name="fastBmp"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool IsBmpRowHasSmth_FullCheck(FastBitmap.FastBitmap fastBmp, int row)
        {
            int posStartX = 0;
            int stepX = (int)(fastBmp.Width * 0.01);
            if (stepX < 2)
                stepX = 2;

            for (int roll = 0; roll < stepX; roll++)
            {

                for (int i = posStartX + roll; i < fastBmp.Width; i += stepX)
                {
                    if (fastBmp.GetPixelInt(i, row) == BlackARGB)
                    {
                        return true;
                    }
                }

            }

            return false;
        }
    }
}
