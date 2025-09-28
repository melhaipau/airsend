
using System;
using NAudio.Wave;

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
            System.Diagnostics.Debug.WriteLine($"[MockSender] Connecting to {ip}:{port} ({serviceType})");
        }

        public void Disconnect()
        {
            System.Diagnostics.Debug.WriteLine("[MockSender] Disconnect");
            _targetIp = null;
        }

        public void SendAudio(byte[] buffer, int offset, int count, WaveFormat format)
        {
            if (string.IsNullOrEmpty(_targetIp)) return;
            // Placeholder: here you would packetize & send via RTP/RTSP for RAOP/AirPlay
            // This mock just logs throughput.
            System.Diagnostics.Debug.WriteLine($"[MockSender] {count} bytes -> {_targetIp}:{_port}");
        }

        public void SetVolume(float vol)
        {
            System.Diagnostics.Debug.WriteLine($"[MockSender] Volume {vol:0.00}");
        }
    }
}
