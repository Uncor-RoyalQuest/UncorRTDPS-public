using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UncorRTDPS.DpsModels;

namespace UncorRTDPS.UncorRTDPS_Windows.DpsPanel
{
    public class Manager_DamageInfoSequence
    {
        private List<Panel_DamageInfo> panel_DamageInfos = new List<Panel_DamageInfo>();
        private StackPanel damageInfosContainer;

        public Manager_DamageInfoSequence(StackPanel damageInfosContainer)
        {
            this.damageInfosContainer = damageInfosContainer;
        }

        public void RefreshHotkeysOnPanels()
        {
            foreach (Panel_DamageInfo pdi in panel_DamageInfos)
            {
                pdi.UnregisterHotkeys();
                pdi.RegisterHotkeys();
            }
        }

        public void UnregisterHotkeysOnPanels()
        {
            foreach (Panel_DamageInfo pdi in panel_DamageInfos)
                pdi.UnregisterHotkeys();
        }

        public void RegisterHotkeysOnPanels()
        {
            foreach (Panel_DamageInfo pdi in panel_DamageInfos)
                pdi.RegisterHotkeys();
        }

        public void ShowFirstNonCommonTargetDetailedDamage()
        {
            if (panel_DamageInfos.Count > 0)
            {
                for (int i = 0; i < panel_DamageInfos.Count && panel_DamageInfos[i].Visibility == Visibility.Visible; i++)
                {
                    if (panel_DamageInfos[i].GetLastTarget() != null)
                    {
                        panel_DamageInfos[i].OpenDamageChart();
                        break;
                    }
                }
            }
        }

        public void UpdatePanelsVisibilityParameters()
        {
            for (int i = 0; i < panel_DamageInfos.Count; i++)
            {
                UpdateVisivilityForPanel(panel_DamageInfos[i]);
            }
        }

        private void UpdateVisivilityForPanel(Panel_DamageInfo p)
        {
            switch (RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.DpsViewMode)
            {
                case 0:
                    UpdVisibilityForPanel_Mode_0(p);
                    break;
                case 1:
                    UpdVisibilityForPanel_Mode_1(p);
                    break;
            }
        }

        private void UpdVisibilityForPanel_Mode_0(Panel_DamageInfo p)
        {
            Visibility v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDamage_mode_0 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_DMG.Visibility = v;
            p.TextBlock_DMG_PercentValue.Visibility = v;
            p.TextBlock_DMG_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowHits_mode_0 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_Hits.Visibility = v;
            p.TextBlock_Hits_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowMaxHitDmg_mode_0 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_MaxHitDmg.Visibility = v;
            p.TextBlock_MaxHitDmg_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowT_mode_0 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_T.Visibility = v;
            p.TextBlock_T_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDps_mode_0 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_DPS.Visibility = v;
            p.TextBlock_DPS_Value.Visibility = v;
        }

        private void UpdVisibilityForPanel_Mode_1(Panel_DamageInfo p)
        {
            Visibility v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDamage_mode_1 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_DMG.Visibility = v;
            p.TextBlock_DMG_PercentValue.Visibility = v;
            p.TextBlock_DMG_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowHits_mode_1 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_Hits.Visibility = v;
            p.TextBlock_Hits_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowMaxHitDmg_mode_1 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_MaxHitDmg.Visibility = v;
            p.TextBlock_MaxHitDmg_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowT_mode_1 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_T.Visibility = v;
            p.TextBlock_T_Value.Visibility = v;

            v = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDps_mode_1 ? Visibility.Visible : Visibility.Collapsed;
            p.TextBlock_DPS.Visibility = v;
            p.TextBlock_DPS_Value.Visibility = v;
        }

        private Panel_DamageInfo CreateNewPanel_DamageInfo()
        {
            Panel_DamageInfo panel_DamageInfo = new Panel_DamageInfo();
            panel_DamageInfo.Visibility = System.Windows.Visibility.Collapsed;
            damageInfosContainer.Children.Add(panel_DamageInfo);
            UpdateVisivilityForPanel(panel_DamageInfo);
            return panel_DamageInfo;
        }

        private void FillListPanelsUntilLength(int len)
        {
            while (panel_DamageInfos.Count < len)
            {
                Panel_DamageInfo panel_DamageInfo = CreateNewPanel_DamageInfo();
                panel_DamageInfos.Add(panel_DamageInfo);
                if (panel_DamageInfos.Count > 1)
                    panel_DamageInfo.Margin = new System.Windows.Thickness(0, 10, 0, 0);
            }
        }

        /// <summary>
        /// With visibility turns on/off
        /// </summary>
        /// <param name="damageModel"></param>
        public void UpdateDamageInfo(DamageModel damageModel)
        {
            //create new if needed, therefore provide confident access to [0] element 
            FillListPanelsUntilLength(1);

            //update data on the corresponding panel
            panel_DamageInfos[0].UpdateViewData(damageModel, damageModel.Target);

            //make visible all needed panels
            SetPanelsVisibility(0, 1, System.Windows.Visibility.Visible);
            //make invisible all unneeded panels
            SetPanelsVisibility(1, panel_DamageInfos.Count, System.Windows.Visibility.Collapsed);
        }


        /// <summary>
        /// No visibility management
        /// </summary>
        /// <param name="damageModel"></param>
        /// <param name="posAt"></param>
        public void UpdateDamageInfo(DamageModel damageModel, int posAt)
        {
            //create new if needed, therefore provide confident access to [0] element 
            FillListPanelsUntilLength(posAt + 1);

            //update data on the corresponding panel
            panel_DamageInfos[posAt].UpdateViewData(damageModel, damageModel.Target);
        }

        public void SetPanelsVisibility(int posStart, int posEnd, System.Windows.Visibility visibility)
        {
            int max = posEnd > panel_DamageInfos.Count ? panel_DamageInfos.Count : posEnd;
            for (int i = posStart; i < max; i++)
            {
                if (panel_DamageInfos[i].Visibility != visibility)
                {
                    panel_DamageInfos[i].Visibility = visibility;
                }
            }
        }
    }
}
