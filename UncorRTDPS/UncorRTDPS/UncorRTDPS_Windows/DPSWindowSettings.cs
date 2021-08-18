namespace UncorRTDPS.UncorRTDPS_Windows
{
    public class DPSWindowSettings
    {
        public int FontSize { get; set; } = 11;
        public float Opacity { get; set; } = 1;
        public long VisualRefreshDelay { get; set; } = 500; //ms
        public bool ShowOcrStat { get; set; } = true; //

        public int ScreenPositionX { get; set; } = 100;
        public int ScreenPositionY { get; set; } = 100;

        //
        public int DpsViewMode { get; set; }

        //
        public bool EnableAliases { get; set; }

        public bool ShowDamage_mode_0 { get; set; }
        public bool ShowHits_mode_0 { get; set; }
        public bool ShowT_mode_0 { get; set; }
        public bool ShowDps_mode_0 { get; set; }

        public bool ShowDamage_mode_1 { get; set; }
        public bool ShowHits_mode_1 { get; set; }
        public bool ShowT_mode_1 { get; set; }
        public bool ShowDps_mode_1 { get; set; }
        public int BossesLimit_mode_1 { get; set; }
        public int ElitesLimit_mode_1 { get; set; }
        public bool ShowCommonMobsDps_mode_1 { get; set; }

        public bool ShowOcrStat_Failures { get; set; }
        public bool ShowOcrStat_Losses { get; set; }
        public bool ShowOcrStat_RPS { get; set; }
        public bool ShowOcrStat_ART { get; set; }
        public bool ShowOcrStat_ALoad { get; set; }
        public bool ShowOcrStat_MLoad { get; set; }

        public DPSWindowSettings() { }

        public DPSWindowSettings(DPSWindowSettings s)
        {
            this.FontSize = s.FontSize;
            this.Opacity = s.Opacity;
            this.VisualRefreshDelay = s.VisualRefreshDelay;
            this.ShowOcrStat = s.ShowOcrStat;
            this.ScreenPositionX = s.ScreenPositionX;
            this.ScreenPositionY = s.ScreenPositionY;

            this.DpsViewMode = s.DpsViewMode;

            this.EnableAliases = s.EnableAliases;

            this.ShowDamage_mode_0 = s.ShowDamage_mode_0;
            this.ShowHits_mode_0 = s.ShowHits_mode_0;
            this.ShowT_mode_0 = s.ShowT_mode_0;
            this.ShowDps_mode_0 = s.ShowDps_mode_0;

            this.ShowDamage_mode_1 = s.ShowDamage_mode_1;
            this.ShowHits_mode_1 = s.ShowHits_mode_1;
            this.ShowT_mode_1 = s.ShowT_mode_1;
            this.ShowDps_mode_1 = s.ShowDps_mode_1;
            this.BossesLimit_mode_1 = s.BossesLimit_mode_1;
            this.ElitesLimit_mode_1 = s.ElitesLimit_mode_1;
            this.ShowCommonMobsDps_mode_1 = s.ShowCommonMobsDps_mode_1;

            this.ShowOcrStat_Failures = s.ShowOcrStat_Failures;
            this.ShowOcrStat_Losses = s.ShowOcrStat_Losses;
            this.ShowOcrStat_RPS = s.ShowOcrStat_RPS;
            this.ShowOcrStat_ART = s.ShowOcrStat_ART;
            this.ShowOcrStat_ALoad = s.ShowOcrStat_ALoad;
            this.ShowOcrStat_MLoad = s.ShowOcrStat_MLoad;
        }


        public static bool IsEquals(DPSWindowSettings s1, DPSWindowSettings s2)
        {
            if (s1.FontSize == s2.FontSize &&
                s1.Opacity == s2.Opacity &&
                s1.VisualRefreshDelay == s2.VisualRefreshDelay &&
                s1.ShowOcrStat == s2.ShowOcrStat &&
                s1.ScreenPositionX == s2.ScreenPositionX &&
                s1.ScreenPositionY == s2.ScreenPositionY &&
                s1.DpsViewMode == s2.DpsViewMode &&
                s1.EnableAliases == s2.EnableAliases &&
                s1.ShowDamage_mode_0 == s2.ShowDamage_mode_0 &&
                s1.ShowHits_mode_0 == s2.ShowHits_mode_0 &&
                s1.ShowT_mode_0 == s2.ShowT_mode_0 &&
                s1.ShowDps_mode_0 == s2.ShowDps_mode_0 &&
                s1.ShowDamage_mode_1 == s2.ShowDamage_mode_1 &&
                s1.ShowHits_mode_1 == s2.ShowHits_mode_1 &&
                s1.ShowT_mode_1 == s2.ShowT_mode_1 &&
                s1.ShowDps_mode_1 == s2.ShowDps_mode_1 &&
                s1.BossesLimit_mode_1 == s2.BossesLimit_mode_1 &&
                s1.ElitesLimit_mode_1 == s2.ElitesLimit_mode_1 &&
                s1.ShowCommonMobsDps_mode_1 == s2.ShowCommonMobsDps_mode_1 &&
                s1.ShowOcrStat_Failures == s2.ShowOcrStat_Failures &&
                s1.ShowOcrStat_Losses == s2.ShowOcrStat_Losses &&
                s1.ShowOcrStat_RPS == s2.ShowOcrStat_RPS &&
                s1.ShowOcrStat_ART == s2.ShowOcrStat_ART &&
                s1.ShowOcrStat_ALoad == s2.ShowOcrStat_ALoad &&
                s1.ShowOcrStat_MLoad == s2.ShowOcrStat_MLoad)
                return true;
            return false;
        }
    }
}
