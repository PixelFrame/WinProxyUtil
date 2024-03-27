using System;

namespace WinProxyUtil.Misc
{
    internal static class ConsoleControl
    {
        static readonly ConsoleColor InfoColor = ConsoleColor.Blue;
        static readonly ConsoleColor WarningColor = ConsoleColor.Yellow;
        static readonly ConsoleColor ErrorColor = ConsoleColor.Red;
        static readonly ConsoleColor RegularColor = ConsoleColor.Gray;

        internal static void WriteWarningLine(string msg)
        {
            WriteColoredLine(msg, WarningColor);
        }

        internal static void WriteInfoLine(string msg)
        {
            WriteColoredLine(msg, InfoColor);
        }

        internal static void WriteInfoLine2(string msg)
        {
            WriteColoredLine(msg, RegularColor, InfoColor);
        }

        internal static void WriteErrorLine(string msg)
        {
            WriteColoredLine(msg, ErrorColor);
        }

        internal static void WriteColoredLine(string msg, ConsoleColor foreColor)
        {
            var originalFore = Console.ForegroundColor;
            Console.ForegroundColor = foreColor;
            Console.WriteLine(msg);
            Console.ForegroundColor = originalFore;
        }

        internal static void WriteColoredLine(string msg, ConsoleColor foreColor, ConsoleColor backColor)
        {
            var originalFore = Console.ForegroundColor;
            var originalBack = Console.BackgroundColor;
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
            Console.Write(msg);
            Console.ForegroundColor = originalFore;
            Console.BackgroundColor = originalBack;
            Console.WriteLine();
        }
    }
}
