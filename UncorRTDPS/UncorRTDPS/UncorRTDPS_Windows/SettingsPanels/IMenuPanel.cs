namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    public interface IMenuPanel
    {
        void ActivateMenuPanel();
        void DeactivateMenuPanel();
        string GetMenuName();
    }
}
