// File: AirSend/Services/Senders/MockRaopSender.cs
using NAudio.Wave;
using AirSend.Utils;

namespace AirSend.Services.Senders
{
    public class MockRaopSender : IAudioSender
    {
        private string? _targetIp;
        private int _port;

        public void ConnectToDevice(string deviceId, string ip, int port, string serviceType)
        {
            _targetIp = ip;
            _port = port;
            Log.Info($"[MockSender] Connect -> {ip}:{port} ({serviceType})");
        }

        public void Disconnect()
        {
            Log.Info("[MockSender] Disconnect");
            _targetIp = null;
        }

        public void SendAudio(byte[] buffer, int offset, int count, WaveFormat format)
        {
            if (string.IsNullOrEmpty(_targetIp)) return;
            // Just log throughput for now
            // For real streaming, replace this class with a RAOP/AirPlay sender implementation
        }

        public void SetVolume(float vol)
        {
            Log.Info($"[MockSender] Volume set to {vol:0.00}");
        }
    }
}
