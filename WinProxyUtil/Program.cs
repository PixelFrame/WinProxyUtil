using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinProxyUtil.Misc;

namespace WinProxyUtil
{
    internal class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 2) { PrintUsage(); }
                else if (args[0].Equals("query", StringComparison.OrdinalIgnoreCase)) { ProcessQuery(args); }
                else if (args[0].Equals("set", StringComparison.OrdinalIgnoreCase)) { ProcessSet(args); }
                else { PrintUsage(); }
            }
            catch(Exception e)
            {
                if (e is IndexOutOfRangeException) { PrintUsage(); }
                else { ConsoleControl.WriteErrorLine(e.Message); }
            }
            finally
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    ConsoleControl.WriteInfoLine("DEBUG PAUSE");
                    Console.ReadKey();
                }
            }
            return Global.StatusCode;
        }

        static void ProcessQuery(string[] args)
        {
            switch (args[1].ToLower())
            {
                case "wininet":
                    if (args[2].Equals("reg", StringComparison.OrdinalIgnoreCase))
                    {
                        var allUsers = false;
                        var includeVpn = false;
                        var includeWow = false;
                        for (int i = 3; i< args.Length; ++i)
                        {
                            switch(args[i].ToLower())
                            {
                                case "-u": allUsers = true;break;
                                case "-v": includeVpn = true;break;
                                case "-w": includeWow = true;break;
                                default: throw new ArgumentException();
                            }
                        }
                        Commands.QueryWinINETReg(allUsers, includeVpn, includeWow);
                    }
                    else if(args[2].Equals("active", StringComparison.OrdinalIgnoreCase))
                    {
                        var includeVpn = false;
                        for (int i = 3; i < args.Length; ++i)
                        {
                            switch (args[i].ToLower())
                            {
                                case "-v": includeVpn = true; break;
                                default: throw new ArgumentException();
                            }
                        }
                        Commands.QueryWinINETProxy(includeVpn);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    break;
                case "winhttp":
                    if (args[2].Equals("default", StringComparison.OrdinalIgnoreCase))
                    {
                        if(args.Length > 3) throw new ArgumentException();
                        Commands.QueryWinHTTPDefault();
                    }
                    else if (args[2].Equals("ie", StringComparison.OrdinalIgnoreCase))
                    {
                        if(args.Length > 3) throw new ArgumentException();
                        Commands.QueryWinHTTPIE();
                    }
                    else if (args[2].Equals("wpad", StringComparison.OrdinalIgnoreCase))
                    {
                        if(args.Length > 3) throw new ArgumentException();
                        Commands.QueryWinHTTPWpad();
                    }
                    else if (args[2].Equals("proxyforurl", StringComparison.OrdinalIgnoreCase))
                    {
                        var url = args[3];
                        var useWpad = false;
                        var useIE = false;
                        var useManual = false;
                        var pacUrl = string.Empty;
                        if(args.Length < 5)
                        {
                            ConsoleControl.WriteErrorLine("Please specify at least one PAC source");
                            throw new ArgumentException();
                        }
                        for (int i = 4; i < args.Length; ++i)
                        {
                            switch (args[i].ToLower())
                            {
                                case "-a": useWpad = true; break;
                                case "-i": useIE = true; break;
                                case "-m": useIE = true; pacUrl = args[++i]; break;
                                default: throw new ArgumentException();
                            }
                        }
                        Commands.QueryWinHTTPProxyForUrl(url, useWpad, useIE, useManual, pacUrl);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                    break;
                case "envvar":
                    var userEnv = false;
                    var machineEnv = false;
                    if (args.Length < 3)
                    {
                        ConsoleControl.WriteErrorLine("Please specify at least one environment variable context");
                        throw new ArgumentException();
                    }

                    for (int i = 2; i < args.Length; ++i)
                    {
                        switch (args[i].ToLower())
                        {
                            case "-u": userEnv = true; break;
                            case "-m": machineEnv = true; break;
                            default: throw new ArgumentException();
                        }
                    }
                    Commands.QueryEnvVar(userEnv, machineEnv);
                    break;
                case "all":
                    Commands.QueryAll();
                    break;
                default :
                    PrintUsage();
                    break;
            }
        }

        static void ProcessSet(string[] args)
        {
            switch (args[1].ToLower())
            {
                case "wininet":
                    var proxySettingsPerUser = -1;
                    var includeVpn = false;
                    var proxyType = -1;
                    string pacUrl = null;
                    string proxyServer = null;
                    string bypassList = null;
                    for (int i = 2; i < args.Length; ++i)
                    {
                        switch (args[i].ToLower())
                        {
                            case "-m": if (proxySettingsPerUser != -1) throw new ArgumentException(); else proxySettingsPerUser = 0; break;
                            case "-u": if (proxySettingsPerUser != -1) throw new ArgumentException(); else proxySettingsPerUser = 1; break;
                            case "-v": includeVpn = true; break;
                            case "-r": if (proxyType != -1) throw new ArgumentException(); else proxyType = 0; break;
                            case "-a": if (proxyType != -1) throw new ArgumentException(); else proxyType = 1; break;
                            case "-p": if (proxyType != -1) throw new ArgumentException(); else proxyType = 2; pacUrl = args[++i]; break;
                            case "-n":
                                if (proxyType != -1) throw new ArgumentException();
                                else
                                {
                                    proxyType = 3; 
                                    proxyServer = args[++i];
                                    if (i + 1 < args.Length && args[i + 1][0] != '-') bypassList = args[++i];
                                }
                                break;
                            default: throw new ArgumentException();
                        }
                    }
                    if(proxyType == -1) throw new ArgumentException();
                    Commands.SetWinINETProxy(proxySettingsPerUser, includeVpn, proxyType, pacUrl, proxyServer, bypassList);
                    break;
                case "winhttp":
                    var disableWpad = -1;
                    var winhttpProxyType = -1;
                    string winhttpProxyServer = null;
                    string winhttpBypassList = null;
                    for (int i = 2; i < args.Length; ++i)
                    {
                        switch (args[i].ToLower())
                        {
                            case "-x": if (disableWpad != -1) throw new ArgumentException(); else disableWpad = 1; break;
                            case "-y": if (disableWpad != -1) throw new ArgumentException(); else disableWpad = 0; break;
                            case "-r": if (winhttpProxyType != -1) throw new ArgumentException(); else proxyType = 0; break;
                            case "-n":
                                if (winhttpProxyType != -1) throw new ArgumentException();
                                else
                                {
                                    winhttpProxyType = 1;
                                    winhttpProxyServer = args[++i];
                                    if (i + 1 < args.Length && args[i + 1][0] != '-') winhttpBypassList = args[++i];
                                }
                                break;
                            default: throw new ArgumentException();
                        }
                    }
                    if (winhttpProxyType == -1) throw new ArgumentException();
                    Commands.SetWinHTTPProxy(disableWpad, winhttpProxyType, winhttpProxyServer, winhttpBypassList);
                    break;
                case "envvar":
                    var userEnv = false;
                    var machineEnv = false;
                    string httpProxy = null;
                    string httpsProxy = null;
                    string ftpProxy = null;
                    string allProxy = null;
                    string noProxy = null;

                    for (int i = 2; i < args.Length; ++i)
                    {
                        switch (args[i].ToLower())
                        {
                            case "-u": userEnv = true; break;
                            case "-m": machineEnv = true; break;
                            case "-h": httpProxy = args[++i]; break;
                            case "-s": httpsProxy = args[++i]; break;
                            case "-f": ftpProxy = args[++i]; break;
                            case "-a": allProxy = args[++i]; break;
                            case "-n": noProxy = args[++i]; break;
                            default: throw new ArgumentException();
                        }
                    }
                    if (userEnv || machineEnv == false)
                    {
                        ConsoleControl.WriteErrorLine("Please specify at least one environment variable context");
                        throw new ArgumentException();
                    }
                    Commands.SetEnvVar(userEnv, machineEnv, httpProxy, httpsProxy, ftpProxy, allProxy, noProxy);
                    break;
                default : throw new InvalidOperationException();
            }
        }

        const string UsageMessage =
@"Windows Proxy Utility
Query and set proxy settings on Windows, including WinINET, WinHTTP and environment variables.

*******************************************************************************************************************

WinProxyUtil query wininet <reg [-u] [-v] [-w] | active [-v]>
    reg         Query proxy settings in registry
    active      Query proxy settings by calling InternetQueryOption
    -u          Query all users' registry. If not specified, only current user's registry will be queried.
    -v          Include proxy settings of RAS connections
    -w          Include WOW6432Node

WinProxyUtil query winhttp <default | ie | wpad | proxyforurl <URL> [-a] [-i] [-m <AutoConfigURL>]>
    default     Query WinHTTP default proxy, same as ""netsh winhttp show proxy""
    ie          Query active WinINET proxy by calling WinHttpGetIEProxyConfigForCurrentUser
    wpad        Detect if WPAD is available in network
    proxyforurl Get proxy config from PAC
    -a          PAC from WPAD
    -i          PAC from WinINET proxy
    -m          Manually specifiy PAC URL

WinProxyUtil query envvar <[-m] [-u]>
    -m          Machine level environment variable
    -u          User level environment variable

WinProxyUtil query all

*******************************************************************************************************************

WinProxyUtil set wininet [-m | -u] [-v] <-r | -a | -p <AutoConfigURL> | -m <ProxyServer> [<BypassList>]>
    -m          Set ProxySettingsPerUser=0
    -u          Set ProxySettingsPerUser=1
    -v          Include proxy settings of RAS connections
    -r          Reset proxy setting
    -a          Set auto detect
    -p          Set PAC URL
    -n          Set named proxy

WinProxyUtil set winhttp [-x | -y] <-r | -m <ProxyServer> [<BypassList>]>
    -x          Set DisableWpad=1
    -y          Set DisableWpad=0
    -r          Reset WinHTTP default proxy setting
    -n          Set WinHTTP default proxy

WinProxyUtil set envvar  [-m] [-u] <[-h <http_proxy>] [-s <HTTPS_PROXY>] [-f <FTP_PROXY>] [-a <ALL_PROXY>] [-n <NO_PROXY>]>
    -m          Machine level environment variable
    -u          User level environment variable
    -h          Set http_proxy
    -s          Set HTTPS_PROXY
    -f          Set FTP_PROXY
    -a          Set ALL_PROXY
    -n          Set NO_PROXY
";

        static void PrintUsage()
        {
            Console.WriteLine(UsageMessage);
            Global.StatusCode = 87;
        }
    }
}
