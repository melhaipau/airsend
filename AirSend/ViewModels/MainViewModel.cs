// File: AirSend/ViewModels/MainViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AirSend.Models;
using AirSend.Services;
using AirSend.Services.Senders;
using AirSend.Utils;

namespace AirSend.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<DiscoveredDevice> Devices { get; } = new();
        private readonly DeviceDiscoveryService _discovery;
        private readonly AudioCaptureService _capture;
        private readonly IAudioSender _sender;

        private DiscoveredDevice? _selectedDevice;
        public DiscoveredDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetProperty(ref _selectedDevice, value))
                {
                    Log.Info(value != null
                        ? $"Selected device: {value.Name} ({value.IP})"
                        : "Selected device: <none>");
                    RefreshCommands();
                }
            }
        }

        private float _volume = 0.75f;
        public float Volume
        {
            get => _volume;
            set
            {
                if (SetProperty(ref _volume, value))
                    _sender.SetVolume(value);
            }
        }

        private bool _audioOnlyMode = true;
        public bool AudioOnlyMode
        {
            get => _audioOnlyMode;
            set
            {
                if (SetProperty(ref _audioOnlyMode, value))
                {
                    Log.Info($"Audio-only mode: {(value ? "ON" : "OFF")}");
                    RefreshCommands();
                }
            }
        }

        public ICommand StartDiscoveryCommand { get; }
        public ICommand StopDiscoveryCommand { get; }
        public RelayCommand StartStreamCommand { get; }
        public RelayCommand StopStreamCommand { get; }

        public MainViewModel()
        {
            _sender = new MockRaopSender();
            _discovery = new DeviceDiscoveryService();
            _discovery.DeviceFound += OnDeviceFound;
            _discovery.DeviceRemoved += OnDeviceRemoved;
            _capture = new AudioCaptureService(_sender);

            StartDiscoveryCommand = new RelayCommand(async () =>
            {
                Log.Info("Starting discovery…");
                await _discovery.StartAsync();
            });

            StopDiscoveryCommand = new RelayCommand(() =>
            {
                Log.Info("Stopping discovery.");
                _discovery.Stop();
            });

            StartStreamCommand = new RelayCommand(StartStream, CanStartStream);
            StopStreamCommand = new RelayCommand(StopStream, () => _isStreaming);
        }

        private bool _isStreaming;

        private bool CanStartStream()
        {
            if (SelectedDevice == null) return false;
            if (AudioOnlyMode && !LooksLikeHomePod(SelectedDevice)) return false;
            return !_isStreaming;
        }

        private void StartStream()
        {
            if (SelectedDevice == null)
            {
                Log.Warn("No device selected.");
                return;
            }

            if (AudioOnlyMode && !LooksLikeHomePod(SelectedDevice))
            {
                Log.Warn($"Audio-only mode is ON. '{SelectedDevice.Name}' does not look like a HomePod.");
                return;
            }

            Log.Info($"Connecting to {SelectedDevice.Name} @ {SelectedDevice.IP}:{SelectedDevice.Port} ({SelectedDevice.ServiceType})…");
            try
            {
                _sender.ConnectToDevice(SelectedDevice.Id, SelectedDevice.IP, SelectedDevice.Port, SelectedDevice.ServiceType);
                _capture.Start();
                _isStreaming = true;
                RefreshCommands();
                Log.Info("Streaming started (mock sender). Replace MockRaopSender with a real RAOP sender to actually hear audio.");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start streaming: {ex.Message}");
            }
        }

        private void StopStream()
        {
            Log.Info("Stopping stream…");
            try
            {
                _capture.Stop();
                _sender.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error($"Error stopping stream: {ex.Message}");
            }
            finally
            {
                _isStreaming = false;
                RefreshCommands();
                Log.Info("Streaming stopped.");
            }
        }

        private static bool LooksLikeHomePod(DiscoveredDevice d)
        {
            // Heuristic: name contains "HomePod" (case-insensitive).
            // You could also inspect Zeroconf TXT records for model info if you extend the model.
            return (d.Name ?? string.Empty).IndexOf("HomePod", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void OnDeviceFound(DiscoveredDevice d)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!Devices.Any(x => x.Id == d.Id))
                {
                    Devices.Add(d);
                    Log.Info($"Found device: {d.Name} ({d.IP}) {d.ServiceType}");
                }
            });
        }

        private void OnDeviceRemoved(string id)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var existing = Devices.FirstOrDefault(x => x.Id == id);
                if (existing != null)
                {
                    Devices.Remove(existing);
                    Log.Warn($"Device disappeared: {existing.Name} ({existing.IP})");
                }
            });
        }

        private void RefreshCommands()
        {
            StartStreamCommand.RaiseCanExecuteChanged();
            StopStreamCommand.RaiseCanExecuteChanged();
        }
    }
}
