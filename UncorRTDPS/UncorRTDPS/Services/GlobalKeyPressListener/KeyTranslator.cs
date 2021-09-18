using System.Windows.Input;

namespace UncorRTDPS.Services.GlobalKeyPressListener
{
    public class KeyTranslator
    {
        private const int LeftShiftVK = 160; //KeyInterop.VirtualKeyFromKey(Key.LeftShift);
        private const int RightShiftVK = 161; // KeyInterop.VirtualKeyFromKey(Key.RightShift);

        private const int LeftControlVK = 162; // KeyInterop.VirtualKeyFromKey(Key.LeftShift);
        private const int RightControlVK = 163; // KeyInterop.VirtualKeyFromKey(Key.RightCtrl);

        private const int LeftAltVK = 164; // KeyInterop.VirtualKeyFromKey(Key.LeftAlt);
        private const int RightAltVK = 165; // KeyInterop.VirtualKeyFromKey(Key.RightAlt);

        private const int LeftWindowsVK = 91;
        private const int RightWindowsVK = 92;


        public static ModifierKeys VkToModifier(int vk)
        {
            if (vk < 91)
                return ModifierKeys.None;

            switch (vk)
            {
                case LeftShiftVK:
                    return ModifierKeys.Shift;

                case RightShiftVK:
                    return ModifierKeys.Shift;

                case LeftControlVK:
                    return ModifierKeys.Control;

                case RightControlVK:
                    return ModifierKeys.Control;

                case LeftAltVK:
                    return ModifierKeys.Alt;

                case RightAltVK:
                    return ModifierKeys.Alt;

                case LeftWindowsVK:
                    return ModifierKeys.Windows;

                case RightWindowsVK:
                    return ModifierKeys.Windows;

            }

            return ModifierKeys.None;
        }

        public static ModifierKeys KeyToModifier(Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                    return ModifierKeys.Shift;

                case Key.RightShift:
                    return ModifierKeys.Shift;

                case Key.LeftCtrl:
                    return ModifierKeys.Control;

                case Key.RightCtrl:
                    return ModifierKeys.Control;

                case Key.LeftAlt:
                    return ModifierKeys.Alt;

                case Key.RightAlt:
                    return ModifierKeys.Alt;

                case Key.LWin:
                    return ModifierKeys.Windows;

                case Key.RWin:
                    return ModifierKeys.Windows;

            }
            return ModifierKeys.None;
        }

    }
}
