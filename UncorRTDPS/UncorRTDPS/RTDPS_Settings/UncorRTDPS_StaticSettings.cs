using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using UncorRTDPS.UncorRTDPS_Windows;
using UncorRTDPS.Util;

namespace UncorRTDPS.RTDPS_Settings
{
    public static class UncorRTDPS_StaticSettings
    {
        public enum Languages
        {
            Russian, English
        }

        /// <summary>
        /// Folder with the resources tree (locale, src, config, etc.)
        /// </summary>
        public static string ResourcesPath { get; set; }
        public static string ConfigFileRTDPS { get; set; }
        public static Languages Language_UI { get; set; }

        //
        public static Screencap.SelectedArea SelectedAreaRTDPS { get; set; }
        public static UncorOCR.OCRSettings OcrSettings { get; set; }
        public static DpsModels.ModelSettings DpsModelSettings { get; set; }
        public static DPSWindowSettings DpsWindowSettings { get; set; }
        public static UncorOCR.Supplementary_Parameters_DamageOCR Supplementary_Parameters_DamageOCR { get; set; }

        //
        public static BitmapImage BiCircleGray { get; set; }
        public static BitmapImage BiCircleGreen { get; set; }
        public static BitmapImage BiCircleLoadingGray { get; set; }
        public static BitmapImage BiGears { get; set; }
        public static BitmapImage BiLockGray { get; set; }
        public static BitmapImage BiLockLockedGray { get; set; }
        public static BitmapImage BiDamageHistory { get; set; }
        public static BitmapImage BiWClose { get; set; }
        //

        public static BitmapImage BiSquare_Question { get; set; }

        public static BitmapImage BiSquare_Icon_TopLeft { get; set; }
        public static BitmapImage BiSquare_Icon_TopRight { get; set; }
        public static BitmapImage BiSquare_Icon_BotLeft { get; set; }
        public static BitmapImage BiSquare_Icon_BotRight { get; set; }

        public static BitmapImage BiArrow_Icon_Top { get; set; }
        public static BitmapImage BiArrow_Icon_Bot { get; set; }
        public static BitmapImage BiArrow_Icon_Left { get; set; }
        public static BitmapImage BiArrow_Icon_Right { get; set; }

        public static System.Drawing.Image ImgCursor { get; set; }

        //
        public static double ScreenScaleFactorByDpi_X { get; set; }
        public static double ScreenScaleFactorByDpi_Y { get; set; }
        //

        //
        public static double RpsMonitoringLimit_Active { get; set; }
        public static double RpsMonitoringLimit_Inactive { get; set; }

        static UncorRTDPS_StaticSettings()
        {
            var dpiXProperty = typeof(System.Windows.SystemParameters).GetProperty("DpiX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var dpiYProperty = typeof(System.Windows.SystemParameters).GetProperty("Dpi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            var dpiX = (int)dpiXProperty.GetValue(null, null);
            var dpiY = (int)dpiYProperty.GetValue(null, null);

            ScreenScaleFactorByDpi_X = (double)dpiX / 96;
            ScreenScaleFactorByDpi_Y = (double)dpiY / 96;
            if (ScreenScaleFactorByDpi_X < 0.01)
                ScreenScaleFactorByDpi_X = 1.0;
            if (ScreenScaleFactorByDpi_Y < 0.01)
                ScreenScaleFactorByDpi_Y = 1.0;
        }

        public static void InitImages()
        {
            //p1
            BiCircleGray = new BitmapImage(
                new Uri(
                    Path.GetFullPath(
                        Path.Combine(ResourcesPath, @"src\icons_rtdps\circle_16x16_gray.png")
                        )
                    )
                );

            BiCircleGreen = new BitmapImage(
                new Uri(
                    Path.GetFullPath(
                        Path.Combine(ResourcesPath, @"src\icons_rtdps\circle_16x16_green.png")
                        )
                    )
                );

            BiCircleLoadingGray = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\circleL_16x16_gray.png")
                       )
                   )
               );

