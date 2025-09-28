
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AirSend.Models;
using AirSend.Services;
using AirSend.Services.Senders;

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
            set => SetProperty(ref _selectedDevice, value);
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

        public ICommand StartDiscoveryCommand { get; }
        public ICommand StopDiscoveryCommand { get; }
        public ICommand StartStreamCommand { get; }
        public ICommand StopStreamCommand { get; }

        public MainViewModel()
        {
            _sender = new MockRaopSender();
            _discovery = new DeviceDiscoveryService();
            _discovery.DeviceFound += OnDeviceFound;
            _discovery.DeviceRemoved += OnDeviceRemoved;
            _capture = new AudioCaptureService(_sender);

            StartDiscoveryCommand = new RelayCommand(async () => await _discovery.StartAsync());
            StopDiscoveryCommand = new RelayCommand(() => _discovery.Stop());
            StartStreamCommand = new RelayCommand(() =>
            {
                var d = SelectedDevice ?? Devices.FirstOrDefault();
                if (d != null)
                {
                    _sender.ConnectToDevice(d.Id, d.IP, d.Port, d.ServiceType);
                    _capture.Start();
                }
            });
            StopStreamCommand = new RelayCommand(() =>
            {
                _capture.Stop();
                _sender.Disconnect();
            });
        }

        private void OnDeviceFound(DiscoveredDevice d)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                if (!Devices.Any(x => x.Id == d.Id)) Devices.Add(d);
            });
        }

        private void OnDeviceRemoved(string id)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var existing = Devices.FirstOrDefault(x => x.Id == id);
                if (existing != null) Devices.Remove(existing);
            });
        }
    }
}
