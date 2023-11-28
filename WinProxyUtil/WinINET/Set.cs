using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinProxyUtil.WinINET
{
    internal static class Set
    {
        internal static bool ClearProxy(string Connection, out int Win32Error)
        {
            return SetProxy(false, "", "", "", Connection, out Win32Error);
        }

        internal static bool SetProxy(bool? AutoDetect, string PacUrl, string ProxyServer, string ProxyBypass, string Connection, out int Win32Error)
        {
            var optionCount = 1;
            if (PacUrl != null) optionCount++;
            if (ProxyServer != null) optionCount++;
            if (ProxyBypass != null) optionCount++;

            bool iRes = false;

            var optIdx = 0;
            var list = new INTERNET_PER_CONN_OPTION_LIST();
            var option = new INTERNET_PER_CONN_OPTION[optionCount];
            var optSize = Marshal.SizeOf(option[0]);

            IntPtr pszPacUrl = Marshal.StringToHGlobalAuto(PacUrl);
            IntPtr pszProxyServer = Marshal.StringToHGlobalAuto(ProxyServer);
            IntPtr pszProxyBypass = Marshal.StringToHGlobalAuto(ProxyBypass);
            IntPtr pOptions = Marshal.AllocHGlobal(optSize * optionCount);
            IntPtr pCurrentOption = new IntPtr(pOptions.ToInt64());
            IntPtr pListBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(list));

            try
            {
                option[optIdx].dwOption = PerConnOption.INTERNET_PER_CONN_FLAGS;
                option[optIdx].Value.dwValue = PerConnFlag.PROXY_TYPE_DIRECT;
                if (AutoDetect.HasValue && AutoDetect.Value) option[optIdx].Value.dwValue |= PerConnFlag.PROXY_TYPE_AUTO_DETECT;
                optIdx++;

                if (PacUrl != null)
                {
                    option[0].Value.dwValue |= PerConnFlag.PROXY_TYPE_AUTO_PROXY_URL;
                    option[optIdx].dwOption = PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL;
                    option[optIdx].Value.pszValue = pszPacUrl;
                    optIdx++;
                }
                if (ProxyServer != null)
                {
                    if(!string.IsNullOrWhiteSpace(ProxyServer)) option[0].Value.dwValue |= PerConnFlag.PROXY_TYPE_PROXY;
                    option[optIdx].dwOption = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
                    option[optIdx].Value.pszValue = pszProxyServer;
                    optIdx++;
                }
                if (ProxyBypass != null)
                {
                    option[optIdx].dwOption = PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS;
                    option[optIdx].Value.pszValue = pszProxyBypass;
                    optIdx++;
                }

                for (var i = 0; i < optionCount; i++)
                {
                    Marshal.StructureToPtr(option[i], pCurrentOption, true);
                    pCurrentOption += optSize;
                }

                list.dwSize = Marshal.SizeOf(list);
                list.pszConnection = Connection;
                list.dwOptionCount = optionCount;
                list.dwOptionError = 0;
                list.pOptions = pOptions;

                iRes = PInvoke.InternetSetOption(IntPtr.Zero, OptionFlag.INTERNET_OPTION_PER_CONNECTION_OPTION, ref list, Marshal.SizeOf(list));
                Win32Error = Marshal.GetLastWin32Error();
            }
            finally
            {
                Marshal.FreeHGlobal(pszPacUrl);
                Marshal.FreeHGlobal(pszProxyServer);
                Marshal.FreeHGlobal(pszProxyBypass);
                Marshal.FreeHGlobal(pOptions);
                Marshal.FreeHGlobal(pListBuffer);
            }
            return iRes;
        }

        internal static bool SetProxyPerUser(int Value)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"Software\Policies\Microsoft\Windows\CurrentVersion\Internet Settings");
                key.SetValue("ProxySettingsPerUser", Value, RegistryValueKind.DWord);
            }
            catch { return false; }
            return true;
        }
    }
}