            BiGears = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\wrench_16x16_gray.png")
                       )
                   )
               );

            BiLockGray = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\lock_10x16_gray_v2.png")
                       )
                   )
               );

            BiLockLockedGray = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\lock_10x16_locked_gray.png")
                       )
                   )
               );

            BiDamageHistory = new BitmapImage(
                new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\history_16x16_gray.png")
                       )
                   )
                );

            BiWClose = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\wclose_16x16_gray.png")
                       )
                   )
               );



            //p2
            BiSquare_Question = new BitmapImage(
               new Uri(
                   Path.GetFullPath(
                       Path.Combine(ResourcesPath, @"src\icons_rtdps\square_48x48_question.png")
                       )
                   )
               );

            BiSquare_Icon_TopLeft = new BitmapImage(
              new Uri(
                  Path.GetFullPath(
                      Path.Combine(ResourcesPath, @"src\icons_rtdps\square_8x8_topleft_gray.png")
                      )
                  )
              );

            BiSquare_Icon_TopRight = new BitmapImage(
              new Uri(
                  Path.GetFullPath(
                      Path.Combine(ResourcesPath, @"src\icons_rtdps\square_8x8_topright_gray.png")
                      )
                  )
              );

            BiSquare_Icon_BotLeft = new BitmapImage(
              new Uri(
                  Path.GetFullPath(
                      Path.Combine(ResourcesPath, @"src\icons_rtdps\square_8x8_botleft_gray.png")
                      )
                  )
              );

            BiSquare_Icon_BotRight = new BitmapImage(
              new Uri(
                  Path.GetFullPath(
                      Path.Combine(ResourcesPath, @"src\icons_rtdps\square_8x8_botright_gray.png")
                      )
                  )
              );

            BiArrow_Icon_Top = new BitmapImage(
              new Uri(
                  Path.GetFullPath(
                      Path.Combine(ResourcesPath, @"src\icons_rtdps\arrow_8x8_top_gray.png")
                      )
                  )
              );

            BiArrow_Icon_Bot = new BitmapImage(
             new Uri(
                 Path.GetFullPath(
                     Path.Combine(ResourcesPath, @"src\icons_rtdps\arrow_8x8_bot_gray.png")
                     )
                 )
             );

            BiArrow_Icon_Left = new BitmapImage(
             new Uri(
                 Path.GetFullPath(
                     Path.Combine(ResourcesPath, @"src\icons_rtdps\arrow_8x8_left_gray.png")
                     )
                 )
             );

            BiArrow_Icon_Right = new BitmapImage(
             new Uri(
                 Path.GetFullPath(
                     Path.Combine(ResourcesPath, @"src\icons_rtdps\arrow_8x8_right_gray.png")
                     )
                 )
             );

            ImgCursor = System.Drawing.Image.FromFile(
                Path.GetFullPath(
                     Path.Combine(ResourcesPath, @"src\icons_rtdps\cursor_8x14.png")
                     )
                );
        }

        public static void InitFromConfig()
        {
            //selected area
            SelectedAreaRTDPS = new Screencap.SelectedArea(
                UncorRTDPS_Config.getConfigVal("sa_Xtopleft"),
                UncorRTDPS_Config.getConfigVal("sa_Ytopleft"),
                UncorRTDPS_Config.getConfigVal("sa_Xbotright"),
                UncorRTDPS_Config.getConfigVal("sa_Ybotright")
                );

            //monitoring rps limits
            double rpsLimit = SDouble.FromString(UncorRTDPS_Config.getConfigVal("monitoring_rpsLimit_Active"));
            if (double.IsNaN(rpsLimit) || rpsLimit < 0.5)
            {
                rpsLimit = 3;
            }
            RpsMonitoringLimit_Active = rpsLimit;

            rpsLimit = SDouble.FromString(UncorRTDPS_Config.getConfigVal("monitoring_rpsLimit_Inactive"));
            if (double.IsNaN(rpsLimit) || rpsLimit < 0.5)
            {
                rpsLimit = 1.5;
            }
            RpsMonitoringLimit_Inactive = rpsLimit;

            //ocr
            float f = SFloat.FromString(UncorRTDPS_Config.getConfigVal("ocr_brightness_barrier"));
            OcrSettings = new UncorOCR.OCRSettings();
            if (float.IsNaN(f))
            {
                OcrSettings.SetBrightnessBarrier(0.25f);
            }
            else
            {
                OcrSettings.SetBrightnessBarrier(f);
            }

            f = SFloat.FromString(UncorRTDPS_Config.getConfigVal("ocr_image_scaling"));
            if (float.IsNaN(f))
            {
                OcrSettings.SetImageScaling(1.5f);
            }
            else
            {
                OcrSettings.SetImageScaling(f);
            }

            string s = UncorRTDPS_Config.getConfigVal("ocr_lang");
            if (s.Equals("ru"))
            {
                OcrSettings.SetLang(Languages.Russian);
            }
            else if (s.Equals("en"))
            {
                OcrSettings.SetLang(Languages.English);
            }
            else
            {
                OcrSettings.SetLang(Languages.English);
            }

            //ocr supplementary params
            int averageCharacterWidth = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_avgCharWidth")) ?? -1;

            int garbageWidthAfterDamage = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_garbWidthAfterDmg")) ?? -1;

            int maximumRowHeight = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_maxRowHeight")) ?? -1;

            int averageWhiteSpaceBetweenRows = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_avgWhiteBetwnRows")) ?? -1;

            int damageWordWidth = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_dmgWordWidth")) ?? -1;

            int targetWordWidth = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_targetWordWidth")) ?? -1;

            int verticallyDataStart = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_vertDataStart")) ?? -1;

            int bitmapHeight = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_bmpHeight")) ?? -1;

            int bitmapWidth = SInt.FromString(UncorRTDPS_Config.getConfigVal("ocr_suppl_bmpWidth")) ?? -1;

            Supplementary_Parameters_DamageOCR = new UncorOCR.Supplementary_Parameters_DamageOCR(
                averageCharacterWidth, garbageWidthAfterDamage,
                maximumRowHeight, averageWhiteSpaceBetweenRows,
                damageWordWidth, targetWordWidth,
                verticallyDataStart,
                bitmapHeight, bitmapWidth);

            //damage model
            long
                longDelayBoss = SLong.FromString(UncorRTDPS_Config.getConfigVal("dpsModel_bossSeparationDelay")) ?? 30000,
                longDelayElite = SLong.FromString(UncorRTDPS_Config.getConfigVal("dpsModel_eliteSeparationDelay")) ?? 15000,
                longDelayCommon = SLong.FromString(UncorRTDPS_Config.getConfigVal("dpsModel_commonSeparationDelay")) ?? 3000;

            DpsModelSettings = new DpsModels.ModelSettings(longDelayBoss, longDelayElite, longDelayCommon);

            //hovering stats window
            DpsWindowSettings = new UncorRTDPS_Windows.DPSWindowSettings();
            int? nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_fontSize"));
            if (nIntVal == null)
                DpsWindowSettings.FontSize = 11;
            else
                DpsWindowSettings.FontSize = nIntVal.Value;

            f = SFloat.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_Opacity"));
            if (float.IsNaN(f) || f < 0.1f || f > 1f)
                DpsWindowSettings.Opacity = 1f;
            else
                DpsWindowSettings.Opacity = f;

            long? nLongVal = SLong.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_refreshDelay"));
            if (nLongVal == null)
                DpsWindowSettings.VisualRefreshDelay = 500;
            else
                DpsWindowSettings.VisualRefreshDelay = nLongVal.Value;

            bool boolVal;
            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_showOcrStat"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat = false;
            }

            nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_X"));
            if (nIntVal == null)
                DpsWindowSettings.ScreenPositionX = 100;
            else
                DpsWindowSettings.ScreenPositionX = nIntVal.Value;

            nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_Y"));
            if (nIntVal == null)
                DpsWindowSettings.ScreenPositionY = 100;
            else
                DpsWindowSettings.ScreenPositionY = nIntVal.Value;
            //


            nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_DpsViewMode"));
            if (nIntVal == null)
                DpsWindowSettings.DpsViewMode = 0;
            else
                DpsWindowSettings.DpsViewMode = nIntVal.Value;

            //dpsWindow_EnableAliases
            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_EnableAliases"), out boolVal))
                DpsWindowSettings.EnableAliases = boolVal;
            else
                DpsWindowSettings.EnableAliases = false;


            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowDamage_Mode_0"), out boolVal))
            {
                DpsWindowSettings.ShowDamage_mode_0 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowDamage_mode_0 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowHits_Mode_0"), out boolVal))
            {
                DpsWindowSettings.ShowHits_mode_0 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowHits_mode_0 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowMaxHitDmg_Mode_0"), out boolVal))
            {
                DpsWindowSettings.ShowMaxHitDmg_mode_0 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowMaxHitDmg_mode_0 = false;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowT_Mode_0"), out boolVal))
            {
                DpsWindowSettings.ShowT_mode_0 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowT_mode_0 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowDps_Mode_0"), out boolVal))
            {
                DpsWindowSettings.ShowDps_mode_0 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowDps_mode_0 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowDamage_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowDamage_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowDamage_mode_1 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowHits_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowHits_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowHits_mode_1 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowMaxHitDmg_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowMaxHitDmg_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowMaxHitDmg_mode_1 = false;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowT_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowT_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowT_mode_1 = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowDps_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowDps_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowDps_mode_1 = true;
            }


            nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_BossesLimit_Mode_1"));
            if (nIntVal == null)
                DpsWindowSettings.BossesLimit_mode_1 = 2;
            else
                DpsWindowSettings.BossesLimit_mode_1 = nIntVal.Value;


            nIntVal = SInt.FromString(UncorRTDPS_Config.getConfigVal("dpsWindow_ElitesLimit_Mode_1"));
            if (nIntVal == null)
                DpsWindowSettings.ElitesLimit_mode_1 = 2;
            else
                DpsWindowSettings.ElitesLimit_mode_1 = nIntVal.Value;

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_ShowCommonMobsDps_Mode_1"), out boolVal))
            {
                DpsWindowSettings.ShowCommonMobsDps_mode_1 = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowCommonMobsDps_mode_1 = true;
            }


            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowFailures"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_Failures = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_Failures = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowLosses"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_Losses = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_Losses = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowRPS"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_RPS = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_RPS = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowART"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_ART = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_ART = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowALoad"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_ALoad = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_ALoad = true;
            }

            if (bool.TryParse(UncorRTDPS_Config.getConfigVal("dpsWindow_OcrStat_ShowMLoad"), out boolVal))
            {
                DpsWindowSettings.ShowOcrStat_MLoad = boolVal;
            }
            else
            {
                DpsWindowSettings.ShowOcrStat_MLoad = true;
            }

        }

        public static void UpdateSelectedAreaRTDPS(Screencap.SelectedArea sa)
        {
            if (Screencap.SelectedArea.IsEquals(SelectedAreaRTDPS, sa))
                return;

            SelectedAreaRTDPS.SetTopLeft(sa.X_topLeft, sa.Y_topLeft);
            SelectedAreaRTDPS.SetBotRight(sa.X_botRight, sa.Y_botRight);

            UncorRTDPS_Config.UpdateConfigVal("sa_Xtopleft", SInt.ToString(sa.X_topLeft));
            UncorRTDPS_Config.UpdateConfigVal("sa_Ytopleft", SInt.ToString(sa.Y_topLeft));
            UncorRTDPS_Config.UpdateConfigVal("sa_Xbotright", SInt.ToString(sa.X_botRight));
            UncorRTDPS_Config.UpdateConfigVal("sa_Ybotright", SInt.ToString(sa.Y_botRight));
            UncorRTDPS_Config.SaveConfigs();

            NotifyAllSelectedAreaChangedListeners();
        }

        public static void UpdateOCRSettings(UncorOCR.OCRSettings s)
        {
            if (UncorOCR.OCRSettings.IsEquals(OcrSettings, s))
                return;

            OcrSettings.SetBrightnessBarrier(s.OCR_brightnessBarrier);
            OcrSettings.SetImageScaling(s.OCR_imageScaling);
            OcrSettings.SetLang(s.Lang);

            UncorRTDPS_Config.UpdateConfigVal("ocr_brightness_barrier", SFloat.ToString(OcrSettings.OCR_brightnessBarrier, "{0:0.##}"));
            UncorRTDPS_Config.UpdateConfigVal("ocr_image_scaling", SFloat.ToString(OcrSettings.OCR_imageScaling, "{0:0.##}"));
            UncorRTDPS_Config.UpdateConfigVal("ocr_lang", OcrSettings.Lang == Languages.Russian ? "ru" : "en");
            UncorRTDPS_Config.SaveConfigs();

            notifyAllOCRSettingsChangedListeners();
        }

        public static void UpdateOCRSupplementaryParameters(UncorOCR.Supplementary_Parameters_DamageOCR supplementaryParams)
        {
            if (supplementaryParams == null)
                return;

            Supplementary_Parameters_DamageOCR = supplementaryParams;
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_avgCharWidth", SInt.ToString(supplementaryParams.AverageCharacterWidth));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_garbWidthAfterDmg", SInt.ToString(supplementaryParams.GarbageWidthAfterDamage));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_dmgWordWidth", SInt.ToString(supplementaryParams.DamageWordWidth));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_targetWordWidth", SInt.ToString(supplementaryParams.TargetWordWidth));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_maxRowHeight", SInt.ToString(supplementaryParams.MaximumRowHeight));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_avgWhiteBetwnRows", SInt.ToString(supplementaryParams.AverageWhiteSpaceBetweenRows));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_vertDataStart", SInt.ToString(supplementaryParams.VerticallyDataStart));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_bmpHeight", SInt.ToString(supplementaryParams.BitmapHeight));
            UncorRTDPS_Config.UpdateConfigVal("ocr_suppl_bmpWidth", SInt.ToString(supplementaryParams.BitmapWidth));
            UncorRTDPS_Config.SaveConfigs();
        }

        public static void UpdatePerformanceSettings(DpsModels.ModelSettings s, double rpsLimit_Active, double rpsLimit_Inactive)
        {
            if (DpsModels.ModelSettings.IsEquals(DpsModelSettings, s)
                && RpsMonitoringLimit_Active == rpsLimit_Active
                && RpsMonitoringLimit_Inactive == rpsLimit_Inactive)
                return;

            DpsModelSettings = new DpsModels.ModelSettings(s);
            RpsMonitoringLimit_Active = rpsLimit_Active;
            RpsMonitoringLimit_Inactive = rpsLimit_Inactive;

            UncorRTDPS_Config.UpdateConfigVal("dpsModel_bossSeparationDelay", SLong.ToString(DpsModelSettings.BossDamageSeparationDelayMS));
            UncorRTDPS_Config.UpdateConfigVal("dpsModel_eliteSeparationDelay", SLong.ToString(DpsModelSettings.EliteDamageSeparationDelayMS));
            UncorRTDPS_Config.UpdateConfigVal("dpsModel_commonSeparationDelay", SLong.ToString(DpsModelSettings.CommonDamageSeparationDelayMS));

            UncorRTDPS_Config.UpdateConfigVal("monitoring_rpsLimit_Active", SDouble.ToString(rpsLimit_Active, "{0:0.##}"));
            UncorRTDPS_Config.UpdateConfigVal("monitoring_rpsLimit_Inactive", SDouble.ToString(rpsLimit_Inactive, "{0:0.##}"));

            UncorRTDPS_Config.SaveConfigs();

            notifyAllDPSModelSettingsChangedListeners(); //Not implemented
        }

        public static void UpdateDPSWindowSettings(UncorRTDPS_Windows.DPSWindowSettings s)
        {
            if (DPSWindowSettings.IsEquals(DpsWindowSettings, s))
                return;

            DpsWindowSettings = new DPSWindowSettings(s);

            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_fontSize", SInt.ToString(DpsWindowSettings.FontSize));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_Opacity", SFloat.ToString(DpsWindowSettings.Opacity, "{0:0.##}"));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_refreshDelay", SLong.ToString(DpsWindowSettings.VisualRefreshDelay));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_showOcrStat", DpsWindowSettings.ShowOcrStat.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_X", SInt.ToString(DpsWindowSettings.ScreenPositionX));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_Y", SInt.ToString(DpsWindowSettings.ScreenPositionY));

            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_DpsViewMode", SInt.ToString(DpsWindowSettings.DpsViewMode));

            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_EnableAliases", DpsWindowSettings.EnableAliases.ToString());

            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowDamage_Mode_0", DpsWindowSettings.ShowDamage_mode_0.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowHits_Mode_0", DpsWindowSettings.ShowHits_mode_0.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowMaxHitDmg_Mode_0", DpsWindowSettings.ShowMaxHitDmg_mode_0.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowT_Mode_0", DpsWindowSettings.ShowT_mode_0.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowDps_Mode_0", DpsWindowSettings.ShowDps_mode_0.ToString());

            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowDamage_Mode_1", DpsWindowSettings.ShowDamage_mode_1.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowHits_Mode_1", DpsWindowSettings.ShowHits_mode_1.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowMaxHitDmg_Mode_1", DpsWindowSettings.ShowMaxHitDmg_mode_1.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowT_Mode_1", DpsWindowSettings.ShowT_mode_1.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowDps_Mode_1", DpsWindowSettings.ShowDps_mode_1.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_BossesLimit_Mode_1", SInt.ToString(DpsWindowSettings.BossesLimit_mode_1));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ElitesLimit_Mode_1", SInt.ToString(DpsWindowSettings.ElitesLimit_mode_1));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_ShowCommonMobsDps_Mode_1", DpsWindowSettings.ShowCommonMobsDps_mode_1.ToString());


            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowFailures", DpsWindowSettings.ShowOcrStat_Failures.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowLosses", DpsWindowSettings.ShowOcrStat_Losses.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowRPS", DpsWindowSettings.ShowOcrStat_RPS.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowART", DpsWindowSettings.ShowOcrStat_ART.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowALoad", DpsWindowSettings.ShowOcrStat_ALoad.ToString());
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_OcrStat_ShowMLoad", DpsWindowSettings.ShowOcrStat_MLoad.ToString());


            UncorRTDPS_Config.SaveConfigs();

            NotifyAllDPSWindowSettingsChangedListeners();
        }

        public static List<ISelectedAreaChangedListener> selectedAreaChangedListeners = new List<ISelectedAreaChangedListener>();

        public static void RegisterSelectedAreaChangedListener(ISelectedAreaChangedListener listener)
        {
            if (selectedAreaChangedListeners.Contains(listener))
                return;
            selectedAreaChangedListeners.Add(listener);
        }

        public static void UnregisterSelectedAreaChangedListener(ISelectedAreaChangedListener listener)
        {
            if (!selectedAreaChangedListeners.Contains(listener))
                return;
            selectedAreaChangedListeners.Remove(listener);
        }

        public static void NotifyAllSelectedAreaChangedListeners()
        {
            for (int i = 0; i < selectedAreaChangedListeners.Count; i++)
                selectedAreaChangedListeners[i].FireEventSelectedAreaChanged();
        }

        //
        public static List<IOCRSettingsChangedListener> ocrSettingsChangedListeners = new List<IOCRSettingsChangedListener>();

        public static void RegisterOCRSettingsChangedListener(IOCRSettingsChangedListener listener)
        {
            if (ocrSettingsChangedListeners.Contains(listener))
                return;
            ocrSettingsChangedListeners.Add(listener);
        }

        public static void UnregisterOCRSettingsChangedListener(IOCRSettingsChangedListener listener)
        {
            if (!ocrSettingsChangedListeners.Contains(listener))
                return;
            ocrSettingsChangedListeners.Remove(listener);
        }

        public static void notifyAllOCRSettingsChangedListeners()
        {
            for (int i = 0; i < ocrSettingsChangedListeners.Count; i++)
                ocrSettingsChangedListeners[i].FireEventOCRSettingsChanged();
        }

        //

        private static void notifyAllDPSModelSettingsChangedListeners()
        {
            //TODO
        }

        //
        public static List<IDPSWindowSettingsChangedListener> dpsWindowSettingsChangedListeners = new List<IDPSWindowSettingsChangedListener>();

        public static void RegisterDPSWindowSettingsChangedListener(IDPSWindowSettingsChangedListener listener)
        {
            if (dpsWindowSettingsChangedListeners.Contains(listener))
                return;
            dpsWindowSettingsChangedListeners.Add(listener);
        }

        public static void UnregisterDPSWindowSettingsChangedListener(IDPSWindowSettingsChangedListener listener)
        {
            if (!dpsWindowSettingsChangedListeners.Contains(listener))
                return;
            dpsWindowSettingsChangedListeners.Remove(listener);
        }

        public static void NotifyAllDPSWindowSettingsChangedListeners()
        {
            for (int i = 0; i < dpsWindowSettingsChangedListeners.Count; i++)
                dpsWindowSettingsChangedListeners[i].FireEventDPSWindowSettingsChanged();
        }
    }
}
