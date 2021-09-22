using System.Collections.Generic;
using UncorRTDPS.DpsModels.TargetsDictionary;
using UncorRTDPS.UncorOCR;
using UncorRTDPS.UncorOCR.DamageOCR_Data;

namespace UncorRTDPS.DpsModels
{
    public class DamageTarget_PoolManager : ITargetDamageListener, IModelSettingsAvailable
    {
        private List<DamageModel> activePool_Bosses = new List<DamageModel>();
        private List<DamageModel> activePool_Elites = new List<DamageModel>();
        private object locker_BossesPool = new object();
        private object locker_ElitesPool = new object();

        private DamageModel[] bosses_buffer = new DamageModel[10];
        private DamageModel[] elites_buffer = new DamageModel[10];


        private List<DamageModel> damageModels_Bosses = new List<DamageModel>();
        private List<DamageModel> damageModels_Elites = new List<DamageModel>();
        private DamageModel commonTarget_DamageModel = new DamageModel();
        private long bossSeparationTime = 30000; //30 sec
        private long eliteSeparationTime = 15000; //15 sec
        private long commonSeparationTime = 3000; //3 sec

        public SearchTargetNameMethod searchTargetNameMethod;

        public enum SearchTargetNameMethod
        {
            HammingDistance, LevenshteinDistance
        }


