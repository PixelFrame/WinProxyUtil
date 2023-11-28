using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinProxyUtil.Misc;

namespace WinProxyUtil
{
    internal static class Commands
    {
        static readonly string splitter = "*******************************************************************************************************************";
        static readonly string[] defaultConn = new string[]
            {
                "DefaultConnectionSettings", "SavedLegacySettings"
            };

        internal static void QueryWinINETReg(bool AllUsers, bool IncludeVpn, bool IncludeWow)
        {
            ConsoleControl.WriteInfoLine2("WinINET From Registry");
            var perMachine = WinINET.Query.GetProxyPerMachine();
            if (perMachine)
            {
                ConsoleControl.WriteInfoLine("WinINET Proxy Settings are set to be PER MACHINE.");
                Console.WriteLine();
            }
            else
            {
                ConsoleControl.WriteInfoLine("WinINET Proxy Settings are set to be PER USER.");
                Console.WriteLine();
            }

            var hives = new List<RegistryKey>();
            if (AllUsers)
            {
                _ = new AdminRequiredAttribute();
                var hku = Registry.Users;
                foreach (var u in hku.GetSubKeyNames())
                {
                    hives.Add(hku.OpenSubKey(u));
                }
            }
            else
            {
                hives.Add(Registry.CurrentUser);
            }
            hives.Add(Registry.LocalMachine);

            var conns = IncludeVpn ? Array.Empty<string>() : defaultConn;
            foreach (var hive in hives)
            {
                WinINET.Query.QueryRegConfig(hive, conns, false);
                if (IncludeWow)
                {
                    WinINET.Query.QueryRegConfig(hive, conns, true);
                }
            }
        }

        internal static void QueryWinINETProxy(bool IncludeVpn)
        {
            ConsoleControl.WriteInfoLine2("WinINET Proxy from InternetQueryOption");
            var conns = new List<string>(defaultConn);
            if (IncludeVpn)
            {
                conns.AddRange(WinINET.Query.GetRasEntries());
            }
            foreach (var conn in conns)
            {
                ConsoleControl.WriteInfoLine(conn);
                if (!WinINET.Query.QueryProxy(conn, out int err))
                {
                    ConsoleControl.WriteErrorLine($"Failed to query WinINET proxy, error {err}");
                }
                Console.WriteLine();
            }
        }

        internal static void QueryWinHTTPDefault()
        {
            ConsoleControl.WriteInfoLine2("WinHTTP Default Proxy");
            WinHTTP.Query.QueryDefaultProxy();
        }

        internal static void QueryWinHTTPIE()
        {
            ConsoleControl.WriteInfoLine2("WinHTTP Get IE Proxy");
            WinHTTP.Query.QueryWinINETProxy();
        }

        internal static void QueryWinHTTPWpad()
        {
            ConsoleControl.WriteInfoLine2("WinHTTP Detect Auto Proxy");
            WinHTTP.Query.QueryDisableWpad();
            WinHTTP.Query.QueryAutoProxy();
        }

        internal static void QueryWinHTTPProxyForUrl(string Url, bool UseWpad, bool UseIE, bool UseManual, string PacUrl)
        {
            ConsoleControl.WriteInfoLine2("WinHTTP Get Proxy for Url");
            if (UseWpad)
            {
                ConsoleControl.WriteInfoLine($"Get Proxy Setting for {Url} with WPAD");
                var WpadPac = WinHTTP.Query.GetAutoProxyUrl();
                if (WpadPac.Length > 0)
                {
                    foreach (var pac in WpadPac)
                    {
                        ConsoleControl.WriteInfoLine($"PAC URL: {pac}");
                        WinHTTP.Query.GetProxyForUrl(Url, pac);
                        Console.WriteLine();
                    }
                }
                else
                {
                    ConsoleControl.WriteWarningLine("No WPAD detected");
                    Console.WriteLine();
                }
            }
            if (UseIE)
            {
                ConsoleControl.WriteInfoLine($"Get Proxy Setting for {Url} with WinINET PAC");
                var IEPac = WinHTTP.Query.GetWinINETPacUrl();
                if (IEPac != null)
                {
                    ConsoleControl.WriteInfoLine($"PAC URL: {IEPac}");
                    WinHTTP.Query.GetProxyForUrl(Url, IEPac);
                    Console.WriteLine();
                }
                else
                {
                    ConsoleControl.WriteWarningLine("No PAC Configured");
                    Console.WriteLine();
                }
            }
            if (UseManual)
            {
                ConsoleControl.WriteInfoLine($"Get Proxy Setting For {Url} with PAC {PacUrl}");

                WinHTTP.Query.GetProxyForUrl(Url, PacUrl);
                Console.WriteLine();
            }
        }

        internal static void QueryEnvVar(bool UserEnv, bool MachineEnv)
        {
            ConsoleControl.WriteInfoLine2("Proxy Environment Variables");
            if (UserEnv)
            {
                ConsoleControl.WriteInfoLine("User Environment Variables");
                EnvVar.Query(false);
                Console.WriteLine();
            }
            if (MachineEnv)
            {
                ConsoleControl.WriteInfoLine("Machine Environment Variables");
                EnvVar.Query(true);
                Console.WriteLine();
            }
        }

