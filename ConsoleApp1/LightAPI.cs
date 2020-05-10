using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Configuration;

namespace ConsoleApp1
{

    public enum MLAPIStatus : int
    {
        MLAPI_OK = 0,
        MLAPI_ERROR = -1,
        MLAPI_TIMEOUT = -2,
        MLAPI_NO_IMPLEMENTED = -3,
        MLAPI_NOT_INITIALIZED = -4,
        MLAPI_INVALID_ARGUMENT = -101,
        MLAPI_DEVICE_NOT_FOUND = -102,
        MLAPI_NOT_SUPPORTED = -103
    }

    static class LightAPI
    {

        public const string SDK_NAME = "MysticLight_SDK.dll";


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        public static string ERROR(MLAPIStatus status)
        {
            return status switch
            {
                MLAPIStatus.MLAPI_OK => "OK",
                MLAPIStatus.MLAPI_ERROR => "Error occurred in MLAPI function",
                MLAPIStatus.MLAPI_DEVICE_NOT_FOUND => "Device not found",
                MLAPIStatus.MLAPI_INVALID_ARGUMENT => "Invalid argument given",
                MLAPIStatus.MLAPI_NOT_INITIALIZED => "MLAPI is not initialized, call initialize function first",
                MLAPIStatus.MLAPI_NOT_SUPPORTED => "The specified device does not support this",
                MLAPIStatus.MLAPI_TIMEOUT => "Request timed out",
                MLAPIStatus.MLAPI_NO_IMPLEMENTED => "MLAPI is not supported on the current system",
                _ => "Unknown MLAPI status",
            };
        }

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_Initialize();

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetDeviceInfo(
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] devTypes,
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_UINT)] ref uint[] ledCount
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetDeviceName(
            string type,
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] devName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetDeviceNameEx(
            string type,
            uint index,
            ref string deviceName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedInfo(
            string type,
            uint index,
            ref string name,
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] ledStyles
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedName(
            string type,
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] deviceName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedColor(
            string type,
            uint index,
            ref uint R,
            ref uint G,
            ref uint B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedStyle(
            string type,
            uint index,
            ref string style
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedMaxBright(
            string type,
            uint index,
            ref uint maxLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedBright(
            string type,
            uint index,
            ref uint currentLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedMaxSpeed(
            string type,
            uint index,
            ref uint maxLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedSpeed(
            string type,
            uint index,
            ref uint currentLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedColor(
            string type,
            uint index,
            uint R,
            uint G,
            uint B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedColors(
            string type,
            uint index,
            [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] ledName,
            ref uint R,
            ref uint G,
            ref uint B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedColorEx(
            string type,
            uint index,
            string ledName,
            uint R,
            uint G,
            uint B,
            uint Sync
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedColorSync(
            string type,
            uint index,
            string ledName,
            uint R,
            uint G,
            uint B,
            uint Sync
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedStyle(
            string type,
            uint index,
            string style
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedBright(
            string type,
            uint index,
            uint level
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_SetLedSpeed(
            string type,
            uint index,
            uint level
        );


        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetErrorMessage(
            int errorCode,
            [MarshalAs(UnmanagedType.BStr)] ref string description
        );

    }
}
