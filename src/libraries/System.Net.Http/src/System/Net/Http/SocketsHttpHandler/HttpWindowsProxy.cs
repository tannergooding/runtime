// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using SafeWinHttpHandle = Interop.WinHttp.SafeWinHttpHandle;

namespace System.Net.Http
{
    internal sealed class HttpWindowsProxy : IMultiWebProxy, IDisposable
    {
        private readonly RegistryKey? _internetSettingsRegistry = Registry.CurrentUser?.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
        private MultiProxy _insecureProxy;      // URI of the http system proxy if set
        private MultiProxy _secureProxy;       // URI of the https system proxy if set
        private FailedProxyCache _failedProxies = new FailedProxyCache();
        private List<string>? _bypass;          // list of domains not to proxy
        private List<IPAddress>? _localIp;
        private ICredentials? _credentials;
        private WinInetProxyHelper _proxyHelper;
        private SafeWinHttpHandle? _sessionHandle;
        private bool _disposed;
        private EventWaitHandle _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        private const int RegistrationFlags = Interop.Advapi32.REG_NOTIFY_CHANGE_NAME | Interop.Advapi32.REG_NOTIFY_CHANGE_LAST_SET | Interop.Advapi32.REG_NOTIFY_CHANGE_ATTRIBUTES | Interop.Advapi32.REG_NOTIFY_THREAD_AGNOSTIC;
        private RegisteredWaitHandle? _registeredWaitHandle;

        // 'proxy' used from tests via Reflection
        public HttpWindowsProxy(WinInetProxyHelper? proxy = null)
        {

            if (_internetSettingsRegistry != null && proxy == null)
            {
                // we register for change notifications so we can react to changes during lifetime.
                if (Interop.Advapi32.RegNotifyChangeKeyValue(_internetSettingsRegistry.Handle, true, RegistrationFlags, _waitHandle.SafeWaitHandle, true) == 0)
                {
                    _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, RegistryChangeNotificationCallback, this, -1, false);
                }
            }

            UpdateConfiguration(proxy);
        }

        private static void RegistryChangeNotificationCallback(object? state, bool timedOut)
        {
            HttpWindowsProxy proxy = (HttpWindowsProxy)state!;
            if (!proxy._disposed)
            {

                // This is executed from threadpool. we should not ever throw here.
                try
                {
                    // We need to register for notification every time. We regisrerand lock before we process configuration
                    // so if there is update it would be serialized to ensure consistency.
                    Interop.Advapi32.RegNotifyChangeKeyValue(proxy._internetSettingsRegistry!.Handle, true, RegistrationFlags, proxy._waitHandle.SafeWaitHandle, true);
                    lock (proxy)
                    {
                        proxy.UpdateConfiguration();
                    }
                }
                catch (Exception ex)
                {
                    if (NetEventSource.Log.IsEnabled()) NetEventSource.Error(proxy, $"Failed to refresh proxy configuration: {ex.Message}");
                }
            }
        }

