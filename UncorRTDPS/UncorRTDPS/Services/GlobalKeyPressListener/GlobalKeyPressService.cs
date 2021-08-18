using System;
using System.Collections.Generic;

namespace UncorRTDPS.Services.GlobalKeyPressListener
{
    class GlobalKeyPressService : IDisposable, Service
    {
        private GlobalKeyboardHook _globalKeyboardHook = null;
        private Dictionary<string, RegisterableGlobalKeyboardListener> registeredGlobalKeyboardListeners = new Dictionary<string, RegisterableGlobalKeyboardListener>();

        public GlobalKeyboardHook GlobalKeyboardHook 
        { 
            get { return this._globalKeyboardHook; } 
        }

        public ServiceResponseStatus InitService(string param)
        {
            //init hook
            try
            {
                _globalKeyboardHook = new GlobalKeyboardHook();
            }
            catch
            {
                return ServiceResponseStatus.FAILED;
            }
            return ServiceResponseStatus.OK;
        }

        public ServiceResponseStatus CloseService()
        {
            Dispose();
            _globalKeyboardHook = null;
            return ServiceResponseStatus.OK;
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }


        //Strong registration. If registered then updates (unregister + register)
        public bool RegisterEventHandler_OnKeyPressed(string uniqueEventHandlerId, RegisterableGlobalKeyboardListener registerableGlobalKeyboardListener)
        {
            if (_globalKeyboardHook == null)
                return false;

            if (registeredGlobalKeyboardListeners.ContainsKey(uniqueEventHandlerId))
            {
                UnregisterEventHandler_OnKeyPressed(uniqueEventHandlerId);
            }

            registeredGlobalKeyboardListeners.Add(uniqueEventHandlerId, registerableGlobalKeyboardListener);
            _globalKeyboardHook.KeyboardPressed += registerableGlobalKeyboardListener.OnKeyPressed_GlobalKeyboardEvent;
            return true;
        }

        public bool UnregisterEventHandler_OnKeyPressed(string uniqueEventHandlerId)
        {
            if (_globalKeyboardHook == null)
                return false;

            if (!registeredGlobalKeyboardListeners.ContainsKey(uniqueEventHandlerId))
                return true;

            _globalKeyboardHook.KeyboardPressed -= registeredGlobalKeyboardListeners[uniqueEventHandlerId].OnKeyPressed_GlobalKeyboardEvent;
            registeredGlobalKeyboardListeners.Remove(uniqueEventHandlerId);

            return true;
        }

        public Dictionary<string, RegisterableGlobalKeyboardListener> GetRegisteredGlobalKeyboardListeners()
        {
            return this.registeredGlobalKeyboardListeners;
        }
    }
}
