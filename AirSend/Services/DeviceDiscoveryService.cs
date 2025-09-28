
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;
using AirSend.Models;

namespace AirSend.Services
{
    public class DeviceDiscoveryService
    {
        public event Action<DiscoveredDevice>? DeviceFound;
        public event Action<string>? DeviceRemoved;

        private CancellationTokenSource? _cts;

        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();

            _ = Task.Run(async () =>
            {
                var known = new Dictionary<string, DiscoveredDevice>();

                while (!_cts!.IsCancellationRequested)
                {
                    try
                    {
                        var results = await ZeroconfResolver.ResolveAsync(new string[] { "_airplay._tcp.local.", "_raop._tcp.local." }, scanTime: TimeSpan.FromSeconds(2));
                        var current = new HashSet<string>();

                        foreach (var r in results)
                        {
                            var ip = r.IPAddress;
                            var name = r.DisplayName;
                            foreach (var svc in r.Services)
                            {
                                var svcType = svc.Key; // _airplay._tcp or _raop._tcp
                                var port = svc.Value.Port;
                                var id = $"{name}@{ip}:{port}/{svcType}";
                                current.Add(id);
                                if (!known.ContainsKey(id))
                                {
                                    var dev = new DiscoveredDevice
                                    {
                                        Id = id,
                                        Name = name,
                                        Hostname = r.HostName,
                                        IP = ip,
                                        Port = port,
                                        ServiceType = svcType
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
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Discovery error: {ex.Message}");
                    }

                    await Task.Delay(5000, _cts.Token);
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
    }
}
