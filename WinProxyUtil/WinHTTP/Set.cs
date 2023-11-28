using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinProxyUtil.WinHTTP
{
    internal static class Set
    {
        internal static void SetDefaultProxy(string ProxyServer, string BypassList)
        {
            var proxy = new WINHTTP_PROXY_INFO();
            if (ProxyServer == null)
            {
                proxy.dwAccessType = AccessType.WINHTTP_ACCESS_TYPE_NAMED_PROXY;
            }
            else
            {
                proxy.dwAccessType = AccessType.WINHTTP_ACCESS_TYPE_NAMED_PROXY;
                proxy.lpszProxy = ProxyServer;
                proxy.lpszProxyBypass = BypassList;
            }
            if(PInvoke.WinHttpSetDefaultProxyConfiguration(ref proxy))
            {
                Console.WriteLine("Successfully set WinHTTP default proxy");
            }
            else
            {
                var err = Marshal.GetLastWin32Error();
                Console.WriteLine($"Failed to set WinHTTP default proxy. Error {PInvoke.ErrorMessage[err]}");
            }
        }

        internal static void ResetDefaultProxy()
        {
            SetDefaultProxy(null, null);
        }

        internal static void SetDisableWpad(int value)
        {
            var key = Registry.LocalMachine.OpenSubKey(Query.DisableWpadKey[1]);
            key.SetValue("DisableWpad", value, RegistryValueKind.DWord);
        }
    }
}
