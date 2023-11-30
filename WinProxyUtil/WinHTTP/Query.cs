using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WinProxyUtil.Misc;

namespace WinProxyUtil.WinHTTP
{
    internal static class Query
    {
        internal static readonly string[] DisableWpadKey =
        {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\WinHttp",
            @"SOFTWARE\Policies\Microsoft\Windows\CurrentVersion\Internet Settings\WinHttp",
        };

        internal static void QueryDisableWpad()
        {
            var key = Registry.LocalMachine;
            foreach (var subkey in DisableWpadKey)
            {
                var value = key.OpenSubKey(subkey)?.GetValue("DisableWpad");
                if (null != value && (int)value == 1)
                {
                    ConsoleControl.WriteWarningLine($"WPAD disabled by {key}\\{subkey}\\DisableWpad");
                }
                else
                {
                    Console.WriteLine($"WPAD not disabled by {key}\\{subkey}\\DisableWpad");
                }
            }
        }

        internal static void QueryDefaultProxy()
        {
            var defproxy = new WINHTTP_PROXY_INFO();
            PInvoke.WinHttpGetDefaultProxyConfiguration(ref defproxy);
            Console.WriteLine($"AccessType  : {defproxy.dwAccessType}");
            Console.WriteLine($"ProxyServer : {defproxy.lpszProxy}");
            Console.WriteLine($"BypassList  : {defproxy.lpszProxyBypass}");
        }

        internal static string GetWinINETPacUrl()
        {
            var ieproxy = new WINHTTP_CURRENT_USER_IE_PROXY_CONFIG();
            PInvoke.WinHttpGetIEProxyConfigForCurrentUser(ref ieproxy);
            return ieproxy.lpszAutoConfigUrl;
        }

        internal static void QueryWinINETProxy()
        {
            var ieproxy = new WINHTTP_CURRENT_USER_IE_PROXY_CONFIG();
            PInvoke.WinHttpGetIEProxyConfigForCurrentUser(ref ieproxy);
            Console.WriteLine($"AutoDetect    : {ieproxy.fAutoDetect}");
            Console.WriteLine($"AutoConfigURL : {ieproxy.lpszAutoConfigUrl}");
            Console.WriteLine($"ProxyServer   : {ieproxy.lpszProxy}");
            Console.WriteLine($"BypassList    : {ieproxy.lpszProxyBypass}");
        }

        internal static string[] GetAutoProxyUrl()
        {
            var result = new List<string>();
            var url = new string('\0', 1024);
            if (PInvoke.WinHttpDetectAutoProxyConfigUrl(AutoDetectFlag.WINHTTP_AUTO_DETECT_TYPE_DHCP, ref url))
            {
                result.Add(url);
            }
            if (PInvoke.WinHttpDetectAutoProxyConfigUrl(AutoDetectFlag.WINHTTP_AUTO_DETECT_TYPE_DNS_A, ref url))
            {
                result.Add(url);
            }
            return result.ToArray();
        }

        internal static void QueryAutoProxy()
        {
            var url = new string('\0', 1024);
            if (PInvoke.WinHttpDetectAutoProxyConfigUrl(AutoDetectFlag.WINHTTP_AUTO_DETECT_TYPE_DHCP, ref url))
            {
                Console.WriteLine($"DHCP WPAD : {url}");
            }
            else
            {
                Console.WriteLine($"DHCP WPAD : Not Detected");
            }
            if (PInvoke.WinHttpDetectAutoProxyConfigUrl(AutoDetectFlag.WINHTTP_AUTO_DETECT_TYPE_DNS_A, ref url))
            {
                Console.WriteLine($"DNS WPAD  : {url}");
            }
            else
            {
                Console.WriteLine($"DNS WPAD  : Not Detected");
            }
        }

        internal static void GetProxyForUrl(string Url, string PacUrl)
        {
            var handle = PInvoke.WinHttpOpen("WinProxyUtil/1.0", AccessType.WINHTTP_ACCESS_TYPE_NO_PROXY, null, null, 0);
            var option = new WINHTTP_AUTOPROXY_OPTIONS
            {
                dwFlags = AutoProxyFlag.WINHTTP_AUTOPROXY_CONFIG_URL | AutoProxyFlag.WINHTTP_AUTOPROXY_HOST_KEEPCASE | AutoProxyFlag.WINHTTP_AUTOPROXY_NO_CACHE_SVC,
                dwAutoDetectFlags = 0,
                lpszAutoConfigUrl = PacUrl,
                fAutoLogonIfChallenged = true
            };
            var proxy = new WINHTTP_PROXY_INFO();
            if (PInvoke.WinHttpGetProxyForUrl(handle, Url, ref option, ref proxy))
            {
                Console.WriteLine($"AccessType  : {proxy.dwAccessType}");
                Console.WriteLine($"ProxyServer : {proxy.lpszProxy}");
                Console.WriteLine($"BypassList  : {proxy.lpszProxyBypass}");
            }
            else
            {
                var err = Marshal.GetLastWin32Error();
                Console.WriteLine($"WinHttpGetProxyForUrl failed. Error {err}");
                Global.StatusCode = err;
            }
            PInvoke.WinHttpCloseHandle(handle);
        }
    }
}
