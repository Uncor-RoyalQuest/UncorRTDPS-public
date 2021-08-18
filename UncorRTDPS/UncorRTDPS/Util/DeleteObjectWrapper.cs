﻿using System;
using System.Runtime.InteropServices;

namespace UncorRTDPS.Util
{
    public class DeleteObjectWrapper
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
    }
}