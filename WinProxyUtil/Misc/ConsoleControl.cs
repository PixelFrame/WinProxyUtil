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
            var temp = Console.BackgroundColor;
            Console.BackgroundColor = InfoColor;
            Console.WriteLine(msg);
            Console.BackgroundColor = temp;
        }

        internal static void WriteErrorLine(string msg)
        {
            WriteColoredLine(msg, ErrorColor);
        }

        internal static void WriteColoredLine(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = RegularColor;
        }
    }
}
