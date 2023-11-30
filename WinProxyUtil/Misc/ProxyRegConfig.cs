using System;
using System.Runtime.InteropServices;
using System.Text;
using WinProxyUtil.WinINET;

namespace WinProxyUtil.Misc
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct ProxyRegConfig
    {
        internal uint Magic;     // Must be 0x46
        internal uint Version;
        internal PerConnFlag Flag;
        internal uint ProxyServerLength;
        internal string ProxyServer;
        internal uint BypassListLength;
        internal string BypassList;
        internal uint PacUrlLength;
        internal string PacUrl;
        internal uint AFlag;
        internal string LastKnownACU;

        internal ProxyRegConfig(byte[] data)
        {
            var idx = 0;
            Magic = BitConverter.ToUInt32(data, idx);
            idx += 4;
            Version = BitConverter.ToUInt32(data, idx);
            idx += 4;
            Flag = (PerConnFlag)BitConverter.ToUInt32(data, idx);
            idx += 4;
            ProxyServerLength = BitConverter.ToUInt32(data, idx);
            idx += 4;
            ProxyServer = Encoding.ASCII.GetString(data, idx, (int)ProxyServerLength);
            idx += (int)ProxyServerLength;
            BypassListLength = BitConverter.ToUInt32(data, idx);
            idx += 4;
            BypassList = Encoding.ASCII.GetString(data, idx, (int)BypassListLength);
            idx += (int)BypassListLength;
            PacUrlLength = BitConverter.ToUInt32(data, idx);
            idx += 4;
            PacUrl = Encoding.ASCII.GetString(data, idx, (int)PacUrlLength);
            idx += (int)PacUrlLength;
            AFlag = BitConverter.ToUInt32(data, idx);
            idx += 4;
            LastKnownACU = Encoding.ASCII.GetString(data, idx, data.Length - idx);
        }

        internal void Print()
        {
            Console.WriteLine($"Version         : {Version}");
            Console.WriteLine($"Flags           : {Flag}");
            Console.WriteLine($"ProxyServer     : {ProxyServer}");
            Console.WriteLine($"BypassList      : {BypassList}");
            Console.WriteLine($"Auto Config URL : {PacUrl}");
        }
    }
}
