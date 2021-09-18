using System;
using UncorRTDPS.Services.GlobalKeyPressListener;

namespace UncorRTDPS.Services.HotKeys
{
    static class HotkeyActionRegistrator
    {
        public static void RegisterActionToHotKey(GlobalKeyPressService globalKeyPressService, HotKeysStorageService hotKeysStorageService, string uid, string combinationName, Action action)
        {
            //create hotkey
            HotKeyCombination hotKeyCombination_ToggleMonitoring = hotKeysStorageService.GetCombinationForName(combinationName);
            if (hotKeyCombination_ToggleMonitoring != null)
            {

                KeyPressSequence keyPressSequence = new KeyPressSequence(hotKeyCombination_ToggleMonitoring);
                keyPressSequence.DelegatedAction = action;

                //register hotkey 
                if (keyPressSequence.SequenceLength > 0)
                {
                    globalKeyPressService.RegisterEventHandler_OnKeyPressed(uid + combinationName, keyPressSequence);
                }
            }
        }

        public static void UnregisterActionFromHotKey(GlobalKeyPressService globalKeyPressService, string uid, string combinationName)
        {
            globalKeyPressService.UnregisterEventHandler_OnKeyPressed(uid + combinationName);
        }
    }
}
