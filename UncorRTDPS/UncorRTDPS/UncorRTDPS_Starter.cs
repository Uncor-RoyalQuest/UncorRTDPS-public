using System.IO;

namespace UncorRTDPS
{
    public static class UncorRTDPS_Starter
    {

        public static bool Check_AllModulesInstalled(string executingFolder)
        {
            bool isCPPRedist15n19Installed = Util.CPPRedistChecker.IsInstalled(Util.RedistributablePackageVersion.VC2015to2019x64) || Util.CPPRedistChecker.IsInstalled(Util.RedistributablePackageVersion.VC2015to2019x86);
            if (!isCPPRedist15n19Installed && File.Exists(Path.GetFullPath(Path.Combine(executingFolder, "cpp_r_ignore.txt"))))
            {
                UtilWindows.MissingVisualCPPRedist missingVisualCPPRedist = new UtilWindows.MissingVisualCPPRedist();
                missingVisualCPPRedist.ShowDialog();
                return false;
            }
            return true;
        }

        
        public static void InitUncorRTDPS(string resourcesPath, string uiLanguage)
        {

            //set uiLang
            if (uiLanguage.Equals("eng") || uiLanguage.Equals("en"))
            {
                RTDPS_Settings.UncorRTDPS_StaticSettings.Language_UI = RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English;
            }
            else if (uiLanguage.Equals("rus") || uiLanguage.Equals("ru"))
            {
                RTDPS_Settings.UncorRTDPS_StaticSettings.Language_UI = RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian;
            }
            else
            {
                RTDPS_Settings.UncorRTDPS_StaticSettings.Language_UI = RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.English;
            }
                
            //resourcesPath
            RTDPS_Settings.UncorRTDPS_StaticSettings.ResourcesPath = resourcesPath;

            //config file path
            RTDPS_Settings.UncorRTDPS_StaticSettings.ConfigFileRTDPS = Path.GetFullPath(Path.Combine(resourcesPath, "config_rtdps.txt"));

            //load configs
            RTDPS_Settings.UncorRTDPS_Config.LoadConfigs();

            //load locGUI_rtdps
            RTDPS_Settings.UncorRTDPS_Localization.LoadLocalizationGUI();

            //init other params of the static settings
            RTDPS_Settings.UncorRTDPS_StaticSettings.InitFromConfig();
            RTDPS_Settings.UncorRTDPS_StaticSettings.InitImages();

            //Add Window Position Service to Service Container
            Services.WindowPositionService windowPositionService = new Services.WindowPositionService();
            windowPositionService.InitService(Path.GetFullPath(Path.Combine(resourcesPath, "src", "windowspositions.json")));
            Services.ServicesContainer.AddNewService("windowPositionService", windowPositionService);

            //Add HotKeysService to Service Container
            Services.HotKeys.HotKeysStorageService hotKeysStorageService = new Services.HotKeys.HotKeysStorageService();
            hotKeysStorageService.InitService(Path.GetFullPath(Path.Combine(resourcesPath, "src", "hotkeys.json")));
            Services.ServicesContainer.AddNewService("hotKeysStorageService", hotKeysStorageService);

            //Add GlobalKeyPressListener Service to Service Container
            Services.GlobalKeyPressListener.GlobalKeyPressService globalKeyPressService = new Services.GlobalKeyPressListener.GlobalKeyPressService();
            globalKeyPressService.InitService(null);
            Services.ServicesContainer.AddNewService("globalKeyPressService", globalKeyPressService);

            //Add SimpleAliasService Service to Service Contrainer
            Services.Aliasing.SimpleAliasService simpleAliasService = new Services.Aliasing.SimpleAliasService();
            simpleAliasService.InitService(Path.GetFullPath(Path.Combine(resourcesPath, "src", "mobs_rtdps", "aliases.txt")));
            Services.ServicesContainer.AddNewService("simpleAliasService_Mobs", simpleAliasService);

            //globalKeyPressService.GlobalKeyboardHook.KeyboardPressed += (e, k) => { Trace.WriteLine(JsonSerializer.Serialize(globalKeyPressService.getRegisteredGlobalKeyboardListeners())); };
        }
        
    }
}