        public DamageTarget_PoolManager(SearchTargetNameMethod searchTargetNameMethod)
        {

            List<Target> bossTargets = TargetsDictionary.TargetsDictionary.BossTargets;
            for (int i = 0; i < bossTargets.Count; i++)
            {
                DamageModel damageModel = new DamageModel();
                damageModel.InitDamageModel(bossTargets[i], bossSeparationTime);
                damageModels_Bosses.Add(damageModel);
            }

            List<Target> eliteTargets = TargetsDictionary.TargetsDictionary.EliteTargets;
            for (int i = 0; i < eliteTargets.Count; i++)
            {
                DamageModel damageModel = new DamageModel();
                damageModel.InitDamageModel(eliteTargets[i], eliteSeparationTime);
                damageModels_Elites.Add(damageModel);
            }

            this.searchTargetNameMethod = searchTargetNameMethod;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// (len, arr) 
        /// (0 , null) if no active
        /// </returns>
        public (int, DamageModel[]) GetActiveBosses_BufferedArray()
        {
            if (activePool_Bosses.Count < 1)
                return (0, null);

            int len = 0;
            lock (locker_BossesPool)
            {
                len = activePool_Bosses.Count;
                for (int i = 0; i < len; i++)
                    bosses_buffer[i] = activePool_Bosses[i];
            }
            return (len, bosses_buffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// (len, arr) 
        /// (0 , null) if no active
        /// </returns>
        public (int, DamageModel[]) GetActiveElites_BufferedArray()
        {
            if (activePool_Elites.Count < 1)
                return (0, null);

            int len = 0;
            lock (locker_ElitesPool)
            {
                len = activePool_Elites.Count;
                for (int i = 0; i < len; i++)
                    elites_buffer[i] = activePool_Elites[i];
            }
            return (len, elites_buffer);
        }

        public DamageModel CommonTarget { get { return commonTarget_DamageModel; } }

        /// <summary>
        /// I.e. the most recent
        /// </summary>
        /// <param name="switchTimePeriod"></param>
        /// <returns>null if no active boss</returns>
        public DamageModel GetMostActiveBoss(long switchTimePeriod)
        {
            if (activePool_Bosses.Count < 1)
                return null;

            DamageModel res = null;
            long lastDmgTime = 0;
            DamageModel tmp_dt_dm;
            lock (locker_BossesPool)
            {
                int len = activePool_Bosses.Count;
                for (int i = 0; i < len; i++)
                {
                    tmp_dt_dm = activePool_Bosses[i];
                    if (tmp_dt_dm.IsActive && tmp_dt_dm.TimeLast > lastDmgTime + switchTimePeriod)
                    {
                        lastDmgTime = tmp_dt_dm.TimeLast;
                        res = tmp_dt_dm;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// I.e. the most recent
        /// </summary>
        /// <param name="switchTimePeriod"></param>
        /// <returns>null if no active elite</returns>
        public DamageModel GetMostActiveElite(long switchTimePeriod)
        {
            if (activePool_Elites.Count < 1)
                return null;

            DamageModel res = null;
            long lastDmgTime = 0;
            DamageModel tmp_dt_dm;
            lock (locker_ElitesPool)
            {
                int len = activePool_Elites.Count;
                for (int i = 0; i < len; i++)
                {
                    tmp_dt_dm = activePool_Elites[i];
                    if (tmp_dt_dm.TimeLast > lastDmgTime + switchTimePeriod)
                    {
                        lastDmgTime = tmp_dt_dm.TimeLast;
                        res = tmp_dt_dm;
                    }
                }
            }

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
            DamageModel damageModel;
            for (int i = startPos; i < endPos; i++)
            {
                if (damageTargetList[i].damage == -1)
                    continue;

                searchFriendlyOriginalName = TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(damageTargetList[i].target);
                target = TargetsDictionary.TargetsDictionary.TryGetTargetFromSimpleDictionary(searchFriendlyOriginalName);
                if (target == null)
                {
                    //target wasnt found by simple search. try hamm/lev method search
                    switch (searchTargetNameMethod)
                    {
                        case SearchTargetNameMethod.HammingDistance:
                            target = TargetsDictionary.TargetsDictionary.TryGetTargetHammingMethodSearch(searchFriendlyOriginalName);
                            break;
                        case SearchTargetNameMethod.LevenshteinDistance:
                            target = TargetsDictionary.TargetsDictionary.TryGetTargetLevenshteinMethodSearch(searchFriendlyOriginalName);
                            break;
                    }
                }
                if (target == null)
                {
                    //target wasnt found by simple search or distance method.
                    //means it is possibly "common" target
                    commonTarget_DamageModel.AddDamage(damageTargetList[i].damage, dmgTime);
                    commonTarget_DamageModel.IsActive = true;
                }
                else
                {
                    //switch between boss and elite
                    switch (target.targetType)
                    {
                        case TargetType.Boss:
                            //get damage model for this target
                            damageModel = damageModels_Bosses[target.idInsideTargetType];
                            //add damage into this damage model
                            damageModel.AddDamage(damageTargetList[i].damage, dmgTime);
                            //check is this damage model already active
                            //if not active then add to active pool
                            if (!damageModel.IsActive)
                                AddToActivePool_Bosses(damageModel);
                            break;
                        case TargetType.Elite:
                            //get damage model for this target
                            damageModel = damageModels_Elites[target.idInsideTargetType];
                            //add damage into this damage model
                            damageModel.AddDamage(damageTargetList[i].damage, dmgTime);
                            //check is this damage model already active
                            //if not active then add to active pool
                            if (!damageModel.IsActive)
                                AddToActivePool_Elites(damageModel);
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
        }

        /// <summary>
        /// dm.target!=null
        /// </summary>
        /// <param name="dm"></param>
        private void AddToActivePool_Bosses(DamageModel dm)
        {
            lock (locker_BossesPool)
            {
                activePool_Bosses.Add(dm);
                dm.IsActive = true;
            }
        }

        /// <summary>
        /// dm.target!=null
        /// </summary>
        /// <param name="dm"></param>
        private void AddToActivePool_Elites(DamageModel dm)
        {
            lock (locker_ElitesPool)
            {
                activePool_Elites.Add(dm);
                dm.IsActive = true;
            }
        }

        private void CheckActivePoolBossesForExpired(long timeToCompare)
        {
            int len = activePool_Bosses.Count;
            for (int i = 0; i < len; i++)
            {
                if (activePool_Bosses[i].IsDamageExpired(timeToCompare))
                {
                    lock (locker_BossesPool)
                    {
                        activePool_Bosses[i].IsActive = false;
                        activePool_Bosses.RemoveAt(i);
                        len -= 1;
                    }
                }
            }
        }

        private void CheckActivePoolElitesForExpired(long timeToCompare)
        {
            int len = activePool_Elites.Count;
            for (int i = 0; i < len; i++)
            {
                if (activePool_Elites[i].IsDamageExpired(timeToCompare))
                {
                    lock (locker_ElitesPool)
                    {
                        activePool_Elites[i].IsActive = false;
                        activePool_Elites.RemoveAt(i);
                        len -= 1;
                    }
                }
            }
        }

        public void RecieveTargetDamage(List<DamageTarget> damageTargets, int posStart, int posEnd, long dmgTime)
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
                damageModels_Bosses[i].MsTimeSeparation = bossSeparationTime;
            }

            for (int i = 0; i < damageModels_Elites.Count; i++)
            {
                damageModels_Elites[i].MsTimeSeparation = eliteSeparationTime;
            }

            commonTarget_DamageModel.MsTimeSeparation = commonSeparationTime;
        }
    }

}
