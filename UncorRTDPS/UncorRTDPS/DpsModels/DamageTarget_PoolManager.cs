using System.Collections.Generic;
using UncorRTDPS.DpsModels.TargetsDictionary;
using UncorRTDPS.UncorOCR;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.DpsModels
{
    public class DamageTarget_PoolManager : ITargetDamageListener, IModelSettingsAvailable
    {
        public List<DamageTarget_DamageModel> activePool_Bosses = new List<DamageTarget_DamageModel>();
        public int activePool_Bosses_CurrentLength = 0;
        public LinkedList<int> freePositions_ActivePool_Bosses = new LinkedList<int>();

        public List<DamageTarget_DamageModel> activePool_Elites = new List<DamageTarget_DamageModel>();
        public int activePool_Elites_CurrentLength = 0;
        public LinkedList<int> freePositions_ActivePool_Elites = new LinkedList<int>();



        public List<DamageTarget_DamageModel> damageModels_Bosses = new List<DamageTarget_DamageModel>();
        public List<DamageTarget_DamageModel> damageModels_Elites = new List<DamageTarget_DamageModel>();
        public DamageTarget_DamageModel commonTarget_DamageModel = new DamageTarget_DamageModel();
        public long bossSeparationTime = 30000; //30 sec
        public long eliteSeparationTime = 15000; //15 sec
        public long commonSeparationTime = 3000; //3 sec


        //
        private int counterUpdLengthOfActivePools = 0;
        public int barrierUpdLengthOfActivePools = 20;
        //

        //
        public SearchTargetNameMethod searchTargetNameMethod;

        public enum SearchTargetNameMethod
        {
            HammingDistance, LevenshteinDistance
        }


        public DamageTarget_PoolManager(SearchTargetNameMethod searchTargetNameMethod)
        {

            List<Target> bossTargets = TargetsDictionary.TargetsDictionary.BossTargets;
            for (int i=0; i<bossTargets.Count; i++)
            {
                DamageTarget_DamageModel dt_dm = new DamageTarget_DamageModel();
                dt_dm.InitDamageTarget_DamageModel(bossTargets[i], bossSeparationTime);
                damageModels_Bosses.Add(dt_dm);
            }

            List<Target> eliteTargets = TargetsDictionary.TargetsDictionary.EliteTargets;
            for (int i = 0; i < eliteTargets.Count; i++)
            {
                DamageTarget_DamageModel dt_dm = new DamageTarget_DamageModel();
                dt_dm.InitDamageTarget_DamageModel(eliteTargets[i], eliteSeparationTime);
                damageModels_Elites.Add(dt_dm);
            }

            this.searchTargetNameMethod = searchTargetNameMethod;
        }


        /// <summary>
        /// returns null if there is no active boss
        /// </summary>
        /// <returns></returns>
        public DamageTarget_DamageModel GetFirstActiveBoss()
        {
            for (int i=0; i<activePool_Bosses_CurrentLength; i++)
            {
                if (activePool_Bosses[i].IsActive)
                    return activePool_Bosses[i];
            }

            return null;
        }

        public DamageTarget_DamageModel GetMostActiveBoss(long switchTimePeriod)
        {
            DamageTarget_DamageModel res = null;
            long lastDmgTime = 0;

            DamageTarget_DamageModel tmp_dt_dm;
            for (int i = 0; i < activePool_Bosses_CurrentLength; i++)
            {
                tmp_dt_dm = activePool_Bosses[i];
                if (tmp_dt_dm.IsActive && tmp_dt_dm.GetLastDamageTime() > lastDmgTime + switchTimePeriod)
                {
                    lastDmgTime = tmp_dt_dm.GetLastDamageTime();
                    res = tmp_dt_dm;
                }
            }

            return res;
        }

        public DamageTarget_DamageModel GetFirstActiveElite()
        {
            for (int i = 0; i < activePool_Elites_CurrentLength; i++)
            {
                if (activePool_Elites[i].IsActive)
                    return activePool_Elites[i];
            }

            return null;
        }

        public DamageTarget_DamageModel GetMostActiveElite(long switchTimePeriod)
        {
            DamageTarget_DamageModel res = null;
            long lastDmgTime = 0;

            DamageTarget_DamageModel tmp_dt_dm;
            for (int i = 0; i < activePool_Elites_CurrentLength; i++)
            {
                tmp_dt_dm = activePool_Elites[i];
                if (tmp_dt_dm.IsActive && tmp_dt_dm.GetLastDamageTime() > lastDmgTime + switchTimePeriod)
                {
                    lastDmgTime = tmp_dt_dm.GetLastDamageTime();
                    res = tmp_dt_dm;
                }
            }

            return res;
        }

        public int CalculateCurrentlyActiveBosses()
        {
            int res = 0;
            for (int i = 0; i < activePool_Bosses_CurrentLength; i++)
                if (activePool_Bosses[i].IsActive)
                    res += 1;
            return res;
        }

        public int CalculateCurrentlyActiveElites()
        {
            int res = 0;
            for (int i = 0; i < activePool_Elites_CurrentLength; i++)
                if (activePool_Elites[i].IsActive)
                    res += 1;
            return res;
        }

        /// <summary>
        /// End pos = pos after last real element
        /// </summary>
        /// <param name="damageTargetList"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        public void AddDamage(List<DamageTarget> damageTargetList, int startPos, int endPos, long dmgTime)
        {
            string searchFriendlyOriginalName;
            Target target;
            DamageTarget_DamageModel dt_dm;
            for (int i = startPos; i < endPos; i++)
            {
                if (damageTargetList[i].damage == -1)
                    continue;

                searchFriendlyOriginalName = TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(damageTargetList[i].target);
                target = TargetsDictionary.TargetsDictionary.TryGetTargetFromSimpleDictionary(searchFriendlyOriginalName);
                if (target == null)
                {
                    //target wasnt found by simple search. try hamming method searcg
                    switch (searchTargetNameMethod)
                    {
                        case SearchTargetNameMethod.HammingDistance:
                            target = TargetsDictionary.TargetsDictionary.TryGetTargetHammingMethodSearch(searchFriendlyOriginalName);
                            break;
                        case SearchTargetNameMethod.LevenshteinDistance:
                            target = TargetsDictionary.TargetsDictionary.TryGetTargetLevenshteinMethodSearch(searchFriendlyOriginalName);
                            break;
                    }
                    /*
                    Trace.Write("Result of search ("+ searchFriendlyOriginalName+") by "+ searchTargetNameMethod);
                    if (target == null)
                    {
                        Trace.Write("[common]");
                    }
                    else
                    {
                        Trace.Write(target.originalName);
                    }
                    Trace.Write(Environment.NewLine);
                    */
                }
                if (target == null)
                {
                    //target wasnt found by simple search or distance method.
                    //means it is possibly "common" target
                    commonTarget_DamageModel.AddDamage(damageTargetList[i].damage, dmgTime);
                    commonTarget_DamageModel.IsActive = true;
                    //Trace.WriteLine("common name =" + damageTargetList[i].target);
                }
                else
                {
                    //switch between boss and elite
                    switch (target.targetType)
                    {
                        case TargetType.Boss:
                            //get damage model for this target
                            dt_dm = damageModels_Bosses[target.idInsideTargetType];
                            //add damage into this damage model
                            dt_dm.AddDamage(damageTargetList[i].damage, dmgTime);
                            //check is this damage model already active
                            //if not active then add to active pool
                            if (!dt_dm.IsActive)
                                AddToActivePool_Bosses(dt_dm);
                            break;
                        case TargetType.Elite:
                            //get damage model for this target
                            dt_dm = damageModels_Elites[target.idInsideTargetType];
                            //add damage into this damage model
                            dt_dm.AddDamage(damageTargetList[i].damage, dmgTime);
                            //check is this damage model already active
                            //if not active then add to active pool
                            if (!dt_dm.IsActive)
                                AddToActivePool_Elites(dt_dm);
                            break;
                        case TargetType.Common:
                            break;
                    }
                }
            }

            //free active pools from expired ones
            CheckActivePoolBossesForExpired(dmgTime);
            CheckActivePoolElitesForExpired(dmgTime);

            //determine common is active or not
            if (commonTarget_DamageModel.IsDamageExpired(dmgTime))
            {
                commonTarget_DamageModel.IsActive = false;
            }

            //once per N adjust length of "activePool_Bosses_CurrentLength" and "activePool_Elites_CurrentLength" to the last active one
            if (counterUpdLengthOfActivePools > barrierUpdLengthOfActivePools)
            {
                counterUpdLengthOfActivePools = 0;
                AdjustActivePoolBossesLength();
                AdjustActivePoolElitesLength();

            }
            else
            {
                counterUpdLengthOfActivePools += 1;
            }
        }

        public void AdjustActivePoolBossesLength()
        {
            for (int i = activePool_Bosses_CurrentLength-1; i>-1; i--)
            {
                if (activePool_Bosses[i].IsActive)
                {
                    break;
                }
                else
                {
                    activePool_Bosses_CurrentLength -= 1;
                }
            }
        }

        public void AdjustActivePoolElitesLength()
        {
            for (int i = activePool_Elites_CurrentLength - 1; i > -1; i--)
            {
                if (activePool_Elites[i].IsActive)
                {
                    break;
                }
                else
                {
                    activePool_Elites_CurrentLength -= 1;
                }
            }
        }

        /// <summary>
        /// dm.target!=null
        /// </summary>
        /// <param name="dm"></param>
        public void AddToActivePool_Bosses(DamageTarget_DamageModel dm)
        {
            if (freePositions_ActivePool_Bosses.Count < 1)
            {
                activePool_Bosses.Add(dm);
                activePool_Bosses_CurrentLength += 1;
            }
            else
            {
                int freePos = freePositions_ActivePool_Bosses.First.Value;
                freePositions_ActivePool_Bosses.RemoveFirst();
                activePool_Bosses[freePos] = dm;
                if (freePos + 1 > activePool_Bosses_CurrentLength)
                    activePool_Bosses_CurrentLength = freePos + 1;
            }
            dm.IsActive = true;
        }

        /// <summary>
        /// dm.target!=null
        /// </summary>
        /// <param name="dm"></param>
        public void AddToActivePool_Elites(DamageTarget_DamageModel dm)
        {
            if (freePositions_ActivePool_Elites.Count < 1)
            {
                activePool_Elites.Add(dm);
                activePool_Elites_CurrentLength += 1;
            }
            else
            {
                int freePos = freePositions_ActivePool_Elites.First.Value;
                freePositions_ActivePool_Elites.RemoveFirst();
                activePool_Elites[freePos] = dm;
                if (freePos + 1 > activePool_Elites_CurrentLength)
                    activePool_Elites_CurrentLength = freePos + 1;
            }
            dm.IsActive = true;
        }

        public void CheckActivePoolBossesForExpired(long timeToCompare)
        {
            for (int i=0; i<activePool_Bosses_CurrentLength; i++)
            {
                if (activePool_Bosses[i].IsDamageExpired(timeToCompare))
                {
                    activePool_Bosses[i].IsActive = false;
                    freePositions_ActivePool_Bosses.AddLast(i);
                }
            }
        }

        public void CheckActivePoolElitesForExpired(long timeToCompare)
        {
            for (int i = 0; i < activePool_Elites_CurrentLength; i++)
            {
                if (activePool_Elites[i].IsDamageExpired(timeToCompare))
                {
                    activePool_Elites[i].IsActive = false;
                    freePositions_ActivePool_Elites.AddLast(i);
                }
            }
        }

        public void FireTargetDamage(List<DamageTarget> damageTargets, int posStart, int posEnd, long dmgTime)
        {
            AddDamage(damageTargets, posStart, posEnd, dmgTime);
        }

        public void ApplySettings(ModelSettings ms)
        {
            bossSeparationTime = ms.BossDamageSeparationDelayMS;
            eliteSeparationTime = ms.EliteDamageSeparationDelayMS;
            commonSeparationTime = ms.CommonDamageSeparationDelayMS;

            for (int i = 0; i < damageModels_Bosses.Count; i++)
            {
                damageModels_Bosses[i].msTimeSeparation = bossSeparationTime;
            }

            for (int i = 0; i < damageModels_Elites.Count; i++)
            {
                damageModels_Elites[i].msTimeSeparation = eliteSeparationTime;
            }

            commonTarget_DamageModel.msTimeSeparation = commonSeparationTime;
        }
    }

}
