using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using UncorRTDPS.DpsModels;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.Services.MobsIcons;

namespace UncorRTDPS.Services.DamageHistory
{
    class EntityViewModel
    {
        private NumberFormatInfo numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = " " };
        public ObservableCollection<Entity> Entities { get; private set; }
        private int datagridIds = 0;
        private MobsIconsService mobsIconsService = null;

        public EntityViewModel()
        {
            Entities = new ObservableCollection<Entity>();
            Service service = ServicesContainer.GetService("mobsIcons");
            if (service != null && service is MobsIconsService)
                mobsIconsService = service as MobsIconsService;
            InitNumberFormat();
        }

        public void LoadEntities(List<DamageModel> list)
        {
            Entities.Clear();
            datagridIds = 0;
            AddEntities(list);
        }

        public void AddEntities(List<DamageModel> list)
        {
            foreach (DamageModel dm in list)
            {
                AddEntity(dm);
            }
        }

        public void AddEntity(DamageModel dm)
        {
            Entities.Add(new Entity(
                datagridIds,
                mobsIconsService?.GetMobImage(dm.Target),
                dm.Target.originalName,
                dm.TotalDamage.ToString("#,0", numberFormatInfo_FancyLong),
                dm.CalcDps().ToString("#,0", numberFormatInfo_FancyLong),
                (dm.CalcDamageTime() / 1000).ToString("#,0", numberFormatInfo_FancyLong),
                dm.Hits.ToString("#,0", numberFormatInfo_FancyLong),
                dm.MaxHitDmg.ToString("#,0", numberFormatInfo_FancyLong),
                DateTimeOffset.FromUnixTimeMilliseconds(dm.TimeStart).DateTime.ToString(),
                DateTimeOffset.FromUnixTimeMilliseconds(dm.TimeLast).LocalDateTime.ToString()
                ));
            datagridIds += 1;
        }

        public void InitNumberFormat()
        {
            string nf = UncorRTDPS_Config.getConfigVal("numberFormat_ThrousandsSeparator");
            if (nf == null || nf.Length < 1 || nf.Equals("0"))
                nf = " ";
            numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = nf };
        }
    }
}
