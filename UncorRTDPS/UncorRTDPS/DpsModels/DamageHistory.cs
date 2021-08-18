using System.Collections.Generic;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.DpsModels
{
    public class DamageHistory
    {
        private long[] lastDamageArr;
        private int lastDamageArrLen = 0;
        public int LastDamageArrLen { get { return lastDamageArrLen; } }

        public DamageHistory()
        {
            int defLen = 16;
            lastDamageArr = new long[defLen];
            for (int i = 0; i < lastDamageArr.Length; i++)
                lastDamageArr[i] = -1;
        }

        public void ExpandArr(int lenToExpand)
        {
            long[] newArr = new long[lenToExpand + (lenToExpand / 2)];
            for (int i = 0; i < lastDamageArr.Length; i++)
                newArr[i] = lastDamageArr[i];
            for (int i = lastDamageArr.Length; i < newArr.Length; i++)
                newArr[i] = -1;
            lastDamageArr = newArr;
            
        }

        public void ResetArrWithNewLength(int len)
        {
            lastDamageArr = new long[len];
            for (int i = 0; i < lastDamageArr.Length; i++)
                lastDamageArr[i] = -1;
            lastDamageArrLen = 0;
        }

        public void UpdateLastDamageArr(List<DamageTarget> damageTarget_list, int actualLen)
        {
            if (lastDamageArr.Length < actualLen)
                ResetArrWithNewLength(actualLen);
            lastDamageArrLen = actualLen;
            for (int i = 0; i < actualLen; i++)
                lastDamageArr[i] = damageTarget_list[i].damage;

        }

        /// <summary>
        ///  By tests, returns -1 when damage[] length < prevDamage[] length AND prevDamage[] already has damage of the damage[] array 
        /// </summary>
        /// <param name="damageTarget_list"></param>
        /// <param name="actualLen"></param>
        /// <returns></returns>
        public int GetIndexOfStartNewDamage(List<DamageTarget> damageTarget_list, int actualLen)
        {
            if (lastDamageArrLen == 1)
            {
                //concat only on 1 value
                long dmgToCompare = lastDamageArr[0];
                if (dmgToCompare != -1)
                {
                    for (int i = 0; i < actualLen; i++)
                    {
                        if (dmgToCompare == damageTarget_list[i].damage)
                        {
                            return i + 1;
                        }
                    }


                    /*
                    if (actualLen < 4)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                    */
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                //take pair of values and check if this seq is in prev arr
                int posVal1 = FindValidValPos(0, damageTarget_list, actualLen);
                int posVal2 = FindValidValPos(posVal1 + 1, damageTarget_list, actualLen);

                while (posVal1 != -1 && posVal2 != -1)
                {
                    int tId = GetIndIfPairExistInPrevDmg(posVal1, posVal2, damageTarget_list);


                    if (tId != -1)
                    {
                        int lastHowManyAfter = lastDamageArrLen - tId;
                        int newHowManyAfter = actualLen - posVal1;

                        if (lastHowManyAfter < newHowManyAfter)
                        {
                            //new dmg was found after index = ...
                            int startIndexNewDmg = posVal1 + lastHowManyAfter;

                            return startIndexNewDmg;
                        }
                        return -1;
                    }
                    posVal1 = posVal2;
                    posVal2 = FindValidValPos(posVal1 + 1, damageTarget_list, actualLen);
                }

                //assume all damage is new
                return 0;
            }
        }

        private int FindValidValPos(int posStart, List<DamageTarget> dmg, int len)
        {
            for (int i = posStart; i < len; i++)
            {
                if (dmg[i].damage != -1)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetIndIfPairExistInPrevDmg(int posVal1, int posVal2, List<DamageTarget> dmg)
        {
            int shiftVal = posVal2 - posVal1;
            int maxInd = lastDamageArrLen - 1;
            for (int i = 0; i < lastDamageArrLen; i++)
            {
                if (i + shiftVal > maxInd)
                    return -1;

                if (lastDamageArr[i] == dmg[posVal1].damage && lastDamageArr[i + shiftVal] == dmg[posVal2].damage)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
