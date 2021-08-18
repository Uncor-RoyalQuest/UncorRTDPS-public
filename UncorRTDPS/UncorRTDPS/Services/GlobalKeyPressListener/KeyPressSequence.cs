using System;
using System.Collections.Generic;
using System.Windows.Input;
using UncorRTDPS.Services.HotKeys;

namespace UncorRTDPS.Services.GlobalKeyPressListener
{
    class KeyPressSequence : RegisterableGlobalKeyboardListener, IDisposable
    {

        internal class DualityKey
        {
            public bool isPressed;
            public Key key;
            public ModifierKeys modifierKeys;

            public DualityKey(Key key)
            {
                this.key = key;
                modifierKeys = ModifierKeys.None;
            }

            public DualityKey(ModifierKeys modifierKeys)
            {
                this.modifierKeys = modifierKeys;
                key = Key.None;
            }
        }

        public Action DelegatedAction { get; set; } = null;

        private int sequenceLength;
        public int SequenceLength
        {
            get { return sequenceLength; }
            set { sequenceLength = value; }
        }
        
        private HotKeyCombination hotKeyCombination;
        private List<DualityKey> dualityKeysSequence;
        private int posKeyStart;

        private long restrictActionCallIfLessThanEveryMs;
        public long RestrictActionCallIfLessThanEveryMs
        {
            get { return restrictActionCallIfLessThanEveryMs; }
            set
            {
                restrictActionCallIfLessThanEveryMs = value;
                restrictActionCallIfLessThanEveryTicks = TimeSpan.TicksPerMillisecond * value;
            }
        }

        private long restrictActionCallIfLessThanEveryTicks;
        private long lastActionTimeInTicks = 0;

        public KeyPressSequence(HotKeyCombination hotKeyCombination)
        {
            RestrictActionCallIfLessThanEveryMs = 200;

            this.hotKeyCombination = hotKeyCombination;
            dualityKeysSequence = new List<DualityKey>();
            for (int i = 0; i < hotKeyCombination.ModifierKeys.Count; i++)
                dualityKeysSequence.Add(new DualityKey(hotKeyCombination.ModifierKeys[i]));
            posKeyStart = hotKeyCombination.ModifierKeys.Count;
            for (int i = 0; i < hotKeyCombination.Keys.Count; i++)
                dualityKeysSequence.Add(new DualityKey(hotKeyCombination.Keys[i]));
            sequenceLength = dualityKeysSequence.Count;
        }


        public void OnKeyPressed_GlobalKeyboardEvent(object sender, GlobalKeyboardHookEventArgs e)
        {
            long utcTicksNow;
            bool isKeyFromTheSequence = false;
           
            ModifierKeys modifierKeys = KeyTranslator.VkToModifier(e.KeyboardData.VirtualCode);

            //it is a key
            if (modifierKeys == ModifierKeys.None)
            {
                Key key = KeyInterop.KeyFromVirtualKey(e.KeyboardData.VirtualCode);
                for (int i = posKeyStart; i < sequenceLength; i++)
                {
                    if (dualityKeysSequence[i].key == key)
                    {
                        if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown ||
                            e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
                        {
                            dualityKeysSequence[i].isPressed = true;
                        }
                        else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp ||
                            e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyUp)
                        {
                            dualityKeysSequence[i].isPressed = false;
                        }

                        isKeyFromTheSequence = true;
                        break;
                    }
                }
            }
            else //else = it is a modifier
            {
                for (int i = 0; i < posKeyStart; i++)
                {
                    if (dualityKeysSequence[i].modifierKeys == modifierKeys)
                    {
                        if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown ||
                            e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
                        {
                            dualityKeysSequence[i].isPressed = true;
                        }
                        else if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp ||
                            e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyUp)
                        {
                            dualityKeysSequence[i].isPressed = false;
                        }

                        isKeyFromTheSequence = true;
                        break;
                    }
                }
            }

            if (isKeyFromTheSequence)
            {
                bool isAllPressed = true;
                for (int i = 0; i < sequenceLength; i++)
                {
                    if (!dualityKeysSequence[i].isPressed)
                    {
                        isAllPressed = false;
                        break;
                    }
                }

                if (isAllPressed)
                {
                    utcTicksNow = DateTime.UtcNow.Ticks;
                    if (utcTicksNow - lastActionTimeInTicks > restrictActionCallIfLessThanEveryTicks)
                    {
                        lastActionTimeInTicks = utcTicksNow;
                        DelegatedAction();
                    }
                }
            }
        }

        public void Dispose()
        {
            DelegatedAction = null;
        }
    }
}
