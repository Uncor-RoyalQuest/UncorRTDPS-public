
namespace UncorRTDPS.Services.GlobalKeyPressListener
{
    interface RegisterableGlobalKeyboardListener
    {
        void OnKeyPressed_GlobalKeyboardEvent(object sender, GlobalKeyboardHookEventArgs e);
    }
}
