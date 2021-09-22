using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UncorRTDPS.Util.Serialization;

namespace UncorRTDPS.DpsModels
{
    public class DamageSequence : ICustomBinarySerializable
    {
        private object locker = new object();
        private List<Damage> damageSequence = new List<Damage>();

        public DamageSequence() { }

        public DamageSequence(List<Damage> damageSequence)
        {
            this.damageSequence = new List<Damage>(damageSequence);
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        /// <returns></returns>
        public DamageSequence Clone()
        {
            DamageSequence res;
            lock (locker)
            {
                res = new DamageSequence(damageSequence);
            }
            return res;
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        /// <returns></returns>
        public void AddDamageToSequence(long damage, long time)
        {
            Damage dmg = new Damage(damage, time);
            lock (locker)
            {
                damageSequence.Add(dmg);
            }
        }

        /// <summary>
        /// Thread safe
        /// </summary>
        /// <returns></returns>
        public void ClearDamageSequence()
        {
            lock (locker)
            {
                damageSequence.Clear();
            }
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public long GetDamageDurationInMs()
        {
            if (damageSequence.Count < 2)
                return 0;

            long timeStart = damageSequence[0].Time;
            long timeEnd = damageSequence[damageSequence.Count - 1].Time;
            return Math.Abs(timeEnd - timeStart);
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public long CalcDps()
        {
            long dmgDurationInSec = GetDamageDurationInMs() / 1000L;
            if (dmgDurationInSec == 0L)
                dmgDurationInSec = 1L;
            //long dps = damageSequence.GetDamage().Sum() / (dmgDuration == 0L ? 1L : dmgDuration);
            long damage = damageSequence.Sum(i => i.Dmg);
            return damage / dmgDurationInSec;
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public long CalcAverageHit()
        {
            return (long)damageSequence.Average(i => i.Dmg);
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public long CalcSumDamage()
        {
            return damageSequence.Sum(i => i.Dmg);
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public long GetHitsCount()
        {
            return damageSequence.Count;
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public double[] GetDamage()
        {
            int len = damageSequence.Count;
            double[] res = new double[len];
            for (int i = 0; i < len; i++)
                res[i] = damageSequence[i].Dmg;
            return res;
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public double[] GetTime()
        {
            int len = damageSequence.Count;
            double[] res = new double[len];
            for (int i = 0; i < len; i++)
                res[i] = damageSequence[i].Time;
            return res;
        }

        /// <summary>
        /// NOT thread safe
        /// </summary>
        /// <returns></returns>
        public double[] GetDamageGroupedIntoSeconds()
        {
            if (damageSequence.Count < 1)
                return new List<double>().ToArray();

            long timeStart = damageSequence[0].Time;
            List<double> res = new List<double>();
            int dmgLen = damageSequence.Count;
            for (int i = 0; i < dmgLen; i++)
            {
                int dmgSecond = (int)((damageSequence[i].Time - timeStart) / 1000L);
                while (res.Count - 1 < dmgSecond)
                    res.Add(0.0);
                res[dmgSecond] += damageSequence[i].Dmg;
            }

            return res.ToArray();
        }

        public void ReadObject(BinaryReader binaryReader)
        {
            int len = binaryReader.ReadInt32();
            for (int i = 0; i < len; i++)
            {
                long dmg = binaryReader.ReadInt64();
                long time = binaryReader.ReadInt64();
                damageSequence.Add(new Damage(dmg, time));
            }
        }

        public void WriteObject(BinaryWriter binaryWriter)
        {
            lock (locker)
            {
                int len = damageSequence.Count;
                binaryWriter.Write(len);
                for (int i = 0; i < len; i++)
                {
                    binaryWriter.Write(damageSequence[i].Dmg);
                    binaryWriter.Write(damageSequence[i].Time);
                }
            }
        }
    }
}
