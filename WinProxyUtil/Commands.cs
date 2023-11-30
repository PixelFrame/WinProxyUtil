using Microsoft.Win32;
using System;
using System.Collections.Generic;
using WinProxyUtil.Misc;

namespace WinProxyUtil
{
    internal static class Commands
    {
        const string splitter = "*******************************************************************************************************************";
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
                UAC.EnsureAdmin();
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

        internal static void QueryAll()
        {
            UAC.EnsureAdmin();
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
                UAC.EnsureAdmin();
                WinINET.Set.SetProxyPerUser(proxySettingsPerUser);
            }

            if (proxyType == -1) return;
            var conns = new List<string>() { "DefaultConnectionSettings" };
            if (includeVpn)
            {
                conns.AddRange(WinINET.Query.GetRasEntries());
            }

            foreach (var conn in conns)
            {
                switch (proxyType)
                {
                    case 0:
                        WinINET.Set.ClearProxy(conn);
                        break;
                    case 1:
                        WinINET.Set.SetProxy(true, null, null, null, conn);
                        break;
                    case 2:
                        WinINET.Set.SetProxy(null, pacUrl, null, null, conn);
                        break;
                    case 3:
                        WinINET.Set.SetProxy(null, null, proxyServer, bypassList, conn);
                        break;
                }
                if (Global.StatusCode != 0) break;
            }
        }

        internal static void SetWinHTTPProxy(int DisableWpad, int WinhttpProxyType, string ProxyServer, string BypassList)
        {
            UAC.EnsureAdmin();
            if (DisableWpad != -1)
            {
                WinHTTP.Set.SetDisableWpad(DisableWpad);
            }
            if (WinhttpProxyType == 0)
            {
                WinHTTP.Set.ResetDefaultProxy();
            }
            else
            {
                WinHTTP.Set.SetDefaultProxy(ProxyServer, BypassList);
            }
        }

        internal static void SetEnvVar(bool UserEnv, bool MachineEnv, bool Reset, string HttpProxy, string HttpsProxy, string FtpProxy, string AllProxy, string NoProxy)
        {
            if (UserEnv)
            {
                try
                {
                    if (Reset)
                    {
                        EnvVar.Set(0, null, false);
                        EnvVar.Set(1, null, false);
                        EnvVar.Set(2, null, false);
                        EnvVar.Set(3, null, false);
                        EnvVar.Set(4, null, false);
                    }
                    else
                    {
                        if (HttpProxy != null) EnvVar.Set(0, HttpProxy, false);
                        if (HttpsProxy != null) EnvVar.Set(1, HttpsProxy, false);
                        if (FtpProxy != null) EnvVar.Set(2, FtpProxy, false);
                        if (AllProxy != null) EnvVar.Set(3, AllProxy, false);
                        if (NoProxy != null) EnvVar.Set(4, NoProxy, false);
                    }
                    ConsoleControl.WriteInfoLine("Successfully set user proxy environment variables");
                }
                catch (Exception e)
                {
                    ConsoleControl.WriteErrorLine($"Failed to set user proxy environment variables, {e.Message}");
                    Global.StatusCode = e.HResult;
                }
            }
            if (MachineEnv)
            {
                SetEnvVarMachine(Reset, HttpProxy, HttpsProxy, FtpProxy, AllProxy, NoProxy);
            }
        }

        private static void SetEnvVarMachine(bool Reset, string HttpProxy, string HttpsProxy, string FtpProxy, string AllProxy, string NoProxy)
        {
            UAC.EnsureAdmin();
            try
            {
                if (Reset)
                {
                    EnvVar.Set(0, null, true);
                    EnvVar.Set(1, null, true);
                    EnvVar.Set(2, null, true);
                    EnvVar.Set(3, null, true);
                    EnvVar.Set(4, null, true);
                }
                else
                {
                    if (HttpProxy != null) EnvVar.Set(0, HttpProxy, true);
                    if (HttpsProxy != null) EnvVar.Set(1, HttpsProxy, true);
                    if (FtpProxy != null) EnvVar.Set(2, FtpProxy, true);
                    if (AllProxy != null) EnvVar.Set(3, AllProxy, true);
                    if (NoProxy != null) EnvVar.Set(4, NoProxy, true);
                }
                ConsoleControl.WriteInfoLine("Successfully set machine proxy environment variables");
            }
            catch (Exception e)
            {
                ConsoleControl.WriteErrorLine($"Failed to set machine proxy environment variables, {e.Message}");
                Global.StatusCode = e.HResult;
            }

        }
    }
}
