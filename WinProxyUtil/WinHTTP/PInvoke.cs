using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinProxyUtil.WinHTTP
{
    internal static class PInvoke
    {
        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr WinHttpOpen(
            string pwszAgentW,
            AccessType dwAccessType,
            string pwszProxyW,
            string pwszProxyBypassW,
            OpenFlag dwFlags);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpGetProxyForUrl(
            IntPtr hSession,
            string lpcwszUrl,
            ref WINHTTP_AUTOPROXY_OPTIONS pAutoProxyOptions,
            ref WINHTTP_PROXY_INFO pProxyInfo);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpGetDefaultProxyConfiguration(ref WINHTTP_PROXY_INFO pProxyInfo);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpSetDefaultProxyConfiguration(ref WINHTTP_PROXY_INFO pProxyInfo);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpGetIEProxyConfigForCurrentUser(ref WINHTTP_CURRENT_USER_IE_PROXY_CONFIG pProxyInfo);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpDetectAutoProxyConfigUrl(
            AutoDetectFlag dwAutoDetectFlags,
            ref string ppwstrAutoConfigUrl);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WinHttpResetAutoProxy(
            IntPtr hSession,
            ResetFlag dwFlags);

        [DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WinHttpCloseHandle(IntPtr hInternet);

        public static readonly Dictionary<int, string> ErrorMessage = new Dictionary<int, string>()
        {
            {12001, "ERROR_WINHTTP_OUT_OF_HANDLES"},
            {12002, "ERROR_WINHTTP_TIMEOUT"},
            {12004, "ERROR_WINHTTP_INTERNAL_ERROR"},
            {12005, "ERROR_WINHTTP_INVALID_URL"},
            {12006, "ERROR_WINHTTP_UNRECOGNIZED_SCHEME"},
            {12007, "ERROR_WINHTTP_NAME_NOT_RESOLVED"},
            {12009, "ERROR_WINHTTP_INVALID_OPTION"},
            {12011, "ERROR_WINHTTP_OPTION_NOT_SETTABLE"},
            {12012, "ERROR_WINHTTP_SHUTDOWN"},
            {12015, "ERROR_WINHTTP_LOGIN_FAILURE"},
            {12017, "ERROR_WINHTTP_OPERATION_CANCELLED"},
            {12018, "ERROR_WINHTTP_INCORRECT_HANDLE_TYPE"},
            {12019, "ERROR_WINHTTP_INCORRECT_HANDLE_STATE"},
            {12029, "ERROR_WINHTTP_CANNOT_CONNECT"},
            {12030, "ERROR_WINHTTP_CONNECTION_ERROR"},
            {12032, "ERROR_WINHTTP_RESEND_REQUEST"},
            {12044, "ERROR_WINHTTP_CLIENT_AUTH_CERT_NEEDED"},
            {12100, "ERROR_WINHTTP_CANNOT_CALL_BEFORE_OPEN"},
            {12101, "ERROR_WINHTTP_CANNOT_CALL_BEFORE_SEND"},
            {12102, "ERROR_WINHTTP_CANNOT_CALL_AFTER_SEND"},
            {12103, "ERROR_WINHTTP_CANNOT_CALL_AFTER_OPEN"},
            {12150, "ERROR_WINHTTP_HEADER_NOT_FOUND"},
            {12152, "ERROR_WINHTTP_INVALID_SERVER_RESPONSE"},
            {12153, "ERROR_WINHTTP_INVALID_HEADER"},
            {12154, "ERROR_WINHTTP_INVALID_QUERY_REQUEST"},
            {12155, "ERROR_WINHTTP_HEADER_ALREADY_EXISTS"},
            {12156, "ERROR_WINHTTP_REDIRECT_FAILED"},
            {12178, "ERROR_WINHTTP_AUTO_PROXY_SERVICE_ERROR"},
            {12166, "ERROR_WINHTTP_BAD_AUTO_PROXY_SCRIPT"},
            {12167, "ERROR_WINHTTP_UNABLE_TO_DOWNLOAD_SCRIPT"},
            {12176, "ERROR_WINHTTP_UNHANDLED_SCRIPT_TYPE"},
            {12177, "ERROR_WINHTTP_SCRIPT_EXECUTION_ERROR"},
            {12172, "ERROR_WINHTTP_NOT_INITIALIZED"},
            {12175, "ERROR_WINHTTP_SECURE_FAILURE"},
            {12037, "ERROR_WINHTTP_SECURE_CERT_DATE_INVALID"},
            {12038, "ERROR_WINHTTP_SECURE_CERT_CN_INVALID"},
            {12045, "ERROR_WINHTTP_SECURE_INVALID_CA"},
            {12057, "ERROR_WINHTTP_SECURE_CERT_REV_FAILED"},
            {12157, "ERROR_WINHTTP_SECURE_CHANNEL_ERROR"},
            {12169, "ERROR_WINHTTP_SECURE_INVALID_CERT"},
            {12170, "ERROR_WINHTTP_SECURE_CERT_REVOKED"},
            {12179, "ERROR_WINHTTP_SECURE_CERT_WRONG_USAGE"},
            {12180, "ERROR_WINHTTP_AUTODETECTION_FAILED"},
            {12181, "ERROR_WINHTTP_HEADER_COUNT_EXCEEDED"},
            {12182, "ERROR_WINHTTP_HEADER_SIZE_OVERFLOW"},
            {12183, "ERROR_WINHTTP_CHUNKED_ENCODING_HEADER_SIZE_OVERFLOW"},
            {12184, "ERROR_WINHTTP_RESPONSE_DRAIN_OVERFLOW"},
            {12185, "ERROR_WINHTTP_CLIENT_CERT_NO_PRIVATE_KEY"},
            {12186, "ERROR_WINHTTP_CLIENT_CERT_NO_ACCESS_PRIVATE_KEY"},
            {12187, "ERROR_WINHTTP_CLIENT_AUTH_CERT_NEEDED_PROXY"},
            {12188, "ERROR_WINHTTP_SECURE_FAILURE_PROXY"},
            {12189, "ERROR_WINHTTP_RESERVED_189"},
            {12190, "ERROR_WINHTTP_HTTP_PROTOCOL_MISMATCH"},
            {12191, "ERROR_WINHTTP_GLOBAL_CALLBACK_FAILED"},
            {12192, "ERROR_WINHTTP_FEATURE_DISABLED"}
        };
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINHTTP_AUTOPROXY_OPTIONS
    {
        public AutoProxyFlag dwFlags;
        public AutoDetectFlag dwAutoDetectFlags;
        public string lpszAutoConfigUrl;
        public IntPtr lpvReserved;
        public int dwReserved;
        public bool fAutoLogonIfChallenged;
    }

    [Flags]
    public enum AccessType
    {
        WINHTTP_ACCESS_TYPE_NO_PROXY        = 0x1,
        WINHTTP_ACCESS_TYPE_DEFAULT_PROXY   = 0x0,
        WINHTTP_ACCESS_TYPE_NAMED_PROXY     = 0x3,
        WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY = 0x4
    }

    [Flags]
    public enum OpenFlag
    {
        WINHTTP_FLAG_ASYNC           = 0x10000000,
        WINHTTP_FLAG_SECURE_DEFAULTS = 0x30000000
    }

    [Flags]
    public enum AutoProxyFlag
    {
        WINHTTP_AUTOPROXY_AUTO_DETECT         = 0x00000001,
        WINHTTP_AUTOPROXY_CONFIG_URL          = 0x00000002,
        WINHTTP_AUTOPROXY_HOST_KEEPCASE       = 0x00000004,
        WINHTTP_AUTOPROXY_HOST_LOWERCASE      = 0x00000008,
        WINHTTP_AUTOPROXY_ALLOW_AUTOCONFIG    = 0x00000100,
        WINHTTP_AUTOPROXY_ALLOW_STATIC        = 0x00000200,
        WINHTTP_AUTOPROXY_ALLOW_CM            = 0x00000400,
        WINHTTP_AUTOPROXY_RUN_INPROCESS       = 0x00010000,
        WINHTTP_AUTOPROXY_RUN_OUTPROCESS_ONLY = 0x00020000,
        WINHTTP_AUTOPROXY_NO_DIRECTACCESS     = 0x00040000,
        WINHTTP_AUTOPROXY_NO_CACHE_CLIENT     = 0x00080000,
        WINHTTP_AUTOPROXY_NO_CACHE_SVC        = 0x00100000
    }

    [Flags]
    public enum AutoDetectFlag
    {
        WINHTTP_AUTO_DETECT_TYPE_DHCP  = 0x00000001,
        WINHTTP_AUTO_DETECT_TYPE_DNS_A = 0x00000002
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINHTTP_PROXY_INFO
    {
        public AccessType dwAccessType;
        public string lpszProxy;
        public string lpszProxyBypass;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WINHTTP_CURRENT_USER_IE_PROXY_CONFIG
    {
        public bool fAutoDetect;
        public string lpszAutoConfigUrl;
        public string lpszProxy;
        public string lpszProxyBypass;
    }

    [Flags]
    public enum ResetFlag
    {
        WINHTTP_RESET_STATE                  = 0x00000001,
        WINHTTP_RESET_SWPAD_CURRENT_NETWORK  = 0x00000002,
        WINHTTP_RESET_SWPAD_ALL              = 0x00000004,
        WINHTTP_RESET_SCRIPT_CACHE           = 0x00000008,
        WINHTTP_RESET_ALL                    = 0x0000FFFF,
        WINHTTP_RESET_NOTIFY_NETWORK_CHANGED = 0x00010000,
        WINHTTP_RESET_OUT_OF_PROC            = 0x00020000,
        WINHTTP_RESET_DISCARD_RESOLVERS      = 0x00040000
    }
}