        [MemberNotNull(nameof(_proxyHelper))]
        private void UpdateConfiguration(WinInetProxyHelper? proxyHelper = null)
        {

            proxyHelper ??= new WinInetProxyHelper();

            if (proxyHelper.AutoSettingsUsed)
            {
                if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(proxyHelper, $"AutoSettingsUsed, calling {nameof(Interop.WinHttp.WinHttpOpen)}");
                SafeWinHttpHandle? sessionHandle = Interop.WinHttp.WinHttpOpen(
                    IntPtr.Zero,
                    Interop.WinHttp.WINHTTP_ACCESS_TYPE_NO_PROXY,
                    Interop.WinHttp.WINHTTP_NO_PROXY_NAME,
                    Interop.WinHttp.WINHTTP_NO_PROXY_BYPASS,
                    (int)Interop.WinHttp.WINHTTP_FLAG_ASYNC);

                if (sessionHandle.IsInvalid)
                {
                    // Proxy failures are currently ignored by managed handler.
                    if (NetEventSource.Log.IsEnabled()) NetEventSource.Error(proxyHelper, $"{nameof(Interop.WinHttp.WinHttpOpen)} returned invalid handle");
                    sessionHandle.Dispose();
                }

                _sessionHandle = sessionHandle;
            }

            if (proxyHelper.ManualSettingsUsed)
            {
                if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(proxyHelper, $"ManualSettingsUsed, {proxyHelper.Proxy}");

                _secureProxy = MultiProxy.ParseManualSettings(_failedProxies, proxyHelper.Proxy, true);
                _insecureProxy = MultiProxy.ParseManualSettings(_failedProxies, proxyHelper.Proxy, false);

                if (!string.IsNullOrWhiteSpace(proxyHelper.ProxyBypass))
                {
                    int idx = 0;
                    string? tmp;
                    bool bypassLocal = false;
                    List<IPAddress>? localIp = null;

                    // Process bypass list for manual setting.
                    // Initial list size is best guess based on string length assuming each entry is at least 5 characters on average.
                    List<string>? bypass = new List<string>(proxyHelper.ProxyBypass.Length / 5);

                    while (idx < proxyHelper.ProxyBypass.Length)
                    {
                        // Strip leading spaces and scheme if any.
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ' ') { idx += 1; };
                        if (string.Compare(proxyHelper.ProxyBypass, idx, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 7;
                        }
                        else if (string.Compare(proxyHelper.ProxyBypass, idx, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 8;
                        }

                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == '[')
                        {
                            // Strip [] from IPv6 so we can use IdnHost laster for matching.
                            idx += 1;
                        }

                        int start = idx;
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ' ' && proxyHelper.ProxyBypass[idx] != ';' && proxyHelper.ProxyBypass[idx] != ']') { idx += 1; };

                        if (idx == start)
                        {
                            // Empty string.
                            tmp = null;
                        }
                        else if (string.Compare(proxyHelper.ProxyBypass, start, "<local>", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            bypassLocal = true;
                            tmp = null;
                        }
                        else
                        {
                            tmp = proxyHelper.ProxyBypass.Substring(start, idx - start);
                        }

                        // Skip trailing characters if any.
                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';')
                        {
                            // Got stopped at space or ']'. Strip until next ';' or end.
                            while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';') { idx += 1; };
                        }
                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ';')
                        {
                            idx++;
                        }
                        if (tmp == null)
                        {
                            continue;
                        }

                        bypass.Add(tmp);
                    }

                    _bypass = bypass.Count > 0 ? bypass : null;

                    if (bypassLocal)
                    {
                        localIp = new List<IPAddress>();
                        foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                        {
                            IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                            foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                            {
                                localIp.Add(addr.Address);
                            }
                        }
                    }

