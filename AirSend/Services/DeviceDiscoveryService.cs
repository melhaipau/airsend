// File: AirSend/Services/DeviceDiscoveryService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;
using AirSend.Models;
using AirSend.Utils;

namespace AirSend.Services
{
    public class DeviceDiscoveryService
    {
        public event Action<DiscoveredDevice>? DeviceFound;
        public event Action<string>? DeviceRemoved;

        private CancellationTokenSource? _cts;

        public async Task StartAsync()
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                var known = new Dictionary<string, DiscoveredDevice>();

                while (!_cts!.IsCancellationRequested)
                {
                    try
                    {
                        var results = await ZeroconfResolver.ResolveAsync(
                            new[] { "_airplay._tcp.local.", "_raop._tcp.local." },
                            scanTime: TimeSpan.FromSeconds(2)
                        );

                        var current = new HashSet<string>();

                        foreach (var host in results)
                        {
                            var ip = (host.IPAddresses != null && host.IPAddresses.Count > 0)
                                ? host.IPAddresses[0]
                                : host.IPAddress;

                            var hostLabel = !string.IsNullOrWhiteSpace(host.DisplayName) ? host.DisplayName : ip;

                            foreach (var svc in host.Services)
                            {
                                var svcType = svc.Key;
                                var port = svc.Value.Port;
                                var id = $"{host.DisplayName}@{ip}:{port}/{svcType}";

                                current.Add(id);
                                if (!known.ContainsKey(id))
                                {
                                    var dev = new DiscoveredDevice
                                    {
                                        Id = id,
                                        Name = host.DisplayName,
                                        Hostname = hostLabel,
                                        IP = ip,
                                        Port = port,
                                        ServiceType = svcType,
                                        IsOnline = true
                                    };
                                    known[id] = dev;
                                    DeviceFound?.Invoke(dev);
                                }
                                else
                                {
                                    known[id].IsOnline = true;
                                }
                            }
                        }

                        var toRemove = known.Keys.Where(k => !current.Contains(k)).ToList();
                        foreach (var k in toRemove)
                        {
                            known[k].IsOnline = false;
                            DeviceRemoved?.Invoke(k);
                            known.Remove(k);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Discovery error: {ex.Message}");
                    }

                    try { await Task.Delay(5000, _cts.Token); }
                    catch (OperationCanceledException) { break; }
                }
            }, _cts.Token);

            await Task.CompletedTask;
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts = null;
        }
    }
}
