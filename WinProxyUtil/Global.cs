namespace WinProxyUtil
{
    internal static class Global
    {
        internal static int StatusCode = 0;

        internal const string UsageMessage =
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

WinProxyUtil set wininet <[-m | -u] [[-v] -r | -a | -p <AutoConfigURL> | -n <ProxyServer> [<BypassList>]]>
    -m          Set ProxySettingsPerUser=0
    -u          Set ProxySettingsPerUser=1
    -v          Include proxy settings of RAS connections
    -r          Reset proxy setting
    -a          Set auto detect
    -p          Set PAC URL
    -n          Set named proxy

WinProxyUtil set winhttp <[-x | -y] [-r | -n <ProxyServer> [<BypassList>]>
    -x          Set DisableWpad=1
    -y          Set DisableWpad=0
    -r          Reset WinHTTP default proxy setting
    -n          Set WinHTTP default proxy

WinProxyUtil set envvar <[-m] [-u]> <-r | [-h <http_proxy>] [-s <HTTPS_PROXY>] [-f <FTP_PROXY>] [-a <ALL_PROXY>] [-n <NO_PROXY>]>
    -m          Machine level environment variable
    -u          User level environment variable
    -r          Reset all environment variables
    -h          Set http_proxy
    -s          Set HTTPS_PROXY
    -f          Set FTP_PROXY
    -a          Set ALL_PROXY
    -n          Set NO_PROXY
";

    }
}
