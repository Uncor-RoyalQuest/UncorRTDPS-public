using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace UncorRTDPS.Services.HotKeys
{
    class HotKeyCombination
    {
        public List<Key> Keys { get; set; } = new List<Key>();
        public List<ModifierKeys> ModifierKeys { get; set; } = new List<ModifierKeys>();

        public HotKeyCombination()
        {

        }

        public HotKeyCombination(List<ModifierKeys> modifierKeys, List<Key> keys)
        {
            this.ModifierKeys = new List<ModifierKeys>(modifierKeys);
            this.Keys = new List<Key>(keys);
        }

        public int GetCombinationLength()
        {
            return Keys.Count + ModifierKeys.Count;
        }

        public override string ToString()
        {
            List<string> res = new List<string>();
            for (int i = 0; i < ModifierKeys.Count; i++)
            {
                res.Add(ModifierKeys[i].ToString());
            }

            for (int i = 0; i < Keys.Count; i++)
            {
                res.Add(Keys[i].ToString());
            }

            return String.Join(" + ", res);
        }
    }
}