                    _localIp = localIp?.Count > 0 ? localIp : null;
                }
            }

            _proxyHelper = proxyHelper;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_sessionHandle != null && !_sessionHandle.IsInvalid)
                {
                    SafeWinHttpHandle.DisposeAndClearHandle(ref _sessionHandle);
                }

                _waitHandle?.Dispose();
                _internetSettingsRegistry?.Dispose();
                _registeredWaitHandle?.Unregister(null);
            }
        }

        /// <summary>
        /// Gets the proxy URI. (IWebProxy interface)
        /// </summary>
        public Uri? GetProxy(Uri uri)
        {
            if (!_proxyHelper.AutoSettingsUsed && !_proxyHelper.ManualSettingsOnly)
            {
                return null;
            }

            GetMultiProxy(uri).ReadNext(out Uri? proxyUri, out _);
            return proxyUri;
        }

        /// <summary>
        /// Gets the proxy URIs.
        /// </summary>
        public MultiProxy GetMultiProxy(Uri uri)
        {
            // We need WinHTTP to detect and/or process a PAC (JavaScript) file. This maps to
            // "Automatically detect settings" and/or "Use automatic configuration script" from IE
            // settings. But, calling into WinHTTP can be slow especially when it has to call into
            // the out-of-process service to discover, load, and run the PAC file. So, we skip
            // calling into WinHTTP if there was a recent failure to detect a PAC file on the network.
            // This is a common error. The default IE settings on a Windows machine consist of the
            // single checkbox for "Automatically detect settings" turned on and most networks
            // won't actually discover a PAC file on the network since WPAD protocol isn't configured.
            if (_proxyHelper.AutoSettingsUsed && !_proxyHelper.RecentAutoDetectionFailure)
            {
                Interop.WinHttp.WINHTTP_PROXY_INFO proxyInfo = default;
                try
                {
                    if (_proxyHelper.GetProxyForUrl(_sessionHandle, uri, out proxyInfo))
                    {
                        // If WinHTTP just specified a Proxy with no ProxyBypass list, then
                        // we can return the Proxy uri directly.
                        if (proxyInfo.ProxyBypass == IntPtr.Zero)
                        {
                            if (proxyInfo.Proxy != IntPtr.Zero)
                            {
                                string proxyStr = Marshal.PtrToStringUni(proxyInfo.Proxy)!;

                                return MultiProxy.CreateLazy(_failedProxies, proxyStr, IsSecureUri(uri));
                            }
                            else
                            {
                                return MultiProxy.Empty;
                            }
                        }

                        // A bypass list was also specified. This means that WinHTTP has fallen back to
                        // using the manual IE settings specified and there is a ProxyBypass list also.
                        // Since we're not really using the full WinHTTP stack, we need to use HttpSystemProxy
                        // to do the computation of the final proxy uri merging the information from the Proxy
                        // and ProxyBypass strings.
                    }
                    else
                    {
                        return MultiProxy.Empty;
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(proxyInfo.Proxy);
                    Marshal.FreeHGlobal(proxyInfo.ProxyBypass);
                }
            }

            // Fallback to manual settings if present.
            if (_proxyHelper.ManualSettingsUsed)
            {
                if (_localIp != null)
                {
                    IPAddress? address;

                    if (uri.IsLoopback)
                    {
                        // This is optimization for loopback addresses.
                        // Unfortunately this does not work for all local addresses.
                        return MultiProxy.Empty;
                    }

                    // Pre-Check if host may be IP address to avoid parsing.
                    if (uri.HostNameType == UriHostNameType.IPv6 || uri.HostNameType == UriHostNameType.IPv4)
                    {
                        // RFC1123 allows labels to start with number.
                        // Leading number may or may not be IP address.
                        // IPv6 [::1] notation. '[' is not valid character in names.
                        if (IPAddress.TryParse(uri.IdnHost, out address))
                        {
                            // Host is valid IP address.
                            // Check if it belongs to local system.
                            foreach (IPAddress a in _localIp)
                            {
                                if (a.Equals(address))
                                {
                                    return MultiProxy.Empty;
                                }
                            }
                        }
                    }
                    if (uri.HostNameType != UriHostNameType.IPv6 && !uri.IdnHost.Contains('.'))
                    {
                        // Not address and does not have a dot.
                        // Hosts without FQDN are considered local.
                        return MultiProxy.Empty;
                    }
                }

                // Check if we have other rules for bypass.
                if (_bypass != null)
                {
                    foreach (string entry in _bypass)
                    {
                        // IdnHost does not have [].
                        if (SimpleRegex.IsMatchWithStarWildcard(uri.IdnHost, entry))
                        {
                            return MultiProxy.Empty;
                        }
                    }
                }

                // We did not find match on bypass list.
                return IsSecureUri(uri) ? _secureProxy : _insecureProxy;
            }

            return MultiProxy.Empty;
        }

        private static bool IsSecureUri(Uri uri)
        {
            return uri.Scheme == UriScheme.Https || uri.Scheme == UriScheme.Wss;
        }

        /// <summary>
        /// Checks if URI is subject to proxy or not.
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            // This HttpSystemProxy class is only consumed by SocketsHttpHandler and is not exposed outside of
            // SocketsHttpHandler. The current pattern for consumption of IWebProxy is to call IsBypassed first.
            // If it returns false, then the caller will call GetProxy. For this proxy implementation, computing
            // the return value for IsBypassed is as costly as calling GetProxy. We want to avoid doing extra
            // work. So, this proxy implementation for the IsBypassed method can always return false. Then the
            // GetProxy method will return non-null for a proxy, or null if no proxy should be used.
            return false;
        }

        public ICredentials? Credentials
        {
            get
            {
                return _credentials;
            }
            set
            {
                _credentials = value;
            }
        }

        // Access function for unit tests.
        internal List<string>? BypassList => _bypass;
    }
}
