using System;

namespace WinProxyUtil.Misc
{
    internal static class EnvVar
    {
        static readonly string[] vars = { "http_proxy", "HTTPS_PROXY", "FTP_PROXY", "ALL_PROXY", "NO_PROXY" };
        internal static void Query(bool isMachine)
        {
            if (isMachine)
            {
                foreach (var var in vars)
                {
                    Console.WriteLine($"{var,-11} : {Environment.GetEnvironmentVariable(var, EnvironmentVariableTarget.Machine)}");
                }
            }
            else
            {
                foreach (var var in vars)
                {
                    Console.WriteLine($"{var,-11} : {Environment.GetEnvironmentVariable(var, EnvironmentVariableTarget.User)}");
                }
            }
        }

        internal static void Set(int target, string value, bool isMachine)
        {
            if (isMachine)
            {
                Environment.SetEnvironmentVariable(vars[target], value, EnvironmentVariableTarget.Machine);
            }
            else
            {
                Environment.SetEnvironmentVariable(vars[target], value, EnvironmentVariableTarget.User);
            }
        }
    }
}
