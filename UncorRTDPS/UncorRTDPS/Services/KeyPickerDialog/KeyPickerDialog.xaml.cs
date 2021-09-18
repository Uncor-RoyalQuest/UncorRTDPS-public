using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UncorRTDPS.Services.GlobalKeyPressListener;

namespace UncorRTDPS.Services.KeyPickerDialog
{
    /// <summary>
    /// Interaction logic for KeyPickerDialog.xaml
    /// </summary>
    public partial class KeyPickerDialog : Window, RegisterableGlobalKeyboardListener, IDisposable
    {

        private string name = "KeyPickerDialog_000";

        public bool IsToSave { get; set; } = false;

        private bool[] isTextChanging = { false, false, false, false };
        private Key[] key = { Key.None, Key.None, Key.None, Key.None };
        private ModifierKeys[] modifierKeys = { ModifierKeys.None, ModifierKeys.None, ModifierKeys.None, ModifierKeys.None };
        private bool[] isKey = { false, false, false, false }; //false = modifier;
        private List<TextBox> textBoxes = new List<TextBox>();
        private GlobalKeyPressService globalKeyPressService;

        private bool[] keyboardFocuses = { false, false, false, false };

        private WindowPositionService windowPositionService = null;
        public bool IsSavedWindowPositionSet { get; set; } = false;

        public KeyPickerDialog()
        {
            InitializeComponent();
            InitLocalization();
            textBoxes.Add(TextBox_Key_1);
            textBoxes.Add(TextBox_Key_2);
            textBoxes.Add(TextBox_Key_3);
            textBoxes.Add(TextBox_Key_4);

            //saved window position
            Service service = ServicesContainer.GetService("windowPositionService");
            if (service != null && service is WindowPositionService)
            {
                windowPositionService = service as WindowPositionService;
                Point<double> p = windowPositionService.GetWindowPosition(name);
                if (p != null)
                {
                    IsSavedWindowPositionSet = true;
                    this.Top = p.Y;
                    this.Left = p.X;
                }
            }

            globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            globalKeyPressService?.RegisterEventHandler_OnKeyPressed("keyPickerDialog", this);
        }

        public void InitLocalization()
        {
            Title = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiTitleKeyPickerDialog");

            TextBlock_Key_1.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiKeyboardKey1");
            TextBlock_Key_2.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiKeyboardKey2");
            TextBlock_Key_3.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiKeyboardKey3");
            TextBlock_Key_4.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiKeyboardKey4");

            Button_DelKey_1.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonDel");
            Button_DelKey_2.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonDel");
            Button_DelKey_3.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonDel");
            Button_DelKey_4.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonDel");

            Button_Save.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonSave");
            Button_Cancel.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonCancel");
        }

        ~KeyPickerDialog()
        {
            globalKeyPressService?.UnregisterEventHandler_OnKeyPressed("keyPickerDialog");
        }

        public void OnKeyPressed_GlobalKeyboardEvent(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown ||
                e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown)
            {
                int vk = e.KeyboardData.VirtualCode;
                UpdateActiveSelector(vk);
            }
        }


        public void UpdateActiveSelector(int vk)
        {
            for (int i = 0; i < keyboardFocuses.Length; i++)
            {
                if (keyboardFocuses[i])
                {
                    UpdateCell_I(vk, i);
                    break;
                }
            }
        }

        private void UpdateCell_I(int vk, int cell)
        {
            ModifierKeys modifier = KeyTranslator.VkToModifier(vk);

            if (modifier != ModifierKeys.None)
            {
                isKey[cell] = false;
                modifierKeys[cell] = modifier;
                textBoxes[cell].Text = modifier.ToString();
            }
            else
            {
                isKey[cell] = true;
                key[cell] = KeyInterop.KeyFromVirtualKey(vk);
                textBoxes[cell].Text = key[cell].ToString();
            }

        }


