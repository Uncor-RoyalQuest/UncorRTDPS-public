namespace UncorRTDPS.UncorRTDPS_Windows
{
    public interface IParentTopmostChanger
    {
        void RegisterIParentTopmostListener(IParentTopmostListener listener);
        void UnregisterIParentTopmostListener(IParentTopmostListener listener);
        void NotifyAllListenersIParentTopmostListener(bool topmost);
    }
}