        [AdminRequired]
        internal static void QueryAll()
        {
            QueryWinINETProxy(true);
            Console.WriteLine(splitter);
            QueryWinINETReg(true, true, true);
            Console.WriteLine(splitter);
            QueryWinHTTPDefault();
            Console.WriteLine(splitter);
            QueryWinHTTPIE();
            Console.WriteLine(splitter);
            QueryWinHTTPWpad();
            Console.WriteLine(splitter);
            QueryWinHTTPProxyForUrl("https://www.example.com", true, true, false, string.Empty);
            Console.WriteLine(splitter);
            QueryEnvVar(true, true);
            Console.WriteLine(splitter);
        }

        internal static void SetWinINETProxy(int proxySettingsPerUser, bool includeVpn, int proxyType, string pacUrl, string proxyServer, string bypassList)
        {
            if (proxySettingsPerUser != -1)
            {
                WinINET.Set.SetProxyPerUser(proxySettingsPerUser);
            }

            var conns = new List<string>() { "DefaultConnectionSettings" };
            if (includeVpn)
            {
                conns.AddRange(WinINET.Query.GetRasEntries());
            }

            int err;
            foreach (var conn in conns)
            {
                switch (proxyType)
                {
                    case 0:
                        if (WinINET.Set.ClearProxy(conn, out err)) ConsoleControl.WriteInfoLine($"Successfully cleared proxy on {conn}");
                        else ConsoleControl.WriteErrorLine($"Failed to clear proxy on {conn}, error {err}");
                        break;
                    case 1:
                        if (WinINET.Set.SetProxy(true, null, null, null, conn, out err)) ConsoleControl.WriteInfoLine($"Successfully set proxy on {conn}");
                        else ConsoleControl.WriteErrorLine($"Failed to set proxy on {conn}, error {err}");
                        break;
                    case 2:
                        if (WinINET.Set.SetProxy(null, pacUrl, null, null, conn, out err)) ConsoleControl.WriteInfoLine($"Successfully set proxy on {conn}");
                        else ConsoleControl.WriteErrorLine($"Failed to set proxy on {conn}, error {err}");
                        break;
                    case 3:
                        if (WinINET.Set.SetProxy(null, null, proxyServer, bypassList, conn, out err)) ConsoleControl.WriteInfoLine($"Successfully set proxy on {conn}");
                        else ConsoleControl.WriteErrorLine($"Failed to set proxy on {conn}, error {err}");
                        break;
                }
            }
        }

        [AdminRequired]
        internal static void SetWinHTTPProxy(int DisableWpad, int WinhttpProxyType, string ProxyServer, string BypassList)
        {
            if (DisableWpad != -1)
            {
                try
                {
                    WinHTTP.Set.SetDisableWpad(DisableWpad);
                    ConsoleControl.WriteInfoLine("Successfully set DisableWpad");
                }
                catch (Exception e)
                {
                    ConsoleControl.WriteErrorLine($"Failed to set DisableWpad, {e.Message}");
                }
            }
            if (WinhttpProxyType == 0)
            {
                try
                {
                    WinHTTP.Set.ResetDefaultProxy();
                    ConsoleControl.WriteInfoLine("Successfully reset WinHTTP default proxy");
                }
                catch (Exception e)
                {
                    ConsoleControl.WriteErrorLine($"Failed to reset WinHTTP default proxy, {e.Message}");
                }
            }
            else
            {
                try
                {
                    WinHTTP.Set.SetDefaultProxy(ProxyServer, BypassList);
                    ConsoleControl.WriteInfoLine("Successfully set WinHTTP default proxy");
                }
                catch (Exception e)
                {
                    ConsoleControl.WriteErrorLine($"Failed to set WinHTTP default proxy, {e.Message}");
                }
            }
        }

        internal static void SetEnvVar(bool UserEnv, bool MachineEnv, string HttpProxy, string HttpsProxy, string FtpProxy, string AllProxy, string NoProxy)
        {
            if (UserEnv)
            {
                try
                {
                    EnvVar.Set(0, HttpProxy, false);
                    EnvVar.Set(1, HttpsProxy, false);
                    EnvVar.Set(2, FtpProxy, false);
                    EnvVar.Set(3, AllProxy, false);
                    EnvVar.Set(4, NoProxy, false);
                    ConsoleControl.WriteInfoLine("Successfully set user proxy environment variables");
                }
                catch (Exception e)
                {
                    ConsoleControl.WriteErrorLine($"Failed to set user proxy environment variables, {e.Message}");
                }
            }
            if (MachineEnv)
            {
                SetEnvVarMachine(HttpProxy, HttpsProxy, FtpProxy, AllProxy, NoProxy);
            }
        }

        [AdminRequired]
        private static void SetEnvVarMachine(string HttpProxy, string HttpsProxy, string FtpProxy, string AllProxy, string NoProxy)
        {
            try
            {
                EnvVar.Set(0, HttpProxy, true);
                EnvVar.Set(1, HttpsProxy, true);
                EnvVar.Set(2, FtpProxy, true);
                EnvVar.Set(3, AllProxy, true);
                EnvVar.Set(4, NoProxy, true);
                ConsoleControl.WriteInfoLine("Successfully set machine proxy environment variables");
            }
            catch (Exception e)
            {
                ConsoleControl.WriteErrorLine($"Failed to set machine proxy environment variables, {e.Message}");
            }

        }
    }
}