        public void SetKeysByCopy(List<ModifierKeys> modifierKeys, List<Key> keys)
        {
            int pos = 0;
            for (int i = 0; i < modifierKeys.Count; i++)
            {
                this.modifierKeys[pos] = modifierKeys[i];

                isTextChanging[pos] = true;
                textBoxes[pos].Text = this.modifierKeys[pos].ToString();
                isTextChanging[pos] = false;

                pos += 1;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                this.key[pos] = keys[i];
                isKey[pos] = true;

                isTextChanging[pos] = true;
                textBoxes[pos].Text = this.key[pos].ToString();
                isTextChanging[pos] = false;

                pos += 1;
            }
        }

        public List<ModifierKeys> GetResultModifiers()
        {
            //List<ModifierKeys> resModifierKeys = new List<ModifierKeys>();

            HashSet<ModifierKeys> resModifierKeys = new HashSet<ModifierKeys>();
            for (int i = 0; i < 4; i++)
            {
                if (!isKey[i] && modifierKeys[i] != ModifierKeys.None)
                    resModifierKeys.Add(modifierKeys[i]);
            }
            return resModifierKeys.ToList();
        }

        public List<Key> GetResultKeys()
        {
            //List<Key> resKeys = new List<Key>();

            HashSet<Key> resKeys = new HashSet<Key>();
            for (int i = 0; i < 4; i++)
            {
                if (isKey[i] && key[i] != Key.None)
                    resKeys.Add(key[i]);
            }
            return resKeys.ToList();
        }


        private void TextBox_Key_I_TextChanged(object sender, TextChangedEventArgs e, int i)
        {
            if (!isTextChanging[i])
            {
                isTextChanging[i] = true;

                if (isKey[i])
                    textBoxes[i].Text = key[i].ToString();
                else
                    textBoxes[i].Text = modifierKeys[i].ToString();

                isTextChanging[i] = false;
            }
        }

        private void Button_DelKey_I_Click(object sender, RoutedEventArgs e, int i)
        {
            key[i] = Key.None;
            modifierKeys[i] = ModifierKeys.None;

            isTextChanging[i] = true;
            textBoxes[i].Text = "";
            isTextChanging[i] = false;
        }

        private void TextBox_Key_1_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_Key_I_TextChanged(sender, e, 0);
        }

        private void TextBox_Key_2_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_Key_I_TextChanged(sender, e, 1);
        }

        private void TextBox_Key_3_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_Key_I_TextChanged(sender, e, 2);
        }

        private void TextBox_Key_4_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_Key_I_TextChanged(sender, e, 3);
        }

        private void Button_DelKey_1_Click(object sender, RoutedEventArgs e)
        {
            Button_DelKey_I_Click(sender, e, 0);
        }

        private void Button_DelKey_2_Click(object sender, RoutedEventArgs e)
        {
            Button_DelKey_I_Click(sender, e, 1);
        }

        private void Button_DelKey_3_Click(object sender, RoutedEventArgs e)
        {
            Button_DelKey_I_Click(sender, e, 2);
        }

        private void Button_DelKey_4_Click(object sender, RoutedEventArgs e)
        {
            Button_DelKey_I_Click(sender, e, 3);
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            IsToSave = true;
            this.Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsToSave = false;
            this.Close();
        }

        private void TextBox_Key_1_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[0] = true;
        }

        private void TextBox_Key_1_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[0] = false;
        }

        private void TextBox_Key_2_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[1] = true;
        }

        private void TextBox_Key_2_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[1] = false;

        }

        private void TextBox_Key_3_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[2] = true;
        }

        private void TextBox_Key_3_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[2] = false;

        }

        private void TextBox_Key_4_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[3] = true;
        }

        private void TextBox_Key_4_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            keyboardFocuses[3] = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowPositionService?.UpdateWindowPosition(name, new Point<double>(this.Left, this.Top));
        }


        public void Dispose()
        {
            textBoxes.Clear();
            globalKeyPressService?.UnregisterEventHandler_OnKeyPressed("keyPickerDialog");
            globalKeyPressService = null;
        }
    }
}
