using Microsoft.Win32;
using System.Runtime.InteropServices;
using WinProxyUtil.Misc;

namespace WinProxyUtil.WinHTTP
{
    internal static class Set
    {
        internal static void SetDefaultProxy(string ProxyServer, string BypassList)
        {
            var proxy = new WINHTTP_PROXY_INFO();
            if (ProxyServer == null)
            {
                proxy.dwAccessType = AccessType.WINHTTP_ACCESS_TYPE_NO_PROXY;
            }
            else
            {
                proxy.dwAccessType = AccessType.WINHTTP_ACCESS_TYPE_NAMED_PROXY;
                proxy.lpszProxy = ProxyServer;
                proxy.lpszProxyBypass = BypassList;
            }
            if (PInvoke.WinHttpSetDefaultProxyConfiguration(ref proxy))
            {
                ConsoleControl.WriteInfoLine("Successfully set WinHTTP default proxy");
            }
            else
            {
                var err = Marshal.GetLastWin32Error();
                ConsoleControl.WriteErrorLine($"Failed to set WinHTTP default proxy. Error {err}");
            }
        }

        internal static void ResetDefaultProxy()
        {
            SetDefaultProxy(null, null);
        }

        internal static void SetDisableWpad(int value)
        {
            var key = Registry.LocalMachine.CreateSubKey(Query.DisableWpadKey[1]);
            key.SetValue("DisableWpad", value, RegistryValueKind.DWord);
            ConsoleControl.WriteInfoLine($"Successfully set DisableWpad to {value}");
        }
    }
}
