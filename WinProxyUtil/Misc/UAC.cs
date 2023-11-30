using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace WinProxyUtil.Misc
{
    internal static class UAC
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsUserAnAdmin();

        internal static void RestartAsAdmin()
        {
            var args = Environment.GetCommandLineArgs();
            var psi = new ProcessStartInfo
            {
                FileName = args[0],
                UseShellExecute = true,
                Verb = "runas",
            };

            foreach (var arg in args.Skip(1))
            {
                psi.Arguments += $" {arg}";
            }

            Process.Start(psi);
            Environment.Exit(-1);
        }

        public static void EnsureAdmin()
        {
            if (!IsUserAnAdmin())
            {
                Global.StatusCode = 5;
                throw new Exception("The specified operation needs Administrator privilege.");
            }
        }
    }
}
