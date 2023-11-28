using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinProxyUtil.WinINET
{
    internal static class PInvoke
    {
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool InternetQueryOption(IntPtr hInternet, OptionFlag dwOption, ref INTERNET_PER_CONN_OPTION_LIST lpBuffer, ref int lpdwBufferLength);

        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool InternetSetOption(IntPtr hInternet, OptionFlag dwOption, ref INTERNET_PER_CONN_OPTION_LIST lpBuffer, int dwBufferLength);

        [DllImport("rasapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint RasEnumEntries(
            IntPtr reserved,
            IntPtr lpszPhonebook,
            [In, Out] RASENTRYNAME[] lprasentryname,
            ref int lpcb,
            ref int lpcEntries);

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct INTERNET_PER_CONN_OPTION_LIST
    {
        public int dwSize;
        public string pszConnection;
        public int dwOptionCount;
        public int dwOptionError;
        public IntPtr pOptions;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct INTERNET_PER_CONN_OPTION
    {
        public PerConnOption dwOption;
        public INTERNET_PER_CONN_OPTION_VALUE Value;


        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Auto)]
        public struct INTERNET_PER_CONN_OPTION_VALUE
        {
            [FieldOffset(0)]
            public PerConnFlag dwValue;
            [FieldOffset(0)]
            public IntPtr pszValue;
            [FieldOffset(0)]
            public System.Runtime.InteropServices.ComTypes.FILETIME ftValue;
        }
    }

    enum OptionFlag : uint
    {
        INTERNET_OPTION_PER_CONNECTION_OPTION = 75,
        INTERNET_OPTION_PROXY = 38
    }

    enum PerConnOption : uint
    {
        INTERNET_PER_CONN_FLAGS = 1,
        INTERNET_PER_CONN_PROXY_SERVER = 2,
        INTERNET_PER_CONN_PROXY_BYPASS = 3,
        INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
        INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
        INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
        INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
        INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
        INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
        INTERNET_PER_CONN_FLAGS_UI = 10,
    }

    [Flags]
    enum PerConnFlag : uint
    {
        PROXY_TYPE_DIRECT = 0x00000001,
        PROXY_TYPE_PROXY = 0x00000002,
        PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,
        PROXY_TYPE_AUTO_DETECT = 0x00000008,
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct RASENTRYNAME
    {
        const int MAX_PATH = 260;
        const int RAS_MaxEntryName = 256;

        public int dwSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
        public string szEntryName;
        public int dwFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH + 1)]
        public string szPhonebook;
    }
}
