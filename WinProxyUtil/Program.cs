using System;
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
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException || e is InvalidOperationException) { PrintUsage(); }
                else if (e is ArgumentException) { ConsoleControl.WriteErrorLine(e.Message); Global.StatusCode = 87; }
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
                        for (int i = 3; i < args.Length; ++i)
                        {
                            switch (args[i].ToLower())
                            {
                                case "-u": allUsers = true; break;
                                case "-v": includeVpn = true; break;
                                case "-w": includeWow = true; break;
                                default: throw new ArgumentException($"Invalid arugment: {args[i]}");
                            }
                        }
                        Commands.QueryWinINETReg(allUsers, includeVpn, includeWow);
                    }
                    else if (args[2].Equals("active", StringComparison.OrdinalIgnoreCase))
                    {
                        var includeVpn = false;
                        for (int i = 3; i < args.Length; ++i)
                        {
                            switch (args[i].ToLower())
                            {
                                case "-v": includeVpn = true; break;
                                default: throw new ArgumentException($"Invalid arugment: {args[i]}");
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
                        if (args.Length > 3) throw new ArgumentException("Too many arguments");
                        Commands.QueryWinHTTPDefault();
                    }
                    else if (args[2].Equals("ie", StringComparison.OrdinalIgnoreCase))
                    {
                        if (args.Length > 3) throw new ArgumentException("Too many arguments");
                        Commands.QueryWinHTTPIE();
                    }
                    else if (args[2].Equals("wpad", StringComparison.OrdinalIgnoreCase))
                    {
                        if (args.Length > 3) throw new ArgumentException("Too many arguments");
                        Commands.QueryWinHTTPWpad();
                    }
                    else if (args[2].Equals("proxyforurl", StringComparison.OrdinalIgnoreCase))
                    {
                        var url = args[3];
                        var useWpad = false;
                        var useIE = false;
                        var useManual = false;
                        var pacUrl = string.Empty;
                        if (args.Length < 5)
                        {
                            throw new ArgumentException("Please specify at least one PAC source");
                        }
                        for (int i = 4; i < args.Length; ++i)
                        {
                            switch (args[i].ToLower())
                            {
                                case "-a": useWpad = true; break;
                                case "-i": useIE = true; break;
                                case "-m": useManual = true; pacUrl = args[++i]; break;
                                default: throw new ArgumentException($"Invalid arugment: {args[i]}");
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
                        throw new ArgumentException("Please specify at least one environment variable context");
                    }

                    for (int i = 2; i < args.Length; ++i)
                    {
                        switch (args[i].ToLower())
                        {
                            case "-u": userEnv = true; break;
                            case "-m": machineEnv = true; break;
                            default: throw new ArgumentException($"Invalid arugment: {args[i]}");
                        }
                    }
                    Commands.QueryEnvVar(userEnv, machineEnv);
                    break;
                case "all":
                    Commands.QueryAll();
                    break;
                default:
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
                            case "-m": if (proxySettingsPerUser != -1) throw new ArgumentException("-m and -u cannot be used at the same time"); else proxySettingsPerUser = 0; break;
                            case "-u": if (proxySettingsPerUser != -1) throw new ArgumentException("-m and -u cannot be used at the same time"); else proxySettingsPerUser = 1; break;
                            case "-v": includeVpn = true; break;
                            case "-r": if (proxyType != -1) throw new ArgumentException("-r -a -p -n cannot be used at the same time"); else proxyType = 0; break;
                            case "-a": if (proxyType != -1) throw new ArgumentException("-r -a -p -n cannot be used at the same time"); else proxyType = 1; break;
                            case "-p": if (proxyType != -1) throw new ArgumentException("-r -a -p -n cannot be used at the same time"); else proxyType = 2; pacUrl = args[++i]; break;
                            case "-n":
                                if (proxyType != -1) throw new ArgumentException("-r -a -p -n cannot be used at the same time");
                                else
                                {
                                    proxyType = 3;
                                    proxyServer = args[++i];
                                    if (i + 1 < args.Length && args[i + 1][0] != '-') bypassList = args[++i];
                                }
                                break;
                            default: throw new ArgumentException($"Invalid arugment: {args[i]}");
                        }
                    }
                    if (proxyType == -1 && proxySettingsPerUser == -1) throw new ArgumentException("Please specify the proxy to be set");
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
                            case "-x": if (disableWpad != -1) throw new ArgumentException("-x and -y cannot be used at the same time"); else disableWpad = 1; break;
                            case "-y": if (disableWpad != -1) throw new ArgumentException("-x and -y cannot be used at the same time"); else disableWpad = 0; break;
                            case "-r": if (winhttpProxyType != -1) throw new ArgumentException("-r and -n cannot be used at the same time"); else winhttpProxyType = 0; break;
                            case "-n":
                                if (winhttpProxyType != -1) throw new ArgumentException("-r and -n cannot be used at the same time");
                                else
                                {
                                    winhttpProxyType = 1;
                                    winhttpProxyServer = args[++i];
                                    if (i + 1 < args.Length && args[i + 1][0] != '-') winhttpBypassList = args[++i];
                                }
                                break;
                            default: throw new ArgumentException($"Invalid argument: {args[i]}");
                        }
                    }
                    if (winhttpProxyType == -1 && disableWpad == -1) throw new ArgumentException();
                    Commands.SetWinHTTPProxy(disableWpad, winhttpProxyType, winhttpProxyServer, winhttpBypassList);
                    break;
                case "envvar":
                    var userEnv = false;
                    var machineEnv = false;
                    var reset = false;
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
                            case "-r": reset = true; break;
                            default: throw new ArgumentException($"Invalid argument: {args[i]}");
                        }
                    }
                    if (!(userEnv || machineEnv))
                    {
                        throw new ArgumentException("Please specify at least one environment variable context");
                    }
                    Commands.SetEnvVar(userEnv, machineEnv, reset, httpProxy, httpsProxy, ftpProxy, allProxy, noProxy);
                    break;
                default: throw new InvalidOperationException();
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine(Global.UsageMessage);
            Global.StatusCode = 87;
        }
    }
}
