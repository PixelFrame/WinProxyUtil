using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using WinProxyUtil.Misc;

namespace WinProxyUtil.WinINET
{
    internal static class Query
    {
        internal static bool QueryProxy(string Connection, out int Win32Error)
        {
            var iRes = false;
            var list = new INTERNET_PER_CONN_OPTION_LIST();
            var option = new INTERNET_PER_CONN_OPTION[5];
            var optSize = Marshal.SizeOf(option[0]);
            var listSize = Marshal.SizeOf(list);
            IntPtr pOptions = Marshal.AllocHGlobal(optSize * 5);
            IntPtr pCurrentOption = new IntPtr(pOptions.ToInt64());

            try
            {
                option[0].dwOption = PerConnOption.INTERNET_PER_CONN_FLAGS;
                option[1].dwOption = PerConnOption.INTERNET_PER_CONN_AUTODISCOVERY_FLAGS;
                option[2].dwOption = PerConnOption.INTERNET_PER_CONN_AUTOCONFIG_URL;
                option[3].dwOption = PerConnOption.INTERNET_PER_CONN_PROXY_SERVER;
                option[4].dwOption = PerConnOption.INTERNET_PER_CONN_PROXY_BYPASS;

                for (var i = 0; i < 5; i++)
                {
                    Marshal.StructureToPtr(option[i], pCurrentOption, true);
                    pCurrentOption += optSize;
                }

                list.dwSize = Marshal.SizeOf(list);
                list.pszConnection = Connection;
                list.dwOptionCount = 5;
                list.dwOptionError = 0;
                list.pOptions = pOptions;

                iRes = PInvoke.InternetQueryOption(IntPtr.Zero, OptionFlag.INTERNET_OPTION_PER_CONNECTION_OPTION, ref list, ref listSize);
                Win32Error = Marshal.GetLastWin32Error();

                pCurrentOption = list.pOptions;
                for (var i = 0; i < 5; i++)
                {
                    option[i] = Marshal.PtrToStructure<INTERNET_PER_CONN_OPTION>(pCurrentOption);
                    pCurrentOption += optSize;
                }

                Console.WriteLine($"Flags           : {option[0].Value.dwValue}");
                Console.WriteLine($"Auto Config URL : {Marshal.PtrToStringAuto(option[2].Value.pszValue)}");
                Console.WriteLine($"Proxy Server    : {Marshal.PtrToStringAuto(option[3].Value.pszValue)}");
                Console.WriteLine($"Bypass List     : {Marshal.PtrToStringAuto(option[4].Value.pszValue)}");
            }
            finally
            {
                Marshal.FreeHGlobal(pOptions);
            }
            return iRes;
        }

        internal static bool GetProxyPerMachine()
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"Software\Policies\Microsoft\Windows\CurrentVersion\Internet Settings");
                var value = key.GetValue("ProxySettingsPerUser") as int?;
                return value == 0;
            }
            catch { return false; }
        }

        internal static string[] GetRasEntries()
        {
            var cb = Marshal.SizeOf(typeof(RASENTRYNAME));
            var entries = 0;
            var entryNames = new RASENTRYNAME[1];
            entryNames[0].dwSize = Marshal.SizeOf(typeof(RASENTRYNAME));

            _ = PInvoke.RasEnumEntries(IntPtr.Zero, IntPtr.Zero, entryNames, ref cb, ref entries);
            if (entries == 0) return Array.Empty<string>();

            entryNames = new RASENTRYNAME[entries];
            for (var i = 0; i < entries; i++)
            {
                entryNames[i].dwSize = Marshal.SizeOf(typeof(RASENTRYNAME));
            }

            _ = PInvoke.RasEnumEntries(IntPtr.Zero, IntPtr.Zero, entryNames, ref cb, ref entries);
            return entryNames.Select(e => e.szEntryName).ToArray();
        }

        internal static void QueryRegConfig(RegistryKey Hive, string[] Connections, bool IsWow)
        {
            var KeyPath = IsWow ?
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Internet Settings\Connections" :
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Connections";
            var Key = Hive.OpenSubKey(KeyPath);
            if (Key == null)
            {
                ConsoleControl.WriteWarningLine($"{Hive.Name}\\{KeyPath}");
                ConsoleControl.WriteWarningLine("Registry Key Does Not Exist");
                Console.WriteLine();
                return;
            }
            if (Connections.Length == 0)
            {
                Connections = Key.GetValueNames();
            }
            foreach (var Connection in Connections)
            {
                if (Key.GetValue(Connection) is byte[] value)
                {
                    try
                    {
                        var config = new ProxyRegConfig(value);
                        if (config.Magic != 0x46)
                        {
                            ConsoleControl.WriteErrorLine($"{Key.Name}\\{Connection}");
                            ConsoleControl.WriteErrorLine("Invalid Registry Value");
                        }
                        else
                        {
                            ConsoleControl.WriteInfoLine($"{Key.Name}\\{Connection}");
                            config.Print();
                        }
                    }
                    catch
                    {
                        ConsoleControl.WriteErrorLine($"{Key.Name}\\{Connection}");
                        ConsoleControl.WriteErrorLine("Invalid Registry Value");
                    }
                }
                else
                {
                    ConsoleControl.WriteWarningLine($"{Key.Name}\\{Connection}");
                    ConsoleControl.WriteWarningLine("Registry Value Does Not Exist");
                }
                Console.WriteLine();
            }
        }
    }
}
