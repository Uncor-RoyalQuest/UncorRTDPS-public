using System;
using System.Collections.Generic;
using System.IO;
using UncorRTDPS.DpsModels;
using UncorRTDPS.Util;

namespace UncorRTDPS.Services.DamageHistory
{
    class RecentDamage : Service
    {
        private LinkedList<DamageModel> recentDamage = new LinkedList<DamageModel>();
        private string binaryFileRecentDamageFullPath = null;
        private object locker = new object();
        private const int MAX_RECENT_SIZE = 100;
        private const int DEFAULT_RECENT_SIZE = 20;
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
            recentDamageLimitSize = SInt.FromString(RTDPS_Settings.UncorRTDPS_Config.GetConfigVal("damageHistory_RecentDamageSize")) ?? DEFAULT_RECENT_SIZE;
            if (recentDamageLimitSize < 1)
                recentDamageLimitSize = DEFAULT_RECENT_SIZE;
            if (recentDamageLimitSize > MAX_RECENT_SIZE)
                recentDamageLimitSize = MAX_RECENT_SIZE;

            if (args == null || args.Length < 1)
                return ServiceResponseStatus.FAILED;
            binaryFileRecentDamageFullPath = args[0];

            ServiceResponseStatus status = ServiceResponseStatus.OK;
            if (File.Exists(binaryFileRecentDamageFullPath))
            {
                BinaryReader binaryReader = null;
                try
                {
                    binaryReader = new BinaryReader(File.OpenRead(binaryFileRecentDamageFullPath));
                    int len = binaryReader.ReadInt32();
                    for (int i = 0; i < len; i++)
                    {
                        DamageModel dm = new DamageModel();
                        dm.ReadObject(binaryReader);
                        recentDamage.AddLast(dm);
                    }
                }
                catch
                {
                    recentDamage.Clear();
                    status = ServiceResponseStatus.FAILED;
                }
                finally
                {
                    binaryReader?.Dispose();
                }
            }

            return status;
        }

        public ServiceResponseStatus CloseService()
        {
            RecentDamageUpdated = null;

            ServiceResponseStatus status = ServiceResponseStatus.OK;
            if (binaryFileRecentDamageFullPath != null)
            {
                lock (locker)
                {
                    try
                    {
                        using FileStream fs = new FileStream(binaryFileRecentDamageFullPath, FileMode.Create);
                        try
                        {
                            using BinaryWriter binaryWriter = new BinaryWriter(fs);
                            try
                            {
                                int len = recentDamage.Count;
                                binaryWriter.Write(len);
                                int i = 0;
                                foreach (DamageModel damageModel in recentDamage)
                                {
                                    damageModel.WriteObject(binaryWriter);
                                    i += 1;
                                    if (i >= len)
                                        break;
                                }
                            }
                            catch { status = ServiceResponseStatus.FAILED; }
                            finally { binaryWriter.Dispose(); }
                        }
                        catch { status = ServiceResponseStatus.FAILED; }
                        finally { fs.Dispose(); }

                    }
                    catch { status = ServiceResponseStatus.FAILED; }
                }
            }
            return status;
        }
    }
}
