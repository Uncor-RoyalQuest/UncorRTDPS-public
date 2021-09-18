using System;
using System.Collections.Generic;
using UncorRTDPS.DpsModels;
using UncorRTDPS.Util;

namespace UncorRTDPS.Services.DamageHistory
{
    class RecentDamage : Service
    {
        private LinkedList<DamageModel> recentDamage = new LinkedList<DamageModel>();
        private object locker = new object();
        private const int MAX_RECENT_SIZE = 100;
        private const int DEFAULT_RECENT_SIZE = 10;
        private int recentDamageLimitSize = 10;

        public event EventHandler RecentDamageUpdated;

        public void InvokeRecentDamageUpdated(object sender, EventArgs e)
        {
            RecentDamageUpdated?.Invoke(sender, e);
        }

        public void AddNewDamageToRecent(DamageModel damageModel)
        {
            lock (locker)
            {
                if (recentDamage.Count >= recentDamageLimitSize)
                {
                    recentDamage.RemoveLast();
                }
                recentDamage.AddFirst(damageModel);
            }
            InvokeRecentDamageUpdated(this, null);
        }

        public void ReplaceNewDamageInRecent(DamageModel damageModelOld, DamageModel damageModelNew)
        {
            lock (locker)
            {
                LinkedListNode<DamageModel> node = recentDamage.Find(damageModelOld);
                if (node != null)
                {
                    node.Value = damageModelNew;
                }
            }
            InvokeRecentDamageUpdated(this, null);
        }

        public List<DamageModel> GetClonedRecentDamage()
        {
            List<DamageModel> res;
            lock (locker)
            {
                res = new List<DamageModel>(recentDamage);
            }
            return res;
        }

        public ServiceResponseStatus InitService(string[] args)
        {
            recentDamageLimitSize = SInt.FromString(RTDPS_Settings.UncorRTDPS_Config.getConfigVal("damageHistory_RecentDamageSize")) ?? DEFAULT_RECENT_SIZE;
            if (recentDamageLimitSize < 1)
                recentDamageLimitSize = DEFAULT_RECENT_SIZE;
            if (recentDamageLimitSize > MAX_RECENT_SIZE)
                recentDamageLimitSize = MAX_RECENT_SIZE;
            return ServiceResponseStatus.OK;
        }

        public ServiceResponseStatus CloseService()
        {
            RecentDamageUpdated = null;
            return ServiceResponseStatus.OK;
        }
    }
}
