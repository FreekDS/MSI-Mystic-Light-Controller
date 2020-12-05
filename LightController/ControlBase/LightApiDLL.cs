using System;
using System.Runtime.InteropServices;

namespace MysticLightController
{

    internal enum MLAPIStatus : int
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

    internal static class LightApiDLL
    {

        private const string SDK_NAME = "MysticLight_SDK.dll";


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

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetDeviceInfo(
            [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] devTypes,
            [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] ledCount
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetDeviceName(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] devName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetDeviceNameEx(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.BStr)] out string deviceName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedInfo(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.BStr)] out string name,
            [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] ledStyles
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_GetLedName(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [Out, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] deviceName
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedColor(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.U4)] out uint R,
            [Out, MarshalAs(UnmanagedType.U4)] out uint G,
            [Out, MarshalAs(UnmanagedType.U4)] out uint B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedStyle(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.BStr)] out string style
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedMaxBright(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.U4)] out uint maxLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedBright(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.U4)] out uint currentLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedMaxSpeed(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.U4)] out uint maxLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetLedSpeed(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [Out, MarshalAs(UnmanagedType.U4)] out uint currentLevel
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedColor(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.U4)] uint R,
            [In, MarshalAs(UnmanagedType.U4)] uint G,
            [In, MarshalAs(UnmanagedType.U4)] uint B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedColors(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] ref String[] ledName,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] uint[] R,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] uint[] G,
            [In, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U4)] uint[] B
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedColorEx(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.BStr)] string ledName,
            [In, MarshalAs(UnmanagedType.U4)] uint R,
            [In, MarshalAs(UnmanagedType.U4)] uint G,
            [In, MarshalAs(UnmanagedType.U4)] uint B,
            [In, MarshalAs(UnmanagedType.U4)] uint Sync
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedColorSync(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.BStr)] string ledName,
            [In, MarshalAs(UnmanagedType.U4)] uint R,
            [In, MarshalAs(UnmanagedType.U4)] uint G,
            [In, MarshalAs(UnmanagedType.U4)] uint B,
            [In, MarshalAs(UnmanagedType.U4)] uint Sync
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedStyle(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.BStr)] string style
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedBright(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.U4)] uint level
        );

        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_SetLedSpeed(
            [In, MarshalAs(UnmanagedType.BStr)] string type,
            [In, MarshalAs(UnmanagedType.U4)] uint index,
            [In, MarshalAs(UnmanagedType.U4)] uint level
        );


        [DllImport(SDK_NAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int MLAPI_GetErrorMessage(
            int errorCode,
            [Out, MarshalAs(UnmanagedType.BStr)] out string description
        );

    }
}
